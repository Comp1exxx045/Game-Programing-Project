using UnityEngine;

public class SawHorizontalMover : MonoBehaviour
{
    [Header("Horizontal Movement")]
    [SerializeField] private float leftX = 6f;
    [SerializeField] private float rightX = 10f;
    [SerializeField, Min(0f)] private float moveSpeed = 6f;
    [SerializeField] private bool startMovingRight = true;

    private float currentTargetX;

    /// <summary>
    /// Validates the movement range and selects the first horizontal target.
    /// </summary>
    private void Awake()
    {
        EnsureValidRange();
        currentTargetX = startMovingRight ? rightX : leftX;
    }

    /// <summary>
    /// Moves the saw between the configured world-space X coordinates.
    /// </summary>
    private void Update()
    {
        Vector3 position = transform.position;
        position.x = Mathf.MoveTowards(position.x, currentTargetX, moveSpeed * Time.deltaTime);
        transform.position = position;

        if (Mathf.Approximately(position.x, currentTargetX))
        {
            currentTargetX = Mathf.Approximately(currentTargetX, rightX) ? leftX : rightX;
        }
    }

    /// <summary>
    /// Keeps Inspector values valid while editing the movement settings.
    /// </summary>
    private void OnValidate()
    {
        moveSpeed = Mathf.Max(0f, moveSpeed);
        EnsureValidRange();
    }

    /// <summary>
    /// Orders the two X coordinates so leftX is never greater than rightX.
    /// </summary>
    private void EnsureValidRange()
    {
        if (leftX <= rightX)
        {
            return;
        }

        (leftX, rightX) = (rightX, leftX);
    }

    /// <summary>
    /// Draws the horizontal patrol range when the saw is selected.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        float y = transform.position.y;
        float z = transform.position.z;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(leftX, y, z), new Vector3(rightX, y, z));
        Gizmos.DrawWireSphere(new Vector3(leftX, y, z), 0.15f);
        Gizmos.DrawWireSphere(new Vector3(rightX, y, z), 0.15f);
    }
}
