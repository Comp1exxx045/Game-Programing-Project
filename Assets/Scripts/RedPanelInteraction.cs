using UnityEngine;

public class RedPanelInteraction : MonoBehaviour
{
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private GameObject[] objectsToActivate;
    [SerializeField] private GameObject[] objectsToDeactivate;
    [SerializeField] private bool deactivateThisPanel = true;
    [SerializeField] private AudioClip confirmationSound;
    [SerializeField] private float confirmationVolume = 1f;

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
        PlayConfirmationSound();

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

    /// <summary>
    /// Plays the confirmation sound from a temporary source so deactivating the panel does not stop it.
    /// </summary>
    private void PlayConfirmationSound()
    {
        if (confirmationSound == null)
        {
            return;
        }

        GameObject soundObject = new GameObject("Panel Confirmation Sound");
        soundObject.transform.position = transform.position;

        AudioSource source = soundObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.volume = confirmationVolume;
        source.PlayOneShot(confirmationSound);

        Destroy(soundObject, confirmationSound.length);
    }
}
