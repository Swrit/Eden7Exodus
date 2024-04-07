using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEffect : MonoBehaviour, IObjectReset
{
    private enum HurtMode
    {
        hurtOncePerTarget,
        hurtOneTarget,
        hurtOverTime,
    }

    [SerializeField] private List<EntityDescription> acceptableTargets = new List<EntityDescription>();

    [SerializeField] private HurtMode hurtMode;
    [SerializeField] float damage = 0f;
    [SerializeField] private List<StatusEffectInflicted> inflictStatusEffects = new List<StatusEffectInflicted>();
    [SerializeField] private List<StatusEffectSO> cureStatusEffects = new List<StatusEffectSO>();

    [SerializeField] private LayerMask collideWith;
    private Death death;
    private List<Entity> targetsHit = new List<Entity>();
    public void ResetObject(bool preSpawn = false)
    {
        targetsHit.Clear();
    }

    // Start is called before the first frame update
    void Start()
    {
        death = GetComponent<Death>();

        ProjectileBeam projectileBeam = GetComponent<ProjectileBeam>();
        if (projectileBeam != null) projectileBeam.OnHitObject += ProjectileBeam_OnHitObject;
    }

    private void ProjectileBeam_OnHitObject(object sender, GameObject e)
    {
        Contact(e);
    }

    private void OnTriggerEnter(Collider other)
    {
        Contact(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Contact(collision.gameObject);
    }

    private void Contact(GameObject go)
    {
        if ((death != null) && (death.IsDead)) return;

        //Debug.Log("contact " + go.name);
        if (collideWith.value == (collideWith.value | (1 << go.layer)))
        {
            Debug.Log(gameObject.name + " collide " + go.name);
            if (death != null) death.Die();
        }

        Entity entity = go.GetComponent<Entity>();
        if (entity != null)
        {
            if (acceptableTargets.Contains(entity.Description))
            {
                switch (hurtMode)
                {
                    case HurtMode.hurtOncePerTarget:
                        if (!targetsHit.Contains(entity))
                        {
                            RememberHitEntity(entity);
                            Hurt(entity);
                        }
                        break;
                    case HurtMode.hurtOneTarget:
                        Hurt(entity);
                        if (death != null) death.Die();
                        break;
                    case HurtMode.hurtOverTime:
                        Hurt(entity);
                        break;
                }
            }
        }
    }

    private float GetDamage()
    {
        if (hurtMode == HurtMode.hurtOverTime) return damage * Time.deltaTime;
        else return damage;
    }

    private void Hurt(Entity entity)
    {
        //Debug.Log("hurt " + entity.gameObject.name + " " + GetDamage());
        entity.Health.TryGetHit(GetDamage());
        StatusEffects se = entity.GetComponent<StatusEffects>();
        if (se != null)
        {
            foreach (StatusEffectInflicted sei in inflictStatusEffects)
            {
                se.Inflict(sei);
            }
            foreach (StatusEffectSO sso in cureStatusEffects)
            {
                se.Cure(sso);
            }
        }
    }

    private void RememberHitEntity(Entity entity)
    {
        targetsHit.Add(entity);
        entity.OnEntityRemoved += Entity_OnEntityRemoved;
    }

    private void ForgetHitEntity(Entity entity)
    {
        targetsHit.Remove(entity);
        entity.OnEntityRemoved -= Entity_OnEntityRemoved;
    }

    private void Entity_OnEntityRemoved(object sender, Entity e)
    {
        ForgetHitEntity(e);
    }
}
