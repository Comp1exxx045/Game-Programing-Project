using UnityEngine;

public class RedPanelInteraction : MonoBehaviour
{
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private GameObject[] objectsToActivate;
    [SerializeField] private GameObject[] objectsToDeactivate;
    [SerializeField] private bool deactivateThisPanel = true;

    private int playersInRange;
    private bool hasBeenUsed;

    // Keeps a used panel disabled when it is enabled again.
    void OnEnable()
    {
        if (hasBeenUsed && deactivateThisPanel)
        {
            gameObject.SetActive(false);
        }
    }

    // Handles the interaction input while a player is in range.
    void Update()
    {
        if (!hasBeenUsed &&
            playersInRange > 0 &&
            Input.GetKeyDown(interactionKey))
        {
            UsePanel();
        }
    }

    // Tracks players entering the panel interaction area.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerController>() != null)
        {
            playersInRange++;
        }
    }

    // Tracks players leaving the panel interaction area.
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerController>() != null)
        {
            playersInRange = Mathf.Max(0, playersInRange - 1);
        }
    }

    // Activates and deactivates the configured objects after interaction.
    private void UsePanel()
    {
        hasBeenUsed = true;

        foreach (GameObject target in objectsToActivate)
        {
            if (target != null)
            {
                target.SetActive(true);
            }
        }

        foreach (GameObject target in objectsToDeactivate)
        {
            if (target != null && target != gameObject)
            {
                target.SetActive(false);
            }
        }

        if (deactivateThisPanel)
        {
            gameObject.SetActive(false);
        }
    }
}
