using UnityEngine;

public class RedPanelInteraction : MonoBehaviour
{
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private GameObject[] objectsToActivate;
    [SerializeField] private GameObject[] objectsToDeactivate;
    [SerializeField] private bool deactivateThisPanel = true;

    private int playersInRange;
    private bool hasBeenUsed;

    /// <summary>
    /// Keeps a used panel disabled if its world is enabled again.
    /// </summary>
    private void OnEnable()
    {
        if (hasBeenUsed && deactivateThisPanel)
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Handles the interaction key while at least one player collider is in range.
    /// </summary>
    private void Update()
    {
        if (!hasBeenUsed &&
            playersInRange > 0 &&
            Input.GetKeyDown(interactionKey))
        {
            UsePanel();
        }
    }

    /// <summary>
    /// Tracks player colliders that enter the panel interaction area.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerController>() != null)
        {
            playersInRange++;
        }
    }

    /// <summary>
    /// Tracks player colliders that leave the panel interaction area.
    /// </summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerController>() != null)
        {
            playersInRange = Mathf.Max(0, playersInRange - 1);
        }
    }

    /// <summary>
    /// Applies all configured activation changes and consumes the panel interaction.
    /// </summary>
    private void UsePanel()
    {
        hasBeenUsed = true;

        if (objectsToActivate != null)
        {
            foreach (GameObject target in objectsToActivate)
            {
                if (target != null)
                {
                    target.SetActive(true);
                }
            }
        }

        if (objectsToDeactivate != null)
        {
            foreach (GameObject target in objectsToDeactivate)
            {
                if (target != null && target != gameObject)
                {
                    target.SetActive(false);
                }
            }
        }

        if (deactivateThisPanel)
        {
            gameObject.SetActive(false);
        }
    }
}
