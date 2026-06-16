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

    private bool isPaused;
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

        RegisterButtonListener(resumeButton, ResumeGame, "Resume Button");
        RegisterButtonListener(restartButton, RestartLevel, "Restart Button");
        RegisterButtonListener(mainMenuButton, ReturnToMainMenu, "Main Menu Button");
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
            resumeButton.onClick.RemoveListener(ResumeGame);
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(RestartLevel);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveListener(ReturnToMainMenu);
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

        isPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
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
