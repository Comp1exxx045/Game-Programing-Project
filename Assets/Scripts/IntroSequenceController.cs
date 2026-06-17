using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Plays the opening story sequence and loads the configured scene when it finishes.
/// </summary>
public class IntroSequenceController : MonoBehaviour
{
    [SerializeField] private TMP_Text storyText;
    [SerializeField] private CanvasGroup textCanvasGroup;
    [SerializeField]
    private string[] messages =
    {
        "THE FACILITY WAS BUILT TO HARVEST\nENERGY FROM THE RIFT.",
        "EXTRACTION CONTINUED\nBEYOND SAFE LIMITS.",
        "CONTAINMENT HAS FAILED.",
        "EXECUTE THE RIFT PROTOCOL.",
        "SHUT DOWN THE FACILITY."
    };
    [SerializeField, Min(1f)] private float fontSize = 48f;
    [SerializeField] private float fadeDuration = 0.8f;
    [SerializeField] private float displayDuration = 2.5f;
    [SerializeField] private float intervalDuration = 0.3f;
    [SerializeField] private bool allowSkip = true;
    [SerializeField] private string nextSceneName = "Level01";

    private bool hasStarted;
    private bool isDisplayingMessage;
    private bool skipCurrentDisplay;

    /// <summary>
    /// Validates references and starts the intro sequence once.
    /// </summary>
    private void Start()
    {
        if (hasStarted)
        {
            return;
        }

        if (!ValidateConfiguration())
        {
            return;
        }

        hasStarted = true;
        ApplyTextSettings();
        textCanvasGroup.alpha = 0f;
        StartCoroutine(PlaySequence());
    }

    /// <summary>
    /// Applies editable text settings while values change in the Inspector.
    /// </summary>
    private void OnValidate()
    {
        ApplyTextSettings();
    }

    /// <summary>
    /// Handles skip input for the currently displayed message.
    /// </summary>
    private void Update()
    {
        if (!allowSkip || !isDisplayingMessage || skipCurrentDisplay)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            skipCurrentDisplay = true;
        }
    }

    /// <summary>
    /// Displays each configured message with fade-in, hold, fade-out, and interval timing.
    /// </summary>
    private IEnumerator PlaySequence()
    {
        foreach (string message in messages)
        {
            storyText.text = message;
            skipCurrentDisplay = false;

            yield return FadeText(0f, 1f);
            yield return DisplayCurrentMessage();
            yield return FadeText(1f, 0f);
            yield return new WaitForSecondsRealtime(Mathf.Max(0f, intervalDuration));
        }

        LoadNextScene();
    }

    /// <summary>
    /// Holds the current message on screen until its duration ends or skip is pressed.
    /// </summary>
    private IEnumerator DisplayCurrentMessage()
    {
        isDisplayingMessage = true;

        float elapsed = 0f;
        float duration = Mathf.Max(0f, displayDuration);

        while (elapsed < duration && !skipCurrentDisplay)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        isDisplayingMessage = false;
    }

    /// <summary>
    /// Smoothly fades the story text between two alpha values using unscaled time.
    /// </summary>
    /// <param name="fromAlpha">The starting text alpha.</param>
    /// <param name="toAlpha">The target text alpha.</param>
    private IEnumerator FadeText(float fromAlpha, float toAlpha)
    {
        float duration = Mathf.Max(0f, fadeDuration);

        if (duration <= 0f)
        {
            textCanvasGroup.alpha = toAlpha;
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            textCanvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, easedProgress);
            yield return null;
        }

        textCanvasGroup.alpha = toAlpha;
    }

    /// <summary>
    /// Loads the configured scene after validating that it is included in the build.
    /// </summary>
    private void LoadNextScene()
    {
        if (string.IsNullOrWhiteSpace(nextSceneName))
        {
            Debug.LogError("IntroSequenceController cannot load the next scene because Next Scene Name is empty.", this);
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            Debug.LogError(
                $"IntroSequenceController cannot load scene '{nextSceneName}'. Add it to Scenes In Build or Build Profiles and verify the configured name.",
                this);
            return;
        }

        SceneManager.LoadScene(nextSceneName);
    }

    /// <summary>
    /// Checks required references and message data before playback starts.
    /// </summary>
    /// <returns>True when the intro can be played safely.</returns>
    private bool ValidateConfiguration()
    {
        bool isValid = true;

        if (storyText == null)
        {
            Debug.LogError("IntroSequenceController is missing its Story Text reference.", this);
            isValid = false;
        }

        if (textCanvasGroup == null)
        {
            Debug.LogError("IntroSequenceController is missing its Text Canvas Group reference.", this);
            isValid = false;
        }

        if (messages == null || messages.Length == 0)
        {
            Debug.LogError("IntroSequenceController has no story messages configured.", this);
            isValid = false;
        }

        return isValid;
    }

    /// <summary>
    /// Applies the Inspector-controlled font size to the TextMeshPro component.
    /// </summary>
    private void ApplyTextSettings()
    {
        if (storyText != null)
        {
            storyText.fontSize = Mathf.Max(1f, fontSize);
        }
    }
}
