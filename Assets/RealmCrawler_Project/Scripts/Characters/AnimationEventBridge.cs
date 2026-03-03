using System;
using UnityEngine;

/// <summary>
/// Sits on the same GameObject as the Animator.
/// Animation clips call these methods via Animation Events.
/// Conditions and action runtimes subscribe to the events they need.
/// </summary>
public class AnimationEventBridge : MonoBehaviour
{
    /// <summary>Fired when an attack or spell animation fully completes.</summary>
    public event Action OnAnimationFinished;

    /// <summary>Fired when the combo input window opens (player can buffer next hit).</summary>
    public event Action OnComboWindowOpen;

    /// <summary>Fired when the combo input window closes (missed the buffer).</summary>
    public event Action OnComboWindowClose;

    // Called by Animation Events on the clip
    public void AnimationFinished() => OnAnimationFinished?.Invoke();
    public void ComboWindowOpen()   => OnComboWindowOpen?.Invoke();
    public void ComboWindowClose()  => OnComboWindowClose?.Invoke();
}
