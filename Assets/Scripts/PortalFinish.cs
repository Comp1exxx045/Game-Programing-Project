using UnityEngine;

/// <summary>
/// Connects the existing portal trigger to the level completion interface.
/// </summary>
public class PortalFinish : MonoBehaviour
{
    [SerializeField] private LevelCompletionUI levelCompletionUI;

    private bool hasTriggered;
    private bool missingPlayerTagReported;

    /// <summary>
    /// Resolves the level completion interface when the portal initializes.
    /// </summary>
    private void Awake()
    {
        if (levelCompletionUI == null)
        {
            levelCompletionUI = FindObjectOfType<LevelCompletionUI>(true);
        }
    }

    /// <summary>
    /// Opens the completion interface once when a player enters the portal.
    /// </summary>
    /// <param name="other">The collider that entered the portal trigger.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered || other == null)
        {
            return;
        }

        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player == null)
        {
            return;
        }

        if (!player.CompareTag("Player") && !missingPlayerTagReported)
        {
            Debug.LogWarning(
                $"PortalFinish detected PlayerController on '{player.name}', but the object is not tagged Player. Completion will continue using the component reference.",
                player);
            missingPlayerTagReported = true;
        }

        if (levelCompletionUI == null)
        {
            levelCompletionUI = FindObjectOfType<LevelCompletionUI>(true);
        }

        if (levelCompletionUI == null)
        {
            Debug.LogError(
                "PortalFinish cannot complete the level because no LevelCompletionUI was assigned or found.",
                this);
            return;
        }

        hasTriggered = true;
        levelCompletionUI.ShowLevelComplete();
    }
}
