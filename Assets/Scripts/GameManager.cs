using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Transform player;
    public Vector2 respawnPosition;
    public float respawnDelay = 0f;
    public float deathAnimationDuration = 0.67f;

    private bool isRespawning;
    private bool isDead;
    private RigidbodyConstraints2D savedPlayerConstraints;

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
        if (isDead || isRespawning)
        {
            return;
        }

        if (player == null)
        {
            Debug.LogWarning("GameManager cannot respawn Player because player is not assigned.");
            return;
        }

        StartCoroutine(DeathAndRespawnRoutine());
    }

    private IEnumerator DeathAndRespawnRoutine()
    {
        isDead = true;

        PlayerController playerController = player.GetComponent<PlayerController>();
        Rigidbody2D playerRigidbody = player.GetComponent<Rigidbody2D>();

        if (playerController != null)
        {
            playerController.SetControlEnabled(false);
        }

        if (playerRigidbody != null)
        {
            savedPlayerConstraints = playerRigidbody.constraints;
            playerRigidbody.velocity = Vector2.zero;
            playerRigidbody.angularVelocity = 0f;
            playerRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        if (playerController != null)
        {
            playerController.PlayDeathAnimation();
        }

        if (deathAnimationDuration > 0f)
        {
            yield return new WaitForSeconds(deathAnimationDuration);
        }

        yield return StartCoroutine(RespawnPlayer());
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
            playerRigidbody.constraints = savedPlayerConstraints;
            playerRigidbody.velocity = Vector2.zero;
            playerRigidbody.angularVelocity = 0f;
        }

        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.ResetPlayerState();
            playerController.SetControlEnabled(true);
        }

        isRespawning = false;
        isDead = false;
    }
}
