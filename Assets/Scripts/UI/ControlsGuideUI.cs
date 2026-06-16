using UnityEngine;

/// <summary>
/// Displays the Level01 controls guide, pauses gameplay, and waits for Enter to continue.
/// </summary>
public class ControlsGuideUI : MonoBehaviour
{
    [SerializeField] private GameObject controlsGuideRoot;
    [SerializeField] private GameObject[] guidePanels;

    private PlayerController playerController;
    private WorldSwitcher worldSwitcher;
    private float previousTimeScale = 1f;
    private int currentPanelIndex;
    private bool wasWorldSwitcherEnabled;

    /// <summary>
    /// Gets whether the controls guide is currently open.
    /// </summary>
    public bool IsOpen { get; private set; }

    /// <summary>
    /// Hides the guide root before the first frame and validates required references.
    /// </summary>
    private void Awake()
    {
        if (controlsGuideRoot != null)
        {
            controlsGuideRoot.SetActive(false);
        }

        HideGuidePanels();
        ResolvePlayerController();
        ResolveWorldSwitcher();
        ValidateConfiguration();
    }

    /// <summary>
    /// Opens the guide after other UI components have finished their Awake initialization.
    /// </summary>
    private void Start()
    {
        ShowGuide();
    }

    /// <summary>
    /// Advances the guide when the player presses either Enter key.
    /// </summary>
    private void Update()
    {
        if (!IsOpen)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            AdvanceGuide();
        }
    }

    /// <summary>
    /// Shows the controls guide once and pauses gameplay until the player continues.
    /// </summary>
    public void ShowGuide()
    {
        if (IsOpen)
        {
            return;
        }

        if (controlsGuideRoot == null)
        {
            Debug.LogError(
                "ControlsGuideUI cannot open because Controls Guide Root is not assigned.",
                this);
            return;
        }

        IsOpen = true;
        previousTimeScale = Time.timeScale > 0f ? Time.timeScale : 1f;

        if (ResolvePlayerController())
        {
            playerController.SetControlEnabled(false);
            StopPlayerMotion();
        }

        DisableWorldSwitcher();
        controlsGuideRoot.SetActive(true);
        ShowGuidePanel(0);
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Moves to the next guide panel, or closes the guide after the final panel.
    /// </summary>
    public void AdvanceGuide()
    {
        if (!IsOpen)
        {
            return;
        }

        if (guidePanels == null || guidePanels.Length == 0)
        {
            CloseGuide();
            return;
        }

        if (currentPanelIndex < guidePanels.Length - 1)
        {
            ShowGuidePanel(currentPanelIndex + 1);
            return;
        }

        CloseGuide();
    }

    /// <summary>
    /// Hides the controls guide, restores player control, and resumes the previous time scale.
    /// </summary>
    public void CloseGuide()
    {
        if (!IsOpen)
        {
            return;
        }

        IsOpen = false;

        if (controlsGuideRoot != null)
        {
            controlsGuideRoot.SetActive(false);
        }

        HideGuidePanels();

        if (ResolvePlayerController())
        {
            playerController.SetControlEnabled(true);
        }

        RestoreWorldSwitcher();
        Time.timeScale = previousTimeScale > 0f ? previousTimeScale : 1f;
    }

    /// <summary>
    /// Displays one guide panel and hides every other configured panel.
    /// </summary>
    /// <param name="panelIndex">The index of the panel that should be visible.</param>
    private void ShowGuidePanel(int panelIndex)
    {
        if (guidePanels == null || guidePanels.Length == 0)
        {
            return;
        }

        currentPanelIndex = Mathf.Clamp(panelIndex, 0, guidePanels.Length - 1);

        for (int i = 0; i < guidePanels.Length; i++)
        {
            if (guidePanels[i] != null)
            {
                guidePanels[i].SetActive(i == currentPanelIndex);
            }
        }
    }

    /// <summary>
    /// Hides every configured guide panel.
    /// </summary>
    private void HideGuidePanels()
    {
        if (guidePanels == null)
        {
            return;
        }

        for (int i = 0; i < guidePanels.Length; i++)
        {
            if (guidePanels[i] != null)
            {
                guidePanels[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// Uses the current scene's player controller when one is available.
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
    /// Uses the current scene's world switcher when one is available.
    /// </summary>
    /// <returns>True when a world switcher is available.</returns>
    private bool ResolveWorldSwitcher()
    {
        if (worldSwitcher == null)
        {
            worldSwitcher = FindObjectOfType<WorldSwitcher>(true);
        }

        return worldSwitcher != null;
    }

    /// <summary>
    /// Temporarily disables world-switch input while the guide is open.
    /// </summary>
    private void DisableWorldSwitcher()
    {
        if (!ResolveWorldSwitcher())
        {
            return;
        }

        wasWorldSwitcherEnabled = worldSwitcher.enabled;
        worldSwitcher.enabled = false;
    }

    /// <summary>
    /// Restores the world switcher to the state it had before the guide opened.
    /// </summary>
    private void RestoreWorldSwitcher()
    {
        if (worldSwitcher == null)
        {
            return;
        }

        worldSwitcher.enabled = wasWorldSwitcherEnabled;
    }

    /// <summary>
    /// Clears the player's current physics velocity before gameplay is paused.
    /// </summary>
    private void StopPlayerMotion()
    {
        Rigidbody2D playerRigidbody = playerController.GetComponent<Rigidbody2D>();
        if (playerRigidbody == null)
        {
            return;
        }

        playerRigidbody.velocity = Vector2.zero;
        playerRigidbody.angularVelocity = 0f;
    }

    /// <summary>
    /// Reports missing references with a clear error message.
    /// </summary>
    private void ValidateConfiguration()
    {
        if (controlsGuideRoot == null)
        {
            Debug.LogError("ControlsGuideUI is missing its Controls Guide Root reference.", this);
        }

        if (guidePanels == null || guidePanels.Length == 0)
        {
            Debug.LogError("ControlsGuideUI is missing its guide panel references.", this);
        }
    }
}
