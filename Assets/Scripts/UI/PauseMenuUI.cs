using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controls the reusable pause overlay and pause-related scene navigation.
/// </summary>
public class PauseMenuUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pauseRoot;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Level State")]
    [SerializeField] private LevelCompletionUI levelCompletionUI;
    [SerializeField] private ControlsGuideUI controlsGuideUI;

    [Header("Scenes")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Audio")]
    [SerializeField] private AudioSource buttonClickAudioSource = null;
    [SerializeField] private AudioClip buttonClickSound = null;
    [SerializeField] private float buttonClickDelay = 0.12f;

    private bool isPaused;
    private bool isLoadingScene;
    private bool listenersRegistered;

    /// <summary>
    /// Restores normal time, hides the pause overlay, and validates scene references.
    /// </summary>
    private void Awake()
    {
        Time.timeScale = 1f;

        if (pauseRoot != null)
        {
            pauseRoot.SetActive(false);
        }

        ResolveLevelCompletionUI();
        ResolveControlsGuideUI();
        ResolveButtonClickAudioSource();
        ValidateConfiguration();
    }

    /// <summary>
    /// Registers runtime button callbacks when the always-active canvas is enabled.
    /// </summary>
    private void OnEnable()
    {
        RegisterButtonListeners();
    }

    /// <summary>
    /// Removes runtime button callbacks when the canvas is disabled or destroyed.
    /// </summary>
    private void OnDisable()
    {
        RemoveButtonListeners();
    }

    /// <summary>
    /// Toggles the pause menu when the player presses Escape.
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    /// <summary>
    /// Opens the pause overlay unless the game is already paused or the level is complete.
    /// </summary>
    public void PauseGame()
    {
        if (isPaused || IsLevelComplete() || IsControlsGuideOpen())
        {
            return;
        }

        if (pauseRoot == null)
        {
            Debug.LogError(
                "PauseMenuUI cannot open because Pause Root is not assigned.",
                this);
            return;
        }

        isPaused = true;
        pauseRoot.SetActive(true);
        Time.timeScale = 0f;
        SelectResumeButton();
    }

    /// <summary>
    /// Closes the pause overlay and restores normal gameplay time.
    /// </summary>
    public void ResumeGame()
    {
        if (!isPaused)
        {
            return;
        }

        isPaused = false;

        if (pauseRoot != null)
        {
            pauseRoot.SetActive(false);
        }

        ClearSelectedButton();
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Opens or closes the pause overlay according to the current pause state.
    /// </summary>
    public void TogglePause()
    {
        if (IsControlsGuideOpen())
        {
            return;
        }

        if (isPaused)
        {
            ResumeGame();
            return;
        }

        PauseGame();
    }

    /// <summary>
    /// Restores normal time and reloads the active scene for a clean level restart.
    /// </summary>
    public void RestartLevel()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        LoadScene(currentSceneName, "current scene");
    }

    /// <summary>
    /// Restores normal time and loads the configured main menu scene.
    /// </summary>
    public void ReturnToMainMenu()
    {
        LoadScene(mainMenuSceneName, nameof(mainMenuSceneName));
    }

    /// <summary>
    /// Uses the assigned completion UI or finds the active scene's completion UI.
    /// </summary>
    /// <returns>True when a completion UI is available.</returns>
    private bool ResolveLevelCompletionUI()
    {
        if (levelCompletionUI == null)
        {
            levelCompletionUI = FindObjectOfType<LevelCompletionUI>(true);
        }

        return levelCompletionUI != null;
    }

    /// <summary>
    /// Uses the assigned controls guide or finds the active scene's controls guide.
    /// </summary>
    /// <returns>True when a controls guide is available.</returns>
    private bool ResolveControlsGuideUI()
    {
        if (controlsGuideUI == null)
        {
            controlsGuideUI = FindObjectOfType<ControlsGuideUI>(true);
        }

        return controlsGuideUI != null;
    }

    /// <summary>
    /// Checks whether the level completion flow has already started.
    /// </summary>
    /// <returns>True when the completion screen has been triggered.</returns>
    private bool IsLevelComplete()
    {
        return ResolveLevelCompletionUI() && levelCompletionUI.IsLevelComplete;
    }

    /// <summary>
    /// Checks whether the controls guide is currently blocking gameplay.
    /// </summary>
    /// <returns>True when the controls guide is open.</returns>
    private bool IsControlsGuideOpen()
    {
        return ResolveControlsGuideUI() && controlsGuideUI.IsOpen;
    }

    /// <summary>
    /// Registers each pause menu button callback exactly once.
    /// </summary>
    private void RegisterButtonListeners()
    {
        if (listenersRegistered)
        {
            return;
        }

        RegisterButtonListener(resumeButton, OnResumeButtonClicked, "Resume Button");
        RegisterButtonListener(restartButton, OnRestartButtonClicked, "Restart Button");
        RegisterButtonListener(mainMenuButton, OnMainMenuButtonClicked, "Main Menu Button");
        listenersRegistered = true;
    }

    /// <summary>
    /// Removes callbacks previously registered by this component.
    /// </summary>
    private void RemoveButtonListeners()
    {
        if (!listenersRegistered)
        {
            return;
        }

        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveListener(OnResumeButtonClicked);
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(OnRestartButtonClicked);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);
        }

        listenersRegistered = false;
    }

    /// <summary>
    /// Replaces this component's matching runtime callback on a button.
    /// </summary>
    /// <param name="button">The button that should invoke the callback.</param>
    /// <param name="action">The callback to register.</param>
    /// <param name="referenceName">A readable reference name used in error messages.</param>
    private void RegisterButtonListener(
        Button button,
        UnityEngine.Events.UnityAction action,
        string referenceName)
    {
        if (button == null)
        {
            Debug.LogError(
                $"PauseMenuUI cannot register {referenceName} because its reference is not assigned.",
                this);
            return;
        }

        button.onClick.RemoveListener(action);
        button.onClick.AddListener(action);
    }

    /// <summary>
    /// Plays the click sound and resumes gameplay from the pause menu.
    /// </summary>
    private void OnResumeButtonClicked()
    {
        PlayButtonClickSound();
        ResumeGame();
    }

    /// <summary>
    /// Plays the click sound and starts the current level restart flow.
    /// </summary>
    private void OnRestartButtonClicked()
    {
        RestartLevel();
    }

    /// <summary>
    /// Plays the click sound and starts the main menu return flow.
    /// </summary>
    private void OnMainMenuButtonClicked()
    {
        ReturnToMainMenu();
    }

    /// <summary>
    /// Selects the resume button for keyboard or controller navigation.
    /// </summary>
    private void SelectResumeButton()
    {
        if (EventSystem.current == null || resumeButton == null)
        {
            return;
        }

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
    }

    /// <summary>
    /// Clears the selected pause menu object when the overlay closes.
    /// </summary>
    private void ClearSelectedButton()
    {
        if (EventSystem.current != null &&
            EventSystem.current.currentSelectedGameObject != null &&
            pauseRoot != null &&
            EventSystem.current.currentSelectedGameObject.transform.IsChildOf(pauseRoot.transform))
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    /// <summary>
    /// Validates and loads a scene after restoring the normal time scale.
    /// </summary>
    /// <param name="sceneName">The scene name to load.</param>
    /// <param name="fieldName">The configuration field represented by the scene name.</param>
    private void LoadScene(string sceneName, string fieldName)
    {
        if (isLoadingScene)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError(
                $"PauseMenuUI cannot load a scene because {fieldName} is empty.",
                this);
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError(
                $"PauseMenuUI cannot load scene '{sceneName}'. Add it to Scenes In Build or Build Profiles and verify the configured name.",
                this);
            return;
        }

        StartCoroutine(LoadSceneAfterButtonClick(sceneName));
    }

    /// <summary>
    /// Plays the button click sound, restores time, and loads the requested scene.
    /// </summary>
    /// <param name="sceneName">The scene name to load.</param>
    private IEnumerator LoadSceneAfterButtonClick(string sceneName)
    {
        isLoadingScene = true;
        PlayButtonClickSound();

        yield return new WaitForSecondsRealtime(GetButtonClickDelay());

        isPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Uses the assigned click audio source or finds one on the same canvas.
    /// </summary>
    private void ResolveButtonClickAudioSource()
    {
        if (buttonClickAudioSource == null)
        {
            buttonClickAudioSource = GetComponent<AudioSource>();
        }
    }

    /// <summary>
    /// Plays the configured pause menu button click sound once.
    /// </summary>
    private void PlayButtonClickSound()
    {
        ResolveButtonClickAudioSource();

        if (buttonClickAudioSource == null || buttonClickSound == null)
        {
            return;
        }

        buttonClickAudioSource.PlayOneShot(buttonClickSound);
    }

    /// <summary>
    /// Returns the realtime delay used to let click sounds finish before scene loading.
    /// </summary>
    private float GetButtonClickDelay()
    {
        if (buttonClickSound == null)
        {
            return 0f;
        }

        return Mathf.Max(0f, Mathf.Min(buttonClickDelay, buttonClickSound.length));
    }

    /// <summary>
    /// Reports missing UI and scene references with actionable error messages.
    /// </summary>
    private void ValidateConfiguration()
    {
        if (pauseRoot == null)
        {
            Debug.LogError("PauseMenuUI is missing its Pause Root reference.", this);
        }

        if (resumeButton == null)
        {
            Debug.LogError("PauseMenuUI is missing its Resume Button reference.", this);
        }

        if (restartButton == null)
        {
            Debug.LogError("PauseMenuUI is missing its Restart Button reference.", this);
        }

        if (mainMenuButton == null)
        {
            Debug.LogError("PauseMenuUI is missing its Main Menu Button reference.", this);
        }

        if (string.IsNullOrWhiteSpace(mainMenuSceneName))
        {
            Debug.LogError("PauseMenuUI mainMenuSceneName is empty.", this);
        }
    }
}
