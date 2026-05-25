using UnityEngine;

public class WorldSwitcher : MonoBehaviour
{
    public GameObject[] worldAObjects;
    public GameObject[] worldBObjects;

    private bool isWorldA = true;

    void Start()
    {
        ApplyWorld();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isWorldA = !isWorldA;
            ApplyWorld();
        }
    }

    void ApplyWorld()
    {
        foreach (var obj in worldAObjects)
        {
            obj.SetActive(isWorldA);
        }

        foreach (var obj in worldBObjects)
        {
            obj.SetActive(!isWorldA);
        }
    }
}
