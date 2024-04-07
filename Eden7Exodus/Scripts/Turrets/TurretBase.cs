using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TurretBase : MonoBehaviour
{
    public DepotChoiceSO TurretDescription { get { return turretDescription; } }

    [SerializeField] protected Transform turretBase;
    [SerializeField] protected Transform turretGun;
    [SerializeField] protected DepotChoiceSO turretDescription;
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected AudioClip shootSound;

    protected Vector3 targetPoint = Vector3.zero;

    public abstract void AimAt(Vector3 targetPos);

    public abstract void StartShooting();

    public abstract void StopShooting();

    public abstract void DeactivateTurret();
    public abstract void ActivateTurret();

    protected void RotateTurretBase(Vector3 targetPos)
    {
        Vector3 tbDir = targetPos - turretBase.position;
        tbDir.y = 0;
        turretBase.forward = tbDir;
    }
}
