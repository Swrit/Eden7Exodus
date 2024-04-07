using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private UIWindowBase overlay = null;
    [SerializeField] private UIWindowBase pauseMenu;
    [SerializeField] private UIWindowBase gameOverMenu;
    [SerializeField] private UIWindowBase depotMenu;
    private UIWindowBase activeUI;

    public event EventHandler<float> OnHealthChange;
    public event EventHandler<List<StatusEffectAction>> OnStatusEffectChange;
    public event EventHandler<float> OnTravelDistance;
    public event EventHandler<bool> OnPauseChange;

    public static UIManager Instance { get; private set; }

    private Health playerHealth;
    private StatusEffects playerStatusEffects;
    private Death playerDeath;

    private float healthF;
    private float distanceTraveled = 0;

    private bool playerIsDead = false;
    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InputManager.Instance.OnExit += InputManager_OnExit;

        GameObject playerObject = PlayerControls.Instance.Player;

        PlayerControls.Instance.OnHealthChange += PlayerHealth_OnHealthChange;
        PlayerControls.Instance.OnStatusEffectChange += PlayerStatusEffects_OnStatusEffectChange;
        PlayerControls.Instance.OnDeath += PlayerDeath_OnDeath;
        PlayerControls.Instance.OnEnterDepot += PlayerMove_OnEnterDepot;

        GameManager.Instance.OnDistanceTraveledInKm += GameManager_OnDistanceTraveledInKm;
        GameManager.Instance.OnGameOver += GameManager_OnGameOver;

        activeUI = overlay;

        pauseMenu.UIManagerRegister(this);
        gameOverMenu.UIManagerRegister(this);

        pauseMenu.Hide();
        gameOverMenu.Hide();
        depotMenu.Hide();
    }

    private void InputManager_OnExit(object sender, EventArgs e)
    {
        if (activeUI == overlay)
        {
            if (!playerIsDead) SwitchToWindow(pauseMenu);
        }
        else if (activeUI == pauseMenu)
            SwitchToWindow(overlay);
    }

    private void PlayerHealth_OnHealthChange(object sender, Vector2 e)
    {
        healthF = e.x / e.y;
        OnHealthChange?.Invoke(this, healthF);
    }

    private void PlayerStatusEffects_OnStatusEffectChange(object sender, List<StatusEffectAction> e)
    {
        OnStatusEffectChange?.Invoke(this, e);
    }

    private void PlayerDeath_OnDeath(object sender, EventArgs e)
    {
        playerIsDead = true;
    }

    private void PlayerMove_OnEnterDepot(object sender, EventArgs e)
    {
        if (!playerIsDead) SwitchToWindow(depotMenu);
    }
    private void GameManager_OnDistanceTraveledInKm(object sender, float e)
    {    
        if (playerIsDead) return;
        distanceTraveled = e;
        OnTravelDistance?.Invoke(this, distanceTraveled);
    }

    private void GameManager_OnGameOver(object sender, EventArgs e)
    {
        SwitchToWindow(gameOverMenu);
    }

    // Update is called once per frame
    private void SwitchToWindow(UIWindowBase uIWindow)
    {
        activeUI.Hide();
        activeUI = uIWindow;
        activeUI.Show();
        SetPause(activeUI.ShouldPause());
    }

    public void CloseWindow(UIWindowBase uIWindow)
    {
        if (activeUI == uIWindow)
        {
            SwitchToWindow(overlay);
        }
    }

    private void SetPause(bool pause)
    {
        if (isPaused == pause) return;

        if (pause) Time.timeScale = float.Epsilon;
        else Time.timeScale = 1f;
        isPaused = pause;
        OnPauseChange?.Invoke(this, isPaused);
    }

    //for testing
    public void CheatDepot()
    {
        SwitchToWindow(depotMenu);
    }
}
