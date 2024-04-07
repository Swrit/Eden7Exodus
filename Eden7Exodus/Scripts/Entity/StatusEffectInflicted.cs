using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StatusEffectInflicted
{
    public StatusEffectSO StatusEffect { get { return statusEffect; } }

    [SerializeField] StatusEffectSO statusEffect;
    [SerializeField] private float duration;
    private float timer;

    public StatusEffectInflicted(StatusEffectInflicted original)
    {
        statusEffect = original.statusEffect;
        duration = original.duration;
        timer = duration;
    }

    public bool TimerRanOut(float deltaTime)
    {
        if (!statusEffect.IsTimed) return false;

        timer -= deltaTime;
        return (timer <= 0);
    }

    public void Cumulate(StatusEffectInflicted otherEffect)
    {
        float time = otherEffect.duration;
        switch (statusEffect.CumulativeEffect)
        {
            case StatusEffectCumulativeEffect.add:
                timer += time;
                break;
            case StatusEffectCumulativeEffect.refresh:
                timer = Mathf.Max(timer, time);
                break;
            case StatusEffectCumulativeEffect.none:
                break;
        }
    }
}
