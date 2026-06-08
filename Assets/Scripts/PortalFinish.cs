using UnityEngine;

public class PortalFinish : MonoBehaviour
{
    public string finishMessage = "finish";
    public float messageDuration = 2f;

    private bool showMessage;
    private float messageTimer;

    void Update()
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerController>() == null)
        {
            return;
        }

        showMessage = true;
        messageTimer = messageDuration;
        Debug.Log(finishMessage);
    }

    void OnGUI()
    {
        if (!showMessage)
        {
            return;
        }

        GUIStyle style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 40
        };

        style.normal.textColor = Color.white;

        Rect rect = new Rect(0f, Screen.height * 0.35f, Screen.width, 80f);
        GUI.Label(rect, finishMessage, style);
    }
}
