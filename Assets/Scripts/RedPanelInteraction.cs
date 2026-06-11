using UnityEngine;

public class RedPanelInteraction : MonoBehaviour
{
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private GameObject[] objectsToDeactivate;
    [SerializeField] private bool deactivateThisPanel = true;

    private int playersInRange;
    private bool hasBeenUsed;

    void OnEnable()
    {
        if (hasBeenUsed && deactivateThisPanel)
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!hasBeenUsed &&
            playersInRange > 0 &&
            Input.GetKeyDown(interactionKey))
        {
            DeactivatePanel();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerController>() != null)
        {
            playersInRange++;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerController>() != null)
        {
            playersInRange = Mathf.Max(0, playersInRange - 1);
        }
    }

    private void DeactivatePanel()
    {
        hasBeenUsed = true;

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
