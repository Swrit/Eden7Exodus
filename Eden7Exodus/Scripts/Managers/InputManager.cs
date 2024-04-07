using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInputs playerInputs;

    public event EventHandler OnUp;
    public event EventHandler OnUpCancel;
    public event EventHandler OnDown;
    public event EventHandler OnDownCancel;
    public event EventHandler OnPrimaryAction;
    public event EventHandler OnPrimaryActionCancel;
    public event EventHandler OnExit;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        playerInputs = new PlayerInputs();
        playerInputs.TankControls.Enable();
        playerInputs.TankControls.Up.performed += Up_performed; ;
        playerInputs.TankControls.Up.canceled += Up_canceled; ;
        playerInputs.TankControls.Down.performed += Down_performed; ;
        playerInputs.TankControls.Down.canceled += Down_canceled;
        playerInputs.TankControls.PrimaryAction.performed += PrimaryAction_performed;
        playerInputs.TankControls.PrimaryAction.canceled += PrimaryAction_canceled;
        playerInputs.TankControls.Exit.performed += Exit_performed;
    }

    private void OnDestroy()
    {
        playerInputs.TankControls.Up.performed -= Up_performed; ;
        playerInputs.TankControls.Up.canceled -= Up_canceled; ;
        playerInputs.TankControls.Down.performed -= Down_performed; ;
        playerInputs.TankControls.Down.canceled -= Down_canceled;
        playerInputs.TankControls.PrimaryAction.performed -= PrimaryAction_performed;
        playerInputs.TankControls.PrimaryAction.canceled -= PrimaryAction_canceled;
        playerInputs.TankControls.Exit.performed -= Exit_performed;
    }

    private void PrimaryAction_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPrimaryActionCancel?.Invoke(this, EventArgs.Empty);
    }

    private void PrimaryAction_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPrimaryAction?.Invoke(this, EventArgs.Empty);
    }

    private void Down_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnDownCancel?.Invoke(this, EventArgs.Empty);
    }

    private void Down_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnDown?.Invoke(this, EventArgs.Empty);
    }

    private void Up_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnUpCancel?.Invoke(this, EventArgs.Empty);
    }

    private void Up_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnUp?.Invoke(this, EventArgs.Empty);
    }

    private void Exit_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnExit?.Invoke(this, EventArgs.Empty);
    }

}
