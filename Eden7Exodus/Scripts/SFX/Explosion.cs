using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour, IObjectReset
{
    public event EventHandler OnExplosionEnded;

    [SerializeField] private AudioClip sound;

    private List<ParticleSystem> particleSystems = new List<ParticleSystem>();
    private List<AnimationCallbacks> animations = new List<AnimationCallbacks>();

    public void ResetObject(bool preSpawn = false)
    {
        if (preSpawn) return;
        OnExplosionEnded?.Invoke(this, EventArgs.Empty);
        RemakeAnimationsList();
    }

    // Start is called before the first frame update
    void Start()
    {
        particleSystems = new List<ParticleSystem>(GetComponentsInChildren<ParticleSystem>());
        RemakeAnimationsList();
    }

    // Update is called once per frame
    void Update()
    {
        if (ExplosionEnded()) ObjectPoolManager.Instance.Deactivate(this.gameObject);
    }

    private void OnEnable()
    {
        if (sound != null) SoundManager.Instance.PlaySound(sound);
    }

    private bool ExplosionEnded()
    {
        if (animations.Count > 0) return false;
        foreach (ParticleSystem ps in particleSystems)
        {
            if (ps.IsAlive(true)) return false;
        }
        return true;
    }

    private void RemakeAnimationsList()
    {
        foreach (AnimationCallbacks ac in animations)
        {
            ac.OnAnimationFinished -= AnimationCallbacks_OnAnimationFinished;
        }
        animations = new List<AnimationCallbacks>(GetComponentsInChildren<AnimationCallbacks>());
        foreach (AnimationCallbacks ac in animations)
        {
            ac.OnAnimationFinished += AnimationCallbacks_OnAnimationFinished;
        }
    }
    private void AnimationCallbacks_OnAnimationFinished(object sender, AnimationCallbacks e)
    {
        e.OnAnimationFinished -= AnimationCallbacks_OnAnimationFinished;
        animations.Remove(e);
    }
}
