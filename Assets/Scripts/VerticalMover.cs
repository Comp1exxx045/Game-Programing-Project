using UnityEngine;

/// <summary>
/// Moves a Rigidbody2D object up and down with a sinusoidal vertical motion.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class VerticalMover : MonoBehaviour
{
    [Min(0f)]
    public float moveDistance = 2f;

    [Min(0f)]
    public float moveSpeed = 1.5f;

    private Rigidbody2D rb;
    private Vector2 startPosition;
    private float elapsedTime;

    /// <summary>
    /// Caches the Rigidbody2D and records the starting position.
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = rb.position;
    }

    /// <summary>
    /// Moves the object vertically during the physics update.
    /// </summary>
    private void FixedUpdate()
    {
        if (moveDistance <= 0f || moveSpeed <= 0f)
        {
            rb.MovePosition(startPosition);
            return;
        }

        elapsedTime += Time.fixedDeltaTime;
        float verticalOffset = Mathf.Sin(elapsedTime * moveSpeed) * moveDistance;
        rb.MovePosition(startPosition + Vector2.up * verticalOffset);
    }
}
