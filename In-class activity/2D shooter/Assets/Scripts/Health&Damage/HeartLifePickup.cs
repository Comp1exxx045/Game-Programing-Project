using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A pickup that gives the player one extra life.
/// </summary>
public class HeartLifePickup : MonoBehaviour
{
    private const int PickupSize = 32;
    private static bool sceneLoadHookRegistered = false;
    private static Sprite heartSprite;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void RegisterLevelSpawner()
    {
        if (sceneLoadHookRegistered)
        {
            return;
        }

        sceneLoadHookRegistered = true;
        SceneManager.sceneLoaded += (_, __) => CreateSpawnerForActiveLevel();
        CreateSpawnerForActiveLevel();
    }

    private static void CreateSpawnerForActiveLevel()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if ((sceneName != "Level1" && sceneName != "Level2") || FindObjectOfType<HeartLifePickupSpawner>() != null)
        {
            return;
        }

        GameObject spawnerObject = new GameObject("Heart Life Pickup Spawner");
        spawnerObject.AddComponent<HeartLifePickupSpawner>();
    }

    public static GameObject CreatePickup(Vector3 position)
    {
        GameObject pickupObject = new GameObject("Heart Life Pickup");
        pickupObject.transform.position = position;

        SpriteRenderer spriteRenderer = pickupObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = GetHeartSprite();
        spriteRenderer.sortingOrder = 20;

        CircleCollider2D collider = pickupObject.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.45f;

        pickupObject.AddComponent<HeartLifePickup>();
        return pickupObject;
    }

    private static Sprite GetHeartSprite()
    {
        if (heartSprite != null)
        {
            return heartSprite;
        }

        Texture2D texture = new Texture2D(PickupSize, PickupSize, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;

        Color clear = new Color(0, 0, 0, 0);
        Color heartColor = new Color(1f, 0.08f, 0.25f, 1f);
        Color shineColor = new Color(1f, 0.55f, 0.65f, 1f);

        for (int y = 0; y < PickupSize; y++)
        {
            for (int x = 0; x < PickupSize; x++)
            {
                float normalizedX = (x - PickupSize * 0.5f) / (PickupSize * 0.5f);
                float normalizedY = (y - PickupSize * 0.45f) / (PickupSize * 0.5f);
                float heartFormula = Mathf.Pow(normalizedX * normalizedX + normalizedY * normalizedY - 0.32f, 3)
                    - normalizedX * normalizedX * normalizedY * normalizedY * normalizedY;

                if (heartFormula <= 0f)
                {
                    bool isShine = x > 9 && x < 14 && y > 20 && y < 25;
                    texture.SetPixel(x, y, isShine ? shineColor : heartColor);
                }
                else
                {
                    texture.SetPixel(x, y, clear);
                }
            }
        }

        texture.Apply();
        heartSprite = Sprite.Create(texture, new Rect(0, 0, PickupSize, PickupSize), new Vector2(0.5f, 0.5f), PickupSize);
        return heartSprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Health playerHealth = GetPlayerHealth(collision.gameObject);
        if (playerHealth == null)
        {
            return;
        }

        playerHealth.currentLives = Mathf.Min(playerHealth.currentLives + 1, playerHealth.maximumLives);
        GameManager.UpdateUIElements();
        Destroy(gameObject);
    }

    private Health GetPlayerHealth(GameObject collisionGameObject)
    {
        if (GameManager.instance != null && GameManager.instance.player != null && collisionGameObject != GameManager.instance.player)
        {
            return null;
        }

        if (collisionGameObject.GetComponent<Controller>() == null)
        {
            return null;
        }

        return collisionGameObject.GetComponent<Health>();
    }
}
