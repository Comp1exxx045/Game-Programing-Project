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

    void Start()
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

    void Update()
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

    void HandleMovement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        UpdateFacing(moveInput);

        if (wallJumpControlLockCounter > 0f)
        {
            return;
        }

        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    void UpdateFacing(float horizontalInput)
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

    void SetSpriteFlip(bool flipX)
    {
        foreach (var spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = flipX;
            }
        }
    }

    void HandleWallSlide()
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

    void HandleJump()
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

    void CheckGrounded()
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

    void CheckWalls()
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

    void UpdateJumpBuffer()
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

    void UpdateWallCoyoteTime()
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

    void UpdateWallJumpControlLock()
    {
        if (wallJumpControlLockCounter > 0f)
        {
            wallJumpControlLockCounter -= Time.deltaTime;
        }
    }

    void UpdateAnimatorParameters()
    {
        if (animator == null || rb == null)
        {
            return;
        }

        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("VerticalVelocity", rb.velocity.y);
        animator.SetBool("IsGrounded", isGrounded);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TryKillPlayer(other);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        TryKillPlayer(collision.collider);
    }

    private void TryKillPlayer(Collider2D other)
    {
        if (other != null &&
            other.CompareTag("Trap") &&
            GameManager.Instance != null)
        {
            GameManager.Instance.KillPlayer();
        }
    }

    public void ResetPlayerState()
    {
        jumpBufferCounter = 0f;
        leftWallCoyoteCounter = 0f;
        rightWallCoyoteCounter = 0f;
        wallJumpControlLockCounter = 0f;
        isWallSliding = false;
        wallSide = 0;

        if (animator != null)
        {
            animator.ResetTrigger("Die");
            animator.Play("Idle", 0, 0f);
        }
    }

    public void SetControlEnabled(bool enabled)
    {
        controlEnabled = enabled;
    }

    public void PlayDeathAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
    }

    void OnDrawGizmosSelected()
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
