using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Handles red panel interaction, configured object activation, audio feedback, and optional scene transition.
/// </summary>
public class RedPanelInteraction : MonoBehaviour
{
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private GameObject[] objectsToActivate;
    [SerializeField] private GameObject[] objectsToDeactivate;
    [SerializeField] private bool deactivateThisPanel = true;
    [SerializeField] private AudioClip confirmationSound;
    [SerializeField] private float confirmationVolume = 1f;
    [SerializeField] private AudioClip followUpSound;
    [SerializeField] private float followUpVolume = 1f;
    [SerializeField] private Image fadeImage;
    [SerializeField, Range(0f, 1f)] private float fadeTargetAlpha = 1f;
    [SerializeField] private string sceneToLoadAfterSequence;

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
        if (followUpSound == null)
        {
            source.volume = confirmationVolume;
            source.PlayOneShot(confirmationSound);
            Destroy(soundObject, confirmationSound.length);
            return;
        }

        PanelSoundSequence sequence = soundObject.AddComponent<PanelSoundSequence>();
        sequence.Play(
            source,
            confirmationSound,
            confirmationVolume,
            followUpSound,
            followUpVolume,
            fadeImage,
            fadeTargetAlpha,
            sceneToLoadAfterSequence);
    }

    /// <summary>
    /// Plays a two-step panel audio sequence and coordinates the optional screen fade.
    /// </summary>
    private sealed class PanelSoundSequence : MonoBehaviour
    {
        /// <summary>
        /// Starts the panel sound sequence using the configured clips and fade settings.
        /// </summary>
        /// <param name="source">The temporary audio source used for playback.</param>
        /// <param name="firstClip">The first clip played immediately.</param>
        /// <param name="firstVolume">The playback volume for the first clip.</param>
        /// <param name="secondClip">The follow-up clip played after the first clip ends.</param>
        /// <param name="secondVolume">The playback volume for the follow-up clip.</param>
        /// <param name="fadeImage">The optional image to fade while the follow-up clip plays.</param>
        /// <param name="fadeTargetAlpha">The target alpha for the fade image.</param>
        /// <param name="sceneToLoadAfterSequence">The optional scene loaded after playback finishes.</param>
        public void Play(
            AudioSource source,
            AudioClip firstClip,
            float firstVolume,
            AudioClip secondClip,
            float secondVolume,
            Image fadeImage,
            float fadeTargetAlpha,
            string sceneToLoadAfterSequence)
        {
            StartCoroutine(PlaySequence(
                source,
                firstClip,
                firstVolume,
                secondClip,
                secondVolume,
                fadeImage,
                fadeTargetAlpha,
                sceneToLoadAfterSequence));
        }

        /// <summary>
        /// Plays the first clip, then the follow-up clip, fade, and optional scene load in order.
        /// </summary>
        /// <param name="source">The temporary audio source used for playback.</param>
        /// <param name="firstClip">The first clip played immediately.</param>
        /// <param name="firstVolume">The playback volume for the first clip.</param>
        /// <param name="secondClip">The follow-up clip played after the first clip ends.</param>
        /// <param name="secondVolume">The playback volume for the follow-up clip.</param>
        /// <param name="fadeImage">The optional image to fade while the follow-up clip plays.</param>
        /// <param name="fadeTargetAlpha">The target alpha for the fade image.</param>
        /// <param name="sceneToLoadAfterSequence">The optional scene loaded after playback finishes.</param>
        private IEnumerator PlaySequence(
            AudioSource source,
            AudioClip firstClip,
            float firstVolume,
            AudioClip secondClip,
            float secondVolume,
            Image fadeImage,
            float fadeTargetAlpha,
            string sceneToLoadAfterSequence)
        {
            source.volume = firstVolume;
            source.clip = firstClip;
            source.Play();

            yield return new WaitForSeconds(firstClip.length);

            source.volume = secondVolume;
            source.clip = secondClip;
            source.Play();

            yield return FadeWhileClipPlays(fadeImage, fadeTargetAlpha, secondClip.length);

            LoadSceneIfConfigured(sceneToLoadAfterSequence);

            Destroy(gameObject);
        }

        /// <summary>
        /// Loads the configured scene when a scene name is provided and available.
        /// </summary>
        /// <param name="sceneName">The scene name to load after the panel sequence.</param>
        private void LoadSceneIfConfigured(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                return;
            }

            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                Debug.LogError(
                    $"RedPanelInteraction cannot load scene '{sceneName}'. Add it to Scenes In Build or Build Profiles and verify the configured name.",
                    this);
                return;
            }

            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// Fades an image to the requested alpha over the duration of the follow-up clip.
        /// </summary>
        /// <param name="fadeImage">The image to fade, or null to wait without fading.</param>
        /// <param name="targetAlpha">The alpha value to reach by the end of the fade.</param>
        /// <param name="duration">The fade duration in seconds.</param>
        private static IEnumerator FadeWhileClipPlays(
            Image fadeImage,
            float targetAlpha,
            float duration)
        {
            if (fadeImage == null)
            {
                yield return new WaitForSeconds(duration);
                yield break;
            }

            Color startColor = fadeImage.color;
            Color targetColor = startColor;
            targetColor.a = Mathf.Clamp01(targetAlpha);

            if (duration <= 0f)
            {
                fadeImage.color = targetColor;
                yield break;
            }

            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                fadeImage.color = Color.Lerp(startColor, targetColor, Mathf.Clamp01(elapsed / duration));
                yield return null;
            }

            fadeImage.color = targetColor;
        }
    }
}
