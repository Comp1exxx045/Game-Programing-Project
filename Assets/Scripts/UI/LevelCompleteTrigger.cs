using UnityEngine;

/// <summary>
/// Detects a tagged player entering a level exit and opens the completion UI once.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class LevelCompleteTrigger : MonoBehaviour
{
    [SerializeField] private LevelCompletionUI levelCompletionUI;

    private bool hasTriggered;
    private bool missingPlayerTagReported;

    /// <summary>
    /// Configures the collider as a trigger and resolves the completion UI at runtime.
    /// </summary>
    private void Awake()
    {
        EnsureTriggerCollider();
        ResolveCompletionUI();
    }

    /// <summary>
    /// Configures the required collider when the component is first added.
    /// </summary>
    private void Reset()
    {
        EnsureTriggerCollider();
    }

    /// <summary>
    /// Keeps the required collider in trigger mode after Inspector changes.
    /// </summary>
    private void OnValidate()
    {
        EnsureTriggerCollider();
    }

    /// <summary>
    /// Completes the level once when a correctly tagged player enters the trigger.
    /// </summary>
    /// <param name="other">The collider that entered the level completion trigger.</param>
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

        if (!player.CompareTag("Player"))
        {
            if (!missingPlayerTagReported)
            {
                Debug.LogError(
                    $"LevelCompleteTrigger detected PlayerController on '{player.name}', but the object is not tagged Player.",
                    player);
                missingPlayerTagReported = true;
            }

            return;
        }

        if (!ResolveCompletionUI())
        {
            Debug.LogError(
                "LevelCompleteTrigger cannot complete the level because no LevelCompletionUI was assigned or found.",
                this);
            return;
        }

        hasTriggered = true;
        levelCompletionUI.ShowLevelComplete();
    }

    /// <summary>
    /// Ensures the attached 2D collider is configured as a trigger.
    /// </summary>
    private void EnsureTriggerCollider()
    {
        Collider2D triggerCollider = GetComponent<Collider2D>();
        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true;
        }
    }

    /// <summary>
    /// Uses the assigned completion UI or finds one in the loaded scene.
    /// </summary>
    /// <returns>True when a completion UI is available.</returns>
    private bool ResolveCompletionUI()
    {
        if (levelCompletionUI == null)
        {
            levelCompletionUI = FindObjectOfType<LevelCompletionUI>(true);
        }

        return levelCompletionUI != null;
    }
}
