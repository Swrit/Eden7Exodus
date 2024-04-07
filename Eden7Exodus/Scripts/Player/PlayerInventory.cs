using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour, IObjectReset
{
    public TurretBase ActiveTurret { get { return activeTurret; } }

    public event EventHandler<TurretBase> OnTurretSwap;

    [SerializeField] private TurretDescriptionSO startingTurret;
    private TurretBase activeTurret;
    [SerializeField] private Transform turretHolder;

    public void ResetObject(bool preSpawn = false)
    {
        SwapTurret(startingTurret.TurretObject);
    }

    public void SwapTurret(GameObject newTurretPrefab)
    {
        if (newTurretPrefab.GetComponent<TurretBase>() == null) return;
        Debug.Log("swapping");
        if (activeTurret != null) 
        { 
            activeTurret.DeactivateTurret();
            ObjectPoolManager.Instance.Deactivate(activeTurret.gameObject);
        }
        GameObject newTurret = ObjectPoolManager.Instance.RequestObjectAt(newTurretPrefab, turretHolder.position);
        newTurret.transform.SetParent(turretHolder, true);
        activeTurret = newTurret.GetComponent<TurretBase>();
        OnTurretSwap?.Invoke(this, activeTurret);
    }

}
