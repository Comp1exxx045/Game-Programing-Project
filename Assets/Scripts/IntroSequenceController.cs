using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private void OnValidate()
    {
        ApplyTextSettings();
    }

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

    private void ApplyTextSettings()
    {
        if (storyText != null)
        {
            storyText.fontSize = Mathf.Max(1f, fontSize);
        }
    }
}
