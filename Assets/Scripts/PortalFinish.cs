using UnityEngine;

public class PortalFinish : MonoBehaviour
{
    public string finishMessage = "finish";
    public float messageDuration = 2f;

    private bool showMessage;
    private float messageTimer;
    private GUIStyle messageStyle;

    /// <summary>
    /// Counts down the remaining time for the portal completion message.
    /// </summary>
    private void Update()
    {
        if (!showMessage)
        {
            return;
        }

        messageTimer -= Time.deltaTime;
        if (messageTimer <= 0f)
        {
            showMessage = false;
        }
    }

    /// <summary>
    /// Displays the completion message when the player enters the portal.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerController>() == null)
        {
            return;
        }

        showMessage = true;
        messageTimer = messageDuration;
    }

    /// <summary>
    /// Prepares the cached GUI style and draws the completion message when active.
    /// </summary>
    private void OnGUI()
    {
        EnsureMessageStyle();

        if (!showMessage)
        {
            return;
        }

        Rect rect = new Rect(0f, Screen.height * 0.35f, Screen.width, 80f);
        GUI.Label(rect, finishMessage, messageStyle);
    }

    /// <summary>
    /// Creates the portal message style once before the first interaction.
    /// </summary>
    private void EnsureMessageStyle()
    {
        if (messageStyle != null)
        {
            return;
        }

        messageStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 40
        };

        messageStyle.normal.textColor = Color.white;
    }
}
