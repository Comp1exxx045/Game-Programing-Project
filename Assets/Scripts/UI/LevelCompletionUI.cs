using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controls the level completion overlay, player input lock, and scene navigation.
/// </summary>
public class LevelCompletionUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject completionRoot;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Player")]
    [SerializeField] private PlayerController playerController;

    [Header("Scenes")]
    [SerializeField] private string nextSceneName;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isLevelComplete;
    private bool listenersRegistered;

    /// <summary>
    /// Gets whether the level completion flow has already been triggered.
    /// </summary>
    public bool IsLevelComplete => isLevelComplete;

    /// <summary>
    /// Restores normal time, hides the completion overlay, resolves dependencies,
    /// and registers the button callbacks when the scene initializes.
    /// </summary>
    private void Awake()
    {
        Time.timeScale = 1f;

        if (completionRoot != null)
        {
            completionRoot.SetActive(false);
        }

        ResolvePlayerController();
        RegisterButtonListeners();
        ValidateConfiguration();
    }

    /// <summary>
    /// Removes runtime button callbacks when this component is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        RemoveButtonListeners();
    }

    /// <summary>
    /// Opens the completion overlay once, disables player control, and pauses gameplay.
    /// </summary>
    public void ShowLevelComplete()
    {
        if (isLevelComplete)
        {
            return;
        }

        if (completionRoot == null)
        {
            Debug.LogError(
                "LevelCompletionUI cannot show the completion screen because Completion Root is not assigned.",
                this);
            return;
        }

        isLevelComplete = true;

        if (ResolvePlayerController())
        {
            playerController.SetControlEnabled(false);

            Rigidbody2D playerRigidbody = playerController.GetComponent<Rigidbody2D>();
            if (playerRigidbody != null)
            {
                playerRigidbody.velocity = Vector2.zero;
                playerRigidbody.angularVelocity = 0f;
            }
        }
        else
        {
            Debug.LogError(
                "LevelCompletionUI could not find a PlayerController. The completion screen will open, but player input could not be disabled.",
                this);
        }

        completionRoot.SetActive(true);
        SelectNextLevelButton();
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Loads the next scene configured in the Inspector.
    /// </summary>
    public void LoadNextLevel()
    {
        LoadConfiguredScene(nextSceneName, nameof(nextSceneName));
    }

    /// <summary>
    /// Reloads the currently active scene from its beginning.
    /// </summary>
    public void RestartLevel()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        LoadConfiguredScene(currentSceneName, "current scene");
    }

    /// <summary>
    /// Loads the main menu scene configured in the Inspector.
    /// </summary>
    public void ReturnToMainMenu()
    {
        LoadConfiguredScene(mainMenuSceneName, nameof(mainMenuSceneName));
    }

    /// <summary>
    /// Uses the assigned player controller or finds the active scene's controller.
    /// </summary>
    /// <returns>True when a player controller is available.</returns>
    private bool ResolvePlayerController()
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>(true);
        }

        return playerController != null;
    }

    /// <summary>
    /// Registers each completion button callback exactly once.
    /// </summary>
    private void RegisterButtonListeners()
    {
        if (listenersRegistered)
        {
            return;
        }

        RegisterButtonListener(nextLevelButton, LoadNextLevel, "Next Level Button");
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

        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.RemoveListener(LoadNextLevel);
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
                $"LevelCompletionUI cannot register {referenceName} because its reference is not assigned.",
                this);
            return;
        }

        button.onClick.RemoveListener(action);
        button.onClick.AddListener(action);
    }

    /// <summary>
    /// Selects the next-level button for keyboard or controller navigation.
    /// </summary>
    private void SelectNextLevelButton()
    {
        if (EventSystem.current == null || nextLevelButton == null)
        {
            return;
        }

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(nextLevelButton.gameObject);
    }

    /// <summary>
    /// Validates and loads a scene after restoring the normal game time scale.
    /// </summary>
    /// <param name="sceneName">The scene name to load.</param>
    /// <param name="fieldName">The configuration field represented by the scene name.</param>
    private void LoadConfiguredScene(string sceneName, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError(
                $"LevelCompletionUI cannot load a scene because {fieldName} is empty.",
                this);
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError(
                $"LevelCompletionUI cannot load scene '{sceneName}'. Add it to Scenes In Build or Build Profiles and verify the configured name.",
                this);
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Reports missing UI references and scene names with actionable error messages.
    /// </summary>
    private void ValidateConfiguration()
    {
        if (completionRoot == null)
        {
            Debug.LogError("LevelCompletionUI is missing its Completion Root reference.", this);
        }

        if (nextLevelButton == null)
        {
            Debug.LogError("LevelCompletionUI is missing its Next Level Button reference.", this);
        }

        if (restartButton == null)
        {
            Debug.LogError("LevelCompletionUI is missing its Restart Button reference.", this);
        }

        if (mainMenuButton == null)
        {
            Debug.LogError("LevelCompletionUI is missing its Main Menu Button reference.", this);
        }

        if (string.IsNullOrWhiteSpace(nextSceneName))
        {
            Debug.LogError("LevelCompletionUI nextSceneName is empty.", this);
        }

        if (string.IsNullOrWhiteSpace(mainMenuSceneName))
        {
            Debug.LogError("LevelCompletionUI mainMenuSceneName is empty.", this);
        }
    }
}
