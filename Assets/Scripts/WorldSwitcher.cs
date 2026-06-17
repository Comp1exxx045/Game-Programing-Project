using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Switches between WorldA and WorldB object sets and plays feedback when the world changes.
/// </summary>
public class WorldSwitcher : MonoBehaviour
{
    public GameObject[] worldAObjects;
    public GameObject[] worldBObjects;

    private bool isWorldA = true;
    private PlayerController playerController;

    /// <summary>
    /// Resolves world references and applies the initial world state.
    /// </summary>
    private void Awake()
    {
        FindWorldObjectsIfNeeded();
        ResolvePlayerController();
        ApplyWorld();
    }

    /// <summary>
    /// Switches between worlds when the player presses Space.
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isWorldA = !isWorldA;
            ApplyWorld();
            PlayWorldSwitchSound();
        }
    }

    /// <summary>
    /// Applies visibility for all configured WorldA and WorldB objects.
    /// </summary>
    private void ApplyWorld()
    {
        foreach (GameObject obj in worldAObjects)
        {
            if (obj != null)
            {
                obj.SetActive(isWorldA);
            }
        }

        foreach (GameObject obj in worldBObjects)
        {
            if (obj != null)
            {
                obj.SetActive(!isWorldA);
            }
        }
    }

    /// <summary>
    /// Finds WorldA and WorldB scene objects when arrays are not assigned.
    /// </summary>
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

    /// <summary>
    /// Uses the active scene's player controller as the source for player switch audio.
    /// </summary>
    /// <returns>True when a player controller is available.</returns>
    private bool ResolvePlayerController()
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>(true);
        }

        return playerController != null;
    }

    /// <summary>
    /// Plays the player world-switch sound after the world state changes.
    /// </summary>
    private void PlayWorldSwitchSound()
    {
        if (ResolvePlayerController())
        {
            playerController.PlayWorldSwitchSound();
        }
    }

    /// <summary>
    /// Checks whether an object array contains at least one valid reference.
    /// </summary>
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

    /// <summary>
    /// Finds an object by name within every root of the active scene.
    /// </summary>
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

    /// <summary>
    /// Recursively searches a Transform hierarchy for the requested object name.
    /// </summary>
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
