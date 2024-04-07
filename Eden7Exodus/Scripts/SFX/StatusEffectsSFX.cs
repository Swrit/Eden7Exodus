using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectsSFX : MonoBehaviour
{
    [SerializeField] private StatusEffects statusEffects;

    [Serializable]
    private class StatusVisualEffect
    {
        public StatusEffectAction StatusAction { get { return statusAction; } }
        public List<ParticleSystem> ParticleSystems { get { return particleSystems; } }


        [SerializeField] private StatusEffectAction statusAction;
        [SerializeField] private List<ParticleSystem> particleSystems = new List<ParticleSystem>();
    }

    [SerializeField] private List<StatusVisualEffect> statusVisualEffects = new List<StatusVisualEffect>();

    private void Start()
    {
        if (statusEffects != null)
        {
            statusEffects.OnStatusEffectChange += StatusEffects_OnStatusEffectChange;
        }
    }

    private void StatusEffects_OnStatusEffectChange(object sender, List<StatusEffectAction> e)
    {
        List<ParticleSystem> systemsToKeepActive = new List<ParticleSystem>();
        foreach (StatusVisualEffect sve in statusVisualEffects)
        {
            if (e.Contains(sve.StatusAction))
            {
                foreach (ParticleSystem ps in sve.ParticleSystems)
                {
                    if (!systemsToKeepActive.Contains(ps)) systemsToKeepActive.Add(ps);
                }
            }
        }

        foreach (StatusVisualEffect sve in statusVisualEffects)
        {
            foreach (ParticleSystem ps in sve.ParticleSystems)
            {
                if ((systemsToKeepActive.Contains(ps)) && !ps.isPlaying) ps.Play(true);
                else ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);

            }
        }
    }
}
