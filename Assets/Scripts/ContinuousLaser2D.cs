using UnityEngine;

/// <summary>
/// Casts a continuous 2D laser beam, updates its visual length, and kills the player on contact.
/// </summary>
public class ContinuousLaser2D : MonoBehaviour
{
    private const string BeamVisualName = "BeamVisual";

    [Header("Beam")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private Vector2 localDirection = Vector2.right;
    [SerializeField, Min(0f)] private float maxDistance = 20f;
    [SerializeField, Min(0f)] private float collisionWidth = 0.12f;
    [SerializeField] private LayerMask collisionMask = ~0;
    [SerializeField] private bool triggersBlockBeam;

    [Header("Appearance")]
    [SerializeField] private Sprite laserSprite;
    [SerializeField, Min(0f)] private float visualWidth = 0.5f;
    [SerializeField] private int sortingOrder = 5;

    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// Creates the beam visual and configures rendering before the first physics update.
    /// </summary>
    private void Awake()
    {
        CreateBeamVisualIfNeeded();
        ConfigureSpriteRenderer();
        DisableOldLineRenderer();
    }

    /// <summary>
    /// Updates the laser collision and visual beam using the physics timestep.
    /// </summary>
    private void FixedUpdate()
    {
        UpdateLaser();
    }

    /// <summary>
    /// Keeps edit-time laser settings valid and refreshes the beam preview components.
    /// </summary>
    private void OnValidate()
    {
        if (localDirection.sqrMagnitude < 0.001f)
        {
            localDirection = Vector2.right;
        }

        Transform beamVisual = transform.Find(BeamVisualName);
        if (beamVisual != null)
        {
            spriteRenderer = beamVisual.GetComponent<SpriteRenderer>();
            ConfigureSpriteRenderer();
        }

        DisableOldLineRenderer();
    }

    /// <summary>
    /// Casts the laser, finds the closest blocking hit, and updates the visible beam.
    /// </summary>
    private void UpdateLaser()
    {
        Vector2 origin = firePoint != null ? firePoint.position : transform.position;
        Vector2 direction = transform.TransformDirection(localDirection.normalized);
        Vector2 endPoint = origin + direction * maxDistance;

        RaycastHit2D[] hits = Physics2D.CircleCastAll(
            origin,
            collisionWidth * 0.5f,
            direction,
            maxDistance,
            collisionMask
        );

        RaycastHit2D closestHit = default;
        bool hasHit = false;

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null || IsPartOfLaserObject(hit.collider.transform))
            {
                continue;
            }

            if (!triggersBlockBeam && hit.collider.isTrigger)
            {
                continue;
            }

            if (!hasHit || hit.distance < closestHit.distance)
            {
                closestHit = hit;
                hasHit = true;
            }
        }

        if (hasHit)
        {
            endPoint = closestHit.point;

            PlayerController player = closestHit.collider.GetComponentInParent<PlayerController>();
            if (player != null && GameManager.Instance != null)
            {
                GameManager.Instance.KillPlayer();
            }
        }

        UpdateBeamVisual(origin, endPoint, direction);
    }

    /// <summary>
    /// Checks whether a hit belongs to this laser object or one of its child visuals.
    /// </summary>
    /// <param name="hitTransform">The transform hit by the laser cast.</param>
    /// <returns>True when the hit should be ignored as part of this laser.</returns>
    private bool IsPartOfLaserObject(Transform hitTransform)
    {
        return hitTransform == transform ||
               hitTransform.IsChildOf(transform) ||
               transform.IsChildOf(hitTransform);
    }

    /// <summary>
    /// Scales, rotates, and positions the SpriteRenderer between the laser endpoints.
    /// </summary>
    /// <param name="origin">The world-space start point of the laser.</param>
    /// <param name="endPoint">The world-space end point of the laser.</param>
    /// <param name="direction">The normalized world-space laser direction.</param>
    private void UpdateBeamVisual(Vector2 origin, Vector2 endPoint, Vector2 direction)
    {
        if (spriteRenderer == null || spriteRenderer.sprite == null)
        {
            return;
        }

        float beamLength = Vector2.Distance(origin, endPoint);
        Sprite sprite = spriteRenderer.sprite;
        Vector2 spriteSize = sprite.bounds.size;
        float spritePixelHeight = sprite.rect.height;
        float bottomPadding = sprite.border.y;
        float topPadding = sprite.border.w;
        float visiblePixelHeight = spritePixelHeight - bottomPadding - topPadding;
        float displayedLength = beamLength;
        float centerOffset = 0f;

        if (visiblePixelHeight > 0f)
        {
            displayedLength = beamLength * spritePixelHeight / visiblePixelHeight;
            centerOffset = beamLength * (topPadding - bottomPadding) /
                           (2f * visiblePixelHeight);
        }

        spriteRenderer.transform.position =
            (origin + endPoint) * 0.5f + direction * centerOffset;
        spriteRenderer.transform.rotation = Quaternion.Euler(
            0f,
            0f,
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f
        );
        spriteRenderer.transform.localScale = new Vector3(
            visualWidth / spriteSize.x,
            displayedLength / spriteSize.y,
            1f
        );
    }

    /// <summary>
    /// Creates a child SpriteRenderer for the laser beam when one is not already present.
    /// </summary>
    private void CreateBeamVisualIfNeeded()
    {
        Transform existingVisual = transform.Find(BeamVisualName);
        if (existingVisual != null)
        {
            spriteRenderer = existingVisual.GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            return;
        }

        GameObject beamVisual = new GameObject(BeamVisualName);
        beamVisual.transform.SetParent(transform, false);
        spriteRenderer = beamVisual.AddComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Applies the configured sprite, color, and sorting order to the beam renderer.
    /// </summary>
    private void ConfigureSpriteRenderer()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        if (laserSprite != null)
        {
            spriteRenderer.sprite = laserSprite;
        }

        spriteRenderer.color = Color.white;
        spriteRenderer.sortingOrder = sortingOrder;
    }

    /// <summary>
    /// Disables any legacy LineRenderer so only the sprite beam is visible.
    /// </summary>
    private void DisableOldLineRenderer()
    {
        LineRenderer oldLineRenderer = GetComponent<LineRenderer>();
        if (oldLineRenderer != null)
        {
            oldLineRenderer.enabled = false;
        }
    }

    /// <summary>
    /// Draws the configured laser path while the object is selected in the editor.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Vector2 origin = firePoint != null ? firePoint.position : transform.position;
        Vector2 direction = transform.TransformDirection(localDirection.normalized);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin + direction * maxDistance);
    }
}
