using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretControl : MonoBehaviour
{
    [SerializeField] private PlayerInventory playerInventory;
    private TurretBase activeTurret;
    private bool weaponDisabled = false;
    // Start is called before the first frame update
    void Start()
    {
        activeTurret = playerInventory.ActiveTurret;

        PlayerControls.Instance.OnPrimaryAction += PlayerControls_OnPrimaryAction;
        PlayerControls.Instance.OnPrimaryActionCancel += PlayerControls_OnPrimaryActionCancel;
        PlayerControls.Instance.OnWeaponStatusChange += PlayerControls_OnWeaponStatusChange;
        PlayerControls.Instance.OnMouseTargetChange += PlayerControls_OnMouseTargetChange;

        playerInventory.OnTurretSwap += PlayerInventory_OnTurretSwap;
    }

    private void PlayerControls_OnMouseTargetChange(object sender, Vector3 e)
    {
        activeTurret.AimAt(e);
    }

    private void PlayerControls_OnWeaponStatusChange(object sender, bool e)
    {
        if (weaponDisabled == e) return;

        weaponDisabled = e;
        if (weaponDisabled) activeTurret.DeactivateTurret();
        else activeTurret.ActivateTurret();
    }

    private void PlayerInventory_OnTurretSwap(object sender, TurretBase e)
    {
        activeTurret = e;
        if (!weaponDisabled) activeTurret.ActivateTurret();
    }

    private void PlayerControls_OnPrimaryActionCancel(object sender, System.EventArgs e)
    {
        if (weaponDisabled) return;
        activeTurret.StopShooting();
    }

    private void PlayerControls_OnPrimaryAction(object sender, System.EventArgs e)
    {
        if (weaponDisabled) return;
        activeTurret.StartShooting();
    }

}
