using UnityEngine;

public class HeartLifePickupSpawner : MonoBehaviour
{
    [Tooltip("Seconds after level start before the heart appears")]
    public float spawnDelay = 5f;

    [Tooltip("Distance from the edge of the camera view")]
    public float edgePadding = 1.25f;

    private void Start()
    {
        Invoke(nameof(SpawnHeart), spawnDelay);
    }

    private void SpawnHeart()
    {
        HeartLifePickup.CreatePickup(GetRandomSpawnPosition());
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Camera camera = Camera.main;
        if (camera == null)
        {
            camera = FindObjectOfType<Camera>();
        }

        if (camera == null || !camera.orthographic)
        {
            return Vector3.zero;
        }

        float height = camera.orthographicSize;
        float width = height * camera.aspect;
        Vector3 center = camera.transform.position;

        float x = Random.Range(center.x - width + edgePadding, center.x + width - edgePadding);
        float y = Random.Range(center.y - height + edgePadding, center.y + height - edgePadding);

        return new Vector3(x, y, 0f);
    }
}
