using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles scene navigation and application exit actions for the main menu.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    private const string FirstLevelScene = "Level01";

    [SerializeField] private AudioSource buttonClickAudioSource = null;
    [SerializeField] private AudioClip buttonClickSound = null;
    [SerializeField] private float buttonClickDelay = 0.12f;

    private bool isNavigating;

    /// <summary>
    /// Plays the button click sound before loading the first playable level.
    /// </summary>
    public void StartGame()
    {
        if (isNavigating)
        {
            return;
        }

        StartCoroutine(StartGameAfterClick());
    }

    /// <summary>
    /// Plays the button click sound before stopping Play Mode or exiting the application.
    /// </summary>
    public void QuitGame()
    {
        if (isNavigating)
        {
            return;
        }

        StartCoroutine(QuitGameAfterClick());
    }

    /// <summary>
    /// Waits briefly for the click sound, then loads the first playable level.
    /// </summary>
    private IEnumerator StartGameAfterClick()
    {
        isNavigating = true;
        PlayButtonClickSound();

        yield return new WaitForSecondsRealtime(GetButtonClickDelay());

        SceneManager.LoadScene(FirstLevelScene);
    }

    /// <summary>
    /// Waits briefly for the click sound, then quits the current runtime.
    /// </summary>
    private IEnumerator QuitGameAfterClick()
    {
        isNavigating = true;
        PlayButtonClickSound();

        yield return new WaitForSecondsRealtime(GetButtonClickDelay());

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// Plays the configured menu button click sound once.
    /// </summary>
    private void PlayButtonClickSound()
    {
        if (buttonClickAudioSource == null || buttonClickSound == null)
        {
            return;
        }

        buttonClickAudioSource.PlayOneShot(buttonClickSound);
    }

    /// <summary>
    /// Returns a small realtime delay that lets the click sound be heard.
    /// </summary>
    private float GetButtonClickDelay()
    {
        if (buttonClickSound == null)
        {
            return 0f;
        }

        return Mathf.Max(0f, Mathf.Min(buttonClickDelay, buttonClickSound.length));
    }
}
