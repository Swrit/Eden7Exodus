using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour, IRailMovementController
{
    public static PlayerControls Instance { get; private set; }
    public GameObject Player { get { return player; } }

    //Events from player
    public event EventHandler<Vector2> OnHealthChange;
    public event EventHandler<List<StatusEffectAction>> OnStatusEffectChange;
    public event EventHandler<Death.OnDeathArgs> OnDeath;
    public event EventHandler OnEnterDepot;

    //Events for player
    public event EventHandler OnPrimaryAction;
    public event EventHandler OnPrimaryActionCancel;
    public event EventHandler<bool> OnControlStatusChange;
    public event EventHandler<bool> OnWeaponStatusChange;
    public event EventHandler<Vector3> OnMouseTargetChange;
    public event EventHandler OnControlUp;
    public event EventHandler OnControlDown;

    [SerializeField] private GameObject player;
    [SerializeField] private MouseTarget mouseTarget;

    private bool statusEffectDisablesWeapon = false;
    private bool statusEffectDisablesControls = false;
    private bool gamePaused = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        InputManager.Instance.OnUp += InputManager_OnUp;
        InputManager.Instance.OnDown += InputManager_OnDown;
        InputManager.Instance.OnPrimaryAction += InputManager_OnPrimaryAction;
        InputManager.Instance.OnPrimaryActionCancel += InputManager_OnPrimaryActionCancel;

        UIManager.Instance.OnPauseChange += UIManager_OnPauseChange;

        mouseTarget.OnTargetChange += MouseTarget_OnTargetChange;


    }

    public void RegisterPlayer(GameObject playerApplicant)
    {
        if (player == null) player = playerApplicant;

        StatusEffects playerStatusEffects = player.GetComponent<StatusEffects>();
        if (playerStatusEffects != null) playerStatusEffects.OnStatusEffectChange += Player_OnStatusEffectChange;

        Health health = player.GetComponent<Health>();
        if (health != null) health.OnHealthChange += PlayerHealth_OnHealthChange;

        StatusEffects statusEffects = player.GetComponent<StatusEffects>();
        if (statusEffects != null) statusEffects.OnStatusEffectChange += PlayerStatusEffects_OnStatusEffectChange;

        Death death = player.GetComponent<Death>();
        if (death != null) death.OnDeath += PlayerDeath_OnDeath;

        MoveAlongRail moveAlongRail = player.GetComponent<MoveAlongRail>();
        if (moveAlongRail != null)
        {
            moveAlongRail.SetController(this);
            moveAlongRail.OnEnterDepot += PlayerMoveAlongRail_OnEnterDepot;
        }

    }

    private void PlayerMoveAlongRail_OnEnterDepot(object sender, EventArgs e)
    {
        OnEnterDepot?.Invoke(this, e);
    }

    private void PlayerHealth_OnHealthChange(object sender, Vector2 e)
    {
        OnHealthChange?.Invoke(this, e);
    }

    private void PlayerStatusEffects_OnStatusEffectChange(object sender, List<StatusEffectAction> e)
    {
        OnStatusEffectChange?.Invoke(this, e);
    }
    private void PlayerDeath_OnDeath(object sender, Death.OnDeathArgs e)
    {
        OnDeath?.Invoke(this, e);
    }
    private void MouseTarget_OnTargetChange(object sender, Vector3 e)
    {
        if (ControlsEnabled()) OnMouseTargetChange?.Invoke(this, e);
    }

    private void UIManager_OnPauseChange(object sender, bool e)
    {
        gamePaused = e;
        ControlStatusChanged();
    }

    private void Player_OnStatusEffectChange(object sender, List<StatusEffectAction> e)
    {
        statusEffectDisablesControls = e.Contains(StatusEffectAction.disableControls);
        ControlStatusChanged();
        statusEffectDisablesWeapon = e.Contains(StatusEffectAction.disableWeapon);
        OnWeaponStatusChange?.Invoke(this, statusEffectDisablesWeapon);
    }

    private void InputManager_OnDown(object sender, EventArgs e)
    {
        if (ControlsEnabled()) OnControlDown?.Invoke(this, EventArgs.Empty);
    }

    private void InputManager_OnUp(object sender, EventArgs e)
    {
        if (ControlsEnabled()) OnControlUp?.Invoke(this, EventArgs.Empty);
    }

    private void InputManager_OnPrimaryAction(object sender, EventArgs e)
    {
        if (ControlsEnabled()) OnPrimaryAction?.Invoke(this, EventArgs.Empty);
    }

    private void InputManager_OnPrimaryActionCancel(object sender, EventArgs e)
    {
        if (ControlsEnabled()) OnPrimaryActionCancel?.Invoke(this, EventArgs.Empty);
    }

    private bool ControlsEnabled()
    {
        if (statusEffectDisablesControls) return false;
        if (gamePaused) return false;
        return true;
    }

    private void ControlStatusChanged()
    {
        OnControlStatusChange?.Invoke(this, ControlsEnabled());
    }

}
