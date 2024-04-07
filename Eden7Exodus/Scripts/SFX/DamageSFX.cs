using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSFX : MonoBehaviour
{
    [SerializeField] private Health health;

    [Serializable] 
    private class DamageEffect : IComparable
    {
        public float HealthBelow { get { return healthBelow; } }
        public List<ParticleSystem> ParticleSystems { get { return particleSystems; } }


        [SerializeField] [Range(0, 1)]private float healthBelow = 1f;
        [SerializeField] private List<ParticleSystem> particleSystems = new List<ParticleSystem>();

        public int CompareTo(object obj)
        {
            DamageEffect a = this;
            DamageEffect b = obj as DamageEffect;
            if (a.healthBelow < b.healthBelow) return -1;
            if (a.healthBelow > b.healthBelow) return 1;
            return 0;
        }
    }

    [SerializeField] private List<DamageEffect> damageEffects = new List<DamageEffect>();
    private int activeIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        health.OnHealthChange += Health_OnHealthChange;

        damageEffects.Add(new DamageEffect());
        damageEffects.Sort();
        damageEffects.Reverse();
    }

    private void Health_OnHealthChange(object sender, Vector2 e)
    {
        int newIndex = FindNewIndex(e.x / e.y);
        if (newIndex != activeIndex)
        {
            foreach (ParticleSystem ps in damageEffects[activeIndex].ParticleSystems)
            {
                if (!damageEffects[newIndex].ParticleSystems.Contains(ps)) ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
            foreach (ParticleSystem ps in damageEffects[newIndex].ParticleSystems)
            {
                if (!ps.isPlaying) ps.Play(true);
            }
        }
        activeIndex = newIndex;
        //Debug.Log(newIndex);
    }

    private int FindNewIndex(float hp)
    {
        int result = 0;
        for (int i = 0; i < damageEffects.Count; i++)
        {
            if (hp <= damageEffects[i].HealthBelow) result = i;
            else return result;
        }
        return result;
    }

}
