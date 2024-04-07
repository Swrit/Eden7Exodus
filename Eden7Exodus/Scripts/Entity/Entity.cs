using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityOwner
{
    player,
    enemy,
    neutral,
}

public enum EntityType
{
    player,
    enemy,
    projectile,
    mine,
}

[Serializable]
public struct EntityDescription
{
    public EntityOwner Owner { get { return owner; } }
    public EntityType Type { get { return type; } }

    public bool IsPlayer { get { return ((owner == EntityOwner.player) && (type == EntityType.player)); } }

    public EntityOwner owner;
    public EntityType type;

}

[RequireComponent(typeof(Health))]
public class Entity : MonoBehaviour, IObjectReset
{
    public event EventHandler<Entity> OnEntityRemoved;

    public EntityDescription Description { get { return entityDescription; } }
    public Health Health { get { return health; } }

    [SerializeField] private EntityDescription entityDescription;
    private Health health;

    public void ResetObject(bool preSpawn = false)
    {
        if (preSpawn) return;
        OnEntityRemoved?.Invoke(this, this);
    }

    private void Start()
    {
        health = GetComponent<Health>();

        if (entityDescription.IsPlayer) PlayerControls.Instance.RegisterPlayer(gameObject);
    }

    private void OnDestroy()
    {
        OnEntityRemoved?.Invoke(this, this);
    }
}
