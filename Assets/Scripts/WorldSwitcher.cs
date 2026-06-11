using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldSwitcher : MonoBehaviour
{
    public GameObject[] worldAObjects;
    public GameObject[] worldBObjects;

    private bool isWorldA = true;

    void Awake()
    {
        FindWorldObjectsIfNeeded();
        ApplyWorld();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isWorldA = !isWorldA;
            ApplyWorld();
        }
    }

    void ApplyWorld()
    {
        foreach (var obj in worldAObjects)
        {
            if (obj != null)
            {
                obj.SetActive(isWorldA);
            }
        }

        foreach (var obj in worldBObjects)
        {
            if (obj != null)
            {
                obj.SetActive(!isWorldA);
            }
        }
    }

    private void FindWorldObjectsIfNeeded()
    {
        if (!HasAssignedObject(worldAObjects))
        {
            GameObject worldA = FindObjectInScene("WorldA");
            worldAObjects = worldA != null ? new[] { worldA } : new GameObject[0];
        }

        if (!HasAssignedObject(worldBObjects))
        {
            GameObject worldB = FindObjectInScene("WorldB");
            worldBObjects = worldB != null ? new[] { worldB } : new GameObject[0];
        }

        if (worldAObjects.Length == 0 || worldBObjects.Length == 0)
        {
            Debug.LogWarning(
                "WorldSwitcher could not find both WorldA and WorldB in the current scene.",
                this
            );
        }
    }

    private static bool HasAssignedObject(GameObject[] objects)
    {
        if (objects == null)
        {
            return false;
        }

        foreach (GameObject obj in objects)
        {
            if (obj != null)
            {
                return true;
            }
        }

        return false;
    }

    private static GameObject FindObjectInScene(string objectName)
    {
        Scene scene = SceneManager.GetActiveScene();

        foreach (GameObject rootObject in scene.GetRootGameObjects())
        {
            Transform match = FindChildByName(rootObject.transform, objectName);
            if (match != null)
            {
                return match.gameObject;
            }
        }

        return null;
    }

    private static Transform FindChildByName(Transform current, string objectName)
    {
        if (current.name == objectName)
        {
            return current;
        }

        foreach (Transform child in current)
        {
            Transform match = FindChildByName(child, objectName);
            if (match != null)
            {
                return match;
            }
        }

        return null;
    }
}
