using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
public class TimedBarrier : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField, Min(0f)] private float activeDuration = 2.5f;
    [SerializeField, Min(0f)] private float extraOpenDuration = 1f;

    [Header("Deactivation Animation")]
    [SerializeField, Min(1f)] private float animationFrameRate = 12f;
    [SerializeField, Min(0)] private int passableStartFrame = 13;
    [SerializeField, Min(0)] private int finalFrame = 17;

    private static readonly int ActiveStateHash = Animator.StringToHash("Barrier");
    private static readonly int DeactivateStateHash = Animator.StringToHash("BarrierDeact");

    private Animator barrierAnimator;
    private Collider2D barrierCollider;
    private Coroutine barrierCycle;

    /// <summary>
    /// Caches the Animator and Collider2D used by the barrier cycle.
    /// </summary>
    private void Awake()
    {
        barrierAnimator = GetComponent<Animator>();
        barrierCollider = GetComponent<Collider2D>();
    }

    /// <summary>
    /// Restarts the complete barrier cycle whenever its world becomes active.
    /// </summary>
    private void OnEnable()
    {
        barrierCycle = StartCoroutine(RunBarrierCycle());
    }

    /// <summary>
    /// Repeats the active, deactivation, passable, and reset phases.
    /// </summary>
    private IEnumerator RunBarrierCycle()
    {
        SetColliderEnabled(true);
        PlayState(ActiveStateHash);

        while (true)
        {
            yield return new WaitForSeconds(activeDuration);

            PlayState(DeactivateStateHash);
            yield return new WaitForSeconds(passableStartFrame / animationFrameRate);

            SetColliderEnabled(false);

            float remainingAnimationTime =
                Mathf.Max(0, finalFrame - passableStartFrame) / animationFrameRate;
            yield return new WaitForSeconds(remainingAnimationTime);

            barrierAnimator.speed = 0f;
            yield return new WaitForSeconds(extraOpenDuration);

            SetColliderEnabled(true);
            PlayState(ActiveStateHash);
        }
    }

    /// <summary>
    /// Restores the barrier to a blocking state when the component is disabled.
    /// </summary>
    private void OnDisable()
    {
        if (barrierCycle != null)
        {
            StopCoroutine(barrierCycle);
            barrierCycle = null;
        }

        if (barrierAnimator != null)
        {
            barrierAnimator.speed = 1f;
        }

        SetColliderEnabled(true);
    }

    /// <summary>
    /// Enables or disables the collider that controls whether the player can pass.
    /// </summary>
    private void SetColliderEnabled(bool isEnabled)
    {
        if (barrierCollider != null)
        {
            barrierCollider.enabled = isEnabled;
        }
    }

    /// <summary>
    /// Plays an Animator state from its first frame at normal speed.
    /// </summary>
    private void PlayState(int stateHash)
    {
        if (barrierAnimator == null)
        {
            return;
        }

        barrierAnimator.speed = 1f;
        barrierAnimator.Play(stateHash, 0, 0f);
    }
}
