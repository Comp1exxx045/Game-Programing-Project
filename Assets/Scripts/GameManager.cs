using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Transform player;
    public Vector2 respawnPosition;
    public float respawnDelay = 0f;

    private bool isRespawning;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void KillPlayer()
    {
        if (isRespawning)
        {
            return;
        }

        if (player == null)
        {
            Debug.LogWarning("GameManager cannot respawn Player because player is not assigned.");
            return;
        }

        StartCoroutine(RespawnPlayer());
    }

    private IEnumerator RespawnPlayer()
    {
        isRespawning = true;

        if (respawnDelay > 0f)
        {
            yield return new WaitForSeconds(respawnDelay);
        }

        player.position = new Vector3(respawnPosition.x, respawnPosition.y, player.position.z);

        Rigidbody2D playerRigidbody = player.GetComponent<Rigidbody2D>();
        if (playerRigidbody != null)
        {
            playerRigidbody.velocity = Vector2.zero;
            playerRigidbody.angularVelocity = 0f;
        }

        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.ResetPlayerState();
        }

        isRespawning = false;
    }
}
