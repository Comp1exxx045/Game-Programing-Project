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
    private bool isGrounded;
    private bool isTouchingLeftWall;
    private bool isTouchingRightWall;
    private bool isWallSliding;
    private int wallSide;
    private float jumpBufferCounter;
    private float leftWallCoyoteCounter;
    private float rightWallCoyoteCounter;
    private float wallJumpControlLockCounter;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.gravityScale = gravityScale;
    }

    void Update()
    {
        UpdateJumpBuffer();
        CheckGrounded();
        CheckWalls();
        UpdateWallCoyoteTime();
        UpdateWallJumpControlLock();
        HandleMovement();
        HandleWallSlide();
        HandleJump();
    }

    void HandleMovement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        if (wallJumpControlLockCounter > 0f)
        {
            return;
        }

        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
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
