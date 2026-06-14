using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 16f;

    [Header("Jump")]
    public float jumpForce = 12f;
    public float jumpBufferTime = 0.12f;

    [Header("Wall Jump")]
    public Transform leftWallCheck;
    public Transform rightWallCheck;
    public float wallCheckRadius = 0.2f;
    public LayerMask wallLayer;
    public float wallSlideSpeed = 2f;
    public float wallJumpHorizontalForce = 14f;
    public float wallJumpVerticalForce = 12f;
    public float wallJumpControlLockTime = 0.15f;
    public float wallCoyoteTime = 0.12f;

    [Header("Gravity")]
    public float gravityScale = 3f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer[] spriteRenderers;
    private bool isGrounded;
    private bool isTouchingLeftWall;
    private bool isTouchingRightWall;
    private bool isWallSliding;
    private int wallSide;
    private float jumpBufferCounter;
    private float leftWallCoyoteCounter;
    private float rightWallCoyoteCounter;
    private float wallJumpControlLockCounter;
    private bool controlEnabled = true;
    private MovingPlatform2D currentMovingPlatform;

    /// <summary>
    /// Caches player components and applies the configured physics settings.
    /// </summary>
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        Animator[] animators = GetComponentsInChildren<Animator>();
        foreach (Animator childAnimator in animators)
        {
            if (childAnimator.transform != transform)
            {
                animator = childAnimator;
                break;
            }
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        rb.freezeRotation = true;
        rb.gravityScale = gravityScale;
    }

    /// <summary>
    /// Updates movement, jumping, wall interactions, and animation each frame.
    /// </summary>
    private void Update()
    {
        if (!controlEnabled)
        {
            UpdateAnimatorParameters();
            return;
        }

        UpdateJumpBuffer();
        CheckGrounded();
        CheckWalls();
        UpdateWallCoyoteTime();
        UpdateWallJumpControlLock();
        HandleMovement();
        HandleWallSlide();
        HandleJump();
        UpdateAnimatorParameters();
    }

    /// <summary>
    /// Applies horizontal input and inherits horizontal moving-platform velocity.
    /// </summary>
    private void HandleMovement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        UpdateFacing(moveInput);

        if (wallJumpControlLockCounter > 0f)
        {
            return;
        }

        float platformVelocityX = isGrounded && currentMovingPlatform != null
            ? currentMovingPlatform.CurrentVelocity.x
            : 0f;

        rb.velocity = new Vector2(
            moveInput * moveSpeed + platformVelocityX,
            rb.velocity.y
        );
    }

    /// <summary>
    /// Updates sprite orientation from horizontal movement input.
    /// </summary>
    private void UpdateFacing(float horizontalInput)
    {
        if (spriteRenderers == null || spriteRenderers.Length == 0)
        {
            return;
        }

        if (Mathf.Abs(horizontalInput) < 0.01f && wallJumpControlLockCounter > 0f && rb != null)
        {
            horizontalInput = rb.velocity.x;
        }

        if (horizontalInput > 0.01f)
        {
            SetSpriteFlip(false);
        }
        else if (horizontalInput < -0.01f)
        {
            SetSpriteFlip(true);
        }
    }

    /// <summary>
    /// Applies the requested horizontal flip to every player sprite.
    /// </summary>
    private void SetSpriteFlip(bool flipX)
    {
        foreach (var spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = flipX;
            }
        }
    }

    /// <summary>
    /// Limits falling speed while the player presses into a wall.
    /// </summary>
    private void HandleWallSlide()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        bool pressingIntoLeftWall = isTouchingLeftWall && moveInput < 0f;
        bool pressingIntoRightWall = isTouchingRightWall && moveInput > 0f;
        bool pressingIntoWall = pressingIntoLeftWall || pressingIntoRightWall;

        isWallSliding = !isGrounded && pressingIntoWall && rb.velocity.y <= 0f;

        if (!isWallSliding)
        {
            wallSide = 0;
            return;
        }

        wallSide = pressingIntoLeftWall ? -1 : 1;
        rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
    }

    /// <summary>
    /// Performs buffered ground jumps and directional wall jumps.
    /// </summary>
    private void HandleJump()
    {
        if (jumpBufferCounter <= 0f)
        {
            return;
        }

        float moveInput = Input.GetAxisRaw("Horizontal");
        float wallJumpDirection = 0f;

        if (!isGrounded && leftWallCoyoteCounter > 0f && moveInput > 0f)
        {
            wallJumpDirection = 1f;
        }
        else if (!isGrounded && rightWallCoyoteCounter > 0f && moveInput < 0f)
        {
            wallJumpDirection = -1f;
        }

        if (wallJumpDirection != 0f)
        {
            rb.velocity = new Vector2(
                wallJumpDirection * wallJumpHorizontalForce,
                wallJumpVerticalForce
            );
            wallJumpControlLockCounter = wallJumpControlLockTime;
            jumpBufferCounter = 0f;
            isWallSliding = false;
            return;
        }

        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpBufferCounter = 0f;
        }
    }

    /// <summary>
    /// Checks whether the ground probe overlaps the configured ground layer.
    /// </summary>
    private void CheckGrounded()
    {
        if (groundCheck == null)
        {
            Debug.LogWarning("GroundCheck is not assigned.");
            isGrounded = false;
            return;
        }

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    /// <summary>
    /// Checks whether either wall probe is touching the wall layer.
    /// </summary>
    private void CheckWalls()
    {
        isTouchingLeftWall = leftWallCheck != null && Physics2D.OverlapCircle(
            leftWallCheck.position,
            wallCheckRadius,
            wallLayer
        );

        isTouchingRightWall = rightWallCheck != null && Physics2D.OverlapCircle(
            rightWallCheck.position,
            wallCheckRadius,
            wallLayer
        );
    }

    /// <summary>
    /// Records jump input briefly so near-ground presses are not lost.
    /// </summary>
    private void UpdateJumpBuffer()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            jumpBufferCounter = jumpBufferTime;
            return;
        }

        if (jumpBufferCounter > 0f)
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Maintains short wall-jump grace periods after leaving a wall.
    /// </summary>
    private void UpdateWallCoyoteTime()
    {
        if (isTouchingLeftWall)
        {
            leftWallCoyoteCounter = wallCoyoteTime;
        }
        else if (leftWallCoyoteCounter > 0f)
        {
            leftWallCoyoteCounter -= Time.deltaTime;
        }

        if (isTouchingRightWall)
        {
            rightWallCoyoteCounter = wallCoyoteTime;
        }
        else if (rightWallCoyoteCounter > 0f)
        {
            rightWallCoyoteCounter -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Counts down the temporary horizontal control lock after a wall jump.
    /// </summary>
    private void UpdateWallJumpControlLock()
    {
        if (wallJumpControlLockCounter > 0f)
        {
            wallJumpControlLockCounter -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Sends movement and grounded values to the player Animator.
    /// </summary>
    private void UpdateAnimatorParameters()
    {
        if (animator == null || rb == null)
        {
            return;
        }

        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("VerticalVelocity", rb.velocity.y);
        animator.SetBool("IsGrounded", isGrounded);
    }

    /// <summary>
    /// Handles lethal trigger contacts.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        TryKillPlayer(other);
    }

    /// <summary>
    /// Handles lethal collision contacts.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryKillPlayer(collision.collider);
    }

    /// <summary>
    /// Tracks a moving platform while the player is standing on its upper surface.
    /// </summary>
    private void OnCollisionStay2D(Collision2D collision)
    {
        MovingPlatform2D platform =
            collision.collider.GetComponentInParent<MovingPlatform2D>();

        if (platform == null)
        {
            return;
        }

        for (int index = 0; index < collision.contactCount; index++)
        {
            if (collision.GetContact(index).normal.y > 0.5f)
            {
                currentMovingPlatform = platform;
                return;
            }
        }
    }

    /// <summary>
    /// Stops inheriting platform velocity after leaving the current platform.
    /// </summary>
    private void OnCollisionExit2D(Collision2D collision)
    {
        MovingPlatform2D platform =
            collision.collider.GetComponentInParent<MovingPlatform2D>();

        if (platform == currentMovingPlatform)
        {
            currentMovingPlatform = null;
        }
    }

    /// <summary>
    /// Kills the player when the contacted collider belongs to a trap.
    /// </summary>
    private void TryKillPlayer(Collider2D other)
    {
        if (other != null &&
            other.CompareTag("Trap") &&
            GameManager.Instance != null)
        {
            GameManager.Instance.KillPlayer();
        }
    }

    /// <summary>
    /// Clears temporary movement state after the player respawns.
    /// </summary>
    public void ResetPlayerState()
    {
        jumpBufferCounter = 0f;
        leftWallCoyoteCounter = 0f;
        rightWallCoyoteCounter = 0f;
        wallJumpControlLockCounter = 0f;
        isWallSliding = false;
        wallSide = 0;
        currentMovingPlatform = null;

        if (animator != null)
        {
            animator.ResetTrigger("Die");
            animator.Play("Idle", 0, 0f);
        }
    }

    /// <summary>
    /// Enables or disables player-controlled movement input.
    /// </summary>
    public void SetControlEnabled(bool enabled)
    {
        controlEnabled = enabled;
    }

    /// <summary>
    /// Requests the death animation when an Animator is available.
    /// </summary>
    public void PlayDeathAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
    }

    /// <summary>
    /// Draws ground and wall probes while the player is selected.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        Gizmos.color = Color.blue;

        if (leftWallCheck != null)
        {
            Gizmos.DrawWireSphere(leftWallCheck.position, wallCheckRadius);
        }

        if (rightWallCheck != null)
        {
            Gizmos.DrawWireSphere(rightWallCheck.position, wallCheckRadius);
        }
    }
}
