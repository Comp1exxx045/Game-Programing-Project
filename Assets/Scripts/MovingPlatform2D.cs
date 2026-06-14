using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Vector2 travelOffset = new Vector2(0f, 4f);
    [SerializeField, Min(0f)] private float moveSpeed = 2.5f;
    [SerializeField, Min(0f)] private float startDelay;
    [SerializeField, Min(0f)] private float endpointPause = 0.5f;
    [SerializeField] private bool startAtEnd;

    private Rigidbody2D platformBody;
    private Vector2 startPosition;
    private Vector2 endPosition;
    private Vector2 targetPosition;
    private float delayRemaining;
    private float pauseRemaining;

    public Vector2 CurrentVelocity { get; private set; }

    /// <summary>
    /// Caches the Rigidbody2D and calculates both movement endpoints.
    /// </summary>
    private void Awake()
    {
        platformBody = GetComponent<Rigidbody2D>();
        startPosition = platformBody.position;
        endPosition = startPosition + travelOffset;
    }

    /// <summary>
    /// Resets the platform position and applies its independently configured start delay.
    /// </summary>
    private void OnEnable()
    {
        if (platformBody == null)
        {
            platformBody = GetComponent<Rigidbody2D>();
            startPosition = platformBody.position;
            endPosition = startPosition + travelOffset;
        }

        Vector2 initialPosition = startAtEnd ? endPosition : startPosition;
        platformBody.position = initialPosition;
        targetPosition = startAtEnd ? startPosition : endPosition;
        delayRemaining = startDelay;
        pauseRemaining = 0f;
        CurrentVelocity = Vector2.zero;
    }

    /// <summary>
    /// Moves the platform toward its current endpoint using physics timing.
    /// </summary>
    private void FixedUpdate()
    {
        if (delayRemaining > 0f)
        {
            delayRemaining -= Time.fixedDeltaTime;
            CurrentVelocity = Vector2.zero;
            return;
        }

        if (pauseRemaining > 0f)
        {
            pauseRemaining -= Time.fixedDeltaTime;
            CurrentVelocity = Vector2.zero;
            return;
        }

        Vector2 nextPosition = Vector2.MoveTowards(
            platformBody.position,
            targetPosition,
            moveSpeed * Time.fixedDeltaTime
        );

        CurrentVelocity = (nextPosition - platformBody.position) / Time.fixedDeltaTime;
        platformBody.MovePosition(nextPosition);

        if ((nextPosition - targetPosition).sqrMagnitude <= 0.0001f)
        {
            targetPosition = targetPosition == endPosition ? startPosition : endPosition;
            pauseRemaining = endpointPause;
        }
    }

    /// <summary>
    /// Clears the reported velocity when the platform leaves the active world.
    /// </summary>
    private void OnDisable()
    {
        CurrentVelocity = Vector2.zero;
    }

    /// <summary>
    /// Keeps configurable timing and speed values within valid ranges.
    /// </summary>
    private void OnValidate()
    {
        moveSpeed = Mathf.Max(0f, moveSpeed);
        startDelay = Mathf.Max(0f, startDelay);
        endpointPause = Mathf.Max(0f, endpointPause);
    }

    /// <summary>
    /// Draws the configured movement path while the platform is selected.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Vector3 start = transform.position;
        Vector3 end = start + (Vector3)travelOffset;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(start, end);
        Gizmos.DrawWireSphere(start, 0.15f);
        Gizmos.DrawWireSphere(end, 0.15f);
    }
}
