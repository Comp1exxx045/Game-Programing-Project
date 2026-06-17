using UnityEngine;

/// <summary>
/// Plays a looping AudioSource whenever its GameObject is enabled.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class LoopingAudioOnEnable : MonoBehaviour
{
    private AudioSource audioSource;

    /// <summary>
    /// Caches and configures the AudioSource before playback starts.
    /// </summary>
    private void Awake()
    {
        CacheAndConfigureAudioSource();
    }

    /// <summary>
    /// Starts the loop whenever the object becomes active.
    /// </summary>
    private void OnEnable()
    {
        CacheAndConfigureAudioSource();

        if (audioSource != null && audioSource.clip != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    /// <summary>
    /// Stops the loop when the object is disabled.
    /// </summary>
    private void OnDisable()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    /// <summary>
    /// Finds the local AudioSource and ensures it is configured as a loop.
    /// </summary>
    private void CacheAndConfigureAudioSource()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (audioSource != null)
        {
            audioSource.loop = true;
        }
    }
}
