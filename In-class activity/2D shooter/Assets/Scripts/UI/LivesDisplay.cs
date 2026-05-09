using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// This class inherits from the UIelement class and handles updating the lives display
/// </summary>
public class LivesDisplay : UIelement
{
    [Tooltip("The text UI to use for display")]
    public TextMeshProUGUI displayText = null;

    private static bool sceneLoadHookRegistered = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void RegisterSceneLoadHook()
    {
        if (sceneLoadHookRegistered)
        {
            return;
        }

        sceneLoadHookRegistered = true;
        SceneManager.sceneLoaded += (_, __) => EnsureLivesDisplayExists();
        EnsureLivesDisplayExists();
    }

    private static void EnsureLivesDisplayExists()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            return;
        }

        LivesDisplay livesDisplay = FindObjectOfType<LivesDisplay>(true);
        if (livesDisplay == null)
        {
            TextMeshProUGUI livesText = FindLivesTextInActiveScene();
            if (livesText == null)
            {
                return;
            }

            livesDisplay = livesText.GetComponent<LivesDisplay>();
            if (livesDisplay == null)
            {
                livesDisplay = livesText.gameObject.AddComponent<LivesDisplay>();
            }
        }

        livesDisplay.SetUpDisplayText();
        livesDisplay.DisplayLives();
    }

    private static TextMeshProUGUI FindLivesTextInActiveScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        TextMeshProUGUI[] texts = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
        TextMeshProUGUI fallbackText = null;

        foreach (TextMeshProUGUI text in texts)
        {
            if (text == null || text.gameObject.scene != activeScene)
            {
                continue;
            }

            if (text.gameObject.name == "Lives")
            {
                return text;
            }

            if (fallbackText == null && text.text.StartsWith("Lives"))
            {
                fallbackText = text;
            }
        }

        return fallbackText;
    }

    private void Awake()
    {
        SetUpDisplayText();
    }

    private void Start()
    {
        DisplayLives();
    }

    private void OnEnable()
    {
        DisplayLives();
    }

    private void SetUpDisplayText()
    {
        if (displayText == null)
        {
            displayText = GetComponent<TextMeshProUGUI>();
        }
    }

    /// <summary>
    /// Description:
    /// Updates the lives display
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    public void DisplayLives()
    {
        SetUpDisplayText();
        Health playerHealth = GetPlayerHealth();
        if (displayText != null && playerHealth != null)
        {
            displayText.text = "Lives: " + playerHealth.currentLives.ToString();
        }
    }

    private Health GetPlayerHealth()
    {
        if (GameManager.instance != null && GameManager.instance.player != null)
        {
            return GameManager.instance.player.GetComponent<Health>();
        }

        Controller playerController = FindObjectOfType<Controller>();
        if (playerController != null)
        {
            return playerController.GetComponent<Health>();
        }

        return null;
    }

    /// <summary>
    /// Description:
    /// Overrides the virtual UpdateUI function and uses the DisplayLives to update the lives display
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    public override void UpdateUI()
    {
        // This calls the base update UI function from the UIelement class
        base.UpdateUI();

        // The remaining code is only called for this sub-class of UIelement and not others
        DisplayLives();
    }
}
