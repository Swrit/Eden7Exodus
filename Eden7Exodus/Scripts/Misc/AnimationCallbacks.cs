using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCallbacks : MonoBehaviour
{
    public event EventHandler<AnimationCallbacks> OnAnimationFinished;

    public void AnimationFinished()
    {
        OnAnimationFinished?.Invoke(this, this);
    }

    public void PlaySound(AudioClip sound)
    {
        SoundManager.Instance.PlaySound(sound);
    }
}
