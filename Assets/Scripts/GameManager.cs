using System.Collections;
using UnityEngine;

/// <summary>
/// Coordinates player death, respawn timing, and respawn position management.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Transform player;
    public Vector2 respawnPosition;
    public bool usePlayerStartPositionAsRespawn;
    public float respawnDelay = 0f;
    public float deathAnimationDuration = 0.67f;

    private bool isRespawning;
    private bool isDead;
    private bool respawnPositionInitialized;
    private RigidbodyConstraints2D savedPlayerConstraints;

    /// <summary>
    /// Establishes the singleton instance and resolves the player reference.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        FindPlayerIfNeeded();
    }

    /// <summary>
    /// Clears the singleton reference when this manager is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    /// <summary>
    /// Starts the death and respawn flow if the player is currently alive.
    /// </summary>
    public void KillPlayer()
    {
        if (isDead || isRespawning)
        {
            return;
        }

        if (!FindPlayerIfNeeded())
        {
            Debug.LogWarning("GameManager cannot respawn Player because player is not assigned.");
            return;
        }

        StartCoroutine(DeathAndRespawnRoutine());
    }

    /// <summary>
    /// Locks player control, plays the death animation, and waits before respawning.
    /// </summary>
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

    /// <summary>
    /// Moves the player back to the respawn position and restores movement control.
    /// </summary>
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

    /// <summary>
    /// Resolves the player reference and initializes the respawn point when configured.
    /// </summary>
    /// <returns>True when a player transform is available.</returns>
    private bool FindPlayerIfNeeded()
    {
        if (player == null)
        {
            PlayerController playerController = FindObjectOfType<PlayerController>(true);
            if (playerController != null)
            {
                player = playerController.transform;
            }
        }

        if (player != null && usePlayerStartPositionAsRespawn && !respawnPositionInitialized)
        {
            respawnPosition = player.position;
            respawnPositionInitialized = true;
        }

        return player != null;
    }
}
