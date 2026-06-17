using UnityEngine;

/// <summary>
/// Shows a UI hint once the player reaches a configured world-space X position.
/// </summary>
public class WorldPositionHintUI : MonoBehaviour
{
    [SerializeField] private GameObject hintRoot;
    [SerializeField] private Transform player;
    [SerializeField] private float showAtX = 33f;

    private bool hasShown;

    /// <summary>
    /// Hides the hint at startup and resolves the player reference.
    /// </summary>
    private void Awake()
    {
        if (hintRoot != null)
        {
            hintRoot.SetActive(false);
        }

        ResolvePlayer();
    }

    /// <summary>
    /// Shows the hint once after the player reaches the configured X threshold.
    /// </summary>
    private void Update()
    {
        if (hasShown)
        {
            return;
        }

        if (!ResolvePlayer())
        {
            return;
        }

        if (player.position.x >= showAtX)
        {
            hasShown = true;

            if (hintRoot != null)
            {
                hintRoot.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Uses the assigned player transform or finds the active scene's player controller.
    /// </summary>
    /// <returns>True when a player transform is available.</returns>
    private bool ResolvePlayer()
    {
        if (player == null)
        {
            PlayerController playerController = FindObjectOfType<PlayerController>(true);
            if (playerController != null)
            {
                player = playerController.transform;
            }
        }

        return player != null;
    }
}
