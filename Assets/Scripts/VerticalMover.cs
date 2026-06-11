using UnityEngine;

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

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = rb.position;
    }

    void FixedUpdate()
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
