using UnityEngine;
using TMPro;
using System.Collections;

public class LookAtTarget : MonoBehaviour
{
    [Header("Target Settings")]
    public GameObject defaultTarget;
    public GameObject currentTarget;

    [Header("UI")]
    public TextMeshProUGUI selectedNameText;
    public TextMeshProUGUI planetInfoText;

    private GameObject lastTarget;
    private Color originalColor;

    void Start()
    {
        if (defaultTarget == null)
        {
            defaultTarget = this.gameObject;
        }

        if (currentTarget == null)
        {
            currentTarget = defaultTarget;
        }

        if (selectedNameText != null)
        {
            selectedNameText.text = "";
        }

        if (planetInfoText != null)
        {
            planetInfoText.text = "";
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            if (hits.Length > 0)
            {
                GameObject clicked = hits[0].collider.gameObject;

                currentTarget = clicked;

                AudioSource audio = clicked.GetComponent<AudioSource>();
                if (audio != null)
                {
                    audio.Play();
                }

                HighlightTarget(clicked);

                if (selectedNameText != null)
                {
                    selectedNameText.text = clicked.name;
                }

                if (planetInfoText != null)
                {
                    planetInfoText.text = GetPlanetInfo(clicked.name);
                }

                Debug.Log("Selected: " + clicked.name);
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            currentTarget = defaultTarget;

            ClearHighlight();

            if (selectedNameText != null)
            {
                selectedNameText.text = "";
            }

            if (planetInfoText != null)
            {
                planetInfoText.text = "";
            }
        }

        if (currentTarget != null)
        {
            transform.LookAt(currentTarget.transform);
        }
    }

    void HighlightTarget(GameObject target)
    {
        ClearHighlight();

        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer != null)
        {
            originalColor = renderer.material.color;
            renderer.material.color = Color.yellow;
        }

        lastTarget = target;
    }

    void ClearHighlight()
    {
        if (lastTarget != null)
        {
            Renderer renderer = lastTarget.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = originalColor;
            }
        }

        lastTarget = null;
    }

    string GetPlanetInfo(string objectName)
    {
        switch (objectName)
        {
            case "Sun":
                return "The Sun is a star. It gives light and heat to the solar system.";

            case "Mercury":
                return "Mercury is the closest planet to the Sun.";

            case "Venus":
                return "Venus is very hot and covered with thick clouds.";

            case "Earth":
                return "Earth is our home planet. It has air, water, and life.";

            case "Moon":
                return "The Moon goes around Earth. We can see it at night.";

            case "Mars":
                return "Mars is called the Red Planet because of its rusty surface.";

            case "Jupiter":
                return "Jupiter is the largest planet in the solar system.";

            case "Saturn":
                return "Saturn is famous for its beautiful rings.";

            case "Uranus":
                return "Uranus spins on its side and looks blue-green.";

            case "Neptune":
                return "Neptune is far from the Sun and has strong winds.";

            default:
                return "This is a space object. Click around to explore!";
        }
    }
}