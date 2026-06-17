using UnityEngine;

public class WorldPositionHintUI : MonoBehaviour
{
    [SerializeField] private GameObject hintRoot;
    [SerializeField] private Transform player;
    [SerializeField] private float showAtX = 33f;

    private bool hasShown;

    private void Awake()
    {
        if (hintRoot != null)
        {
            hintRoot.SetActive(false);
        }

        ResolvePlayer();
    }

    private void Update()
    {
        if (hasShown)
        {
            return;
        }

        if (!ResolvePlayer())
        {
            return;
        }

        if (player.position.x >= showAtX)
        {
            hasShown = true;

            if (hintRoot != null)
            {
                hintRoot.SetActive(true);
            }
        }
    }

    private bool ResolvePlayer()
    {
        if (player == null)
        {
            PlayerController playerController = FindObjectOfType<PlayerController>(true);
            if (playerController != null)
            {
                player = playerController.transform;
            }
        }

        return player != null;
    }
}
