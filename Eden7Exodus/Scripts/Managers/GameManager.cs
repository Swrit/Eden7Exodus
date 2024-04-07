using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler OnGameOver;

    public event EventHandler<float> OnDistanceTraveledInKm;
    public event EventHandler<float> OnScrollSpeedChanged;

    public float CurrentScrollSpeed { get { return currentScrollSpeed; } }

    [SerializeField] private LandScroll landScroll;
    [SerializeField] private float distanceToKm = 1f;
    private float distanceTraveledInKm = 0f;
    private float currentScrollSpeed = 0f;

    private Explosion explosion;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        landScroll.OnScroll += LandScroll_OnScroll;
        landScroll.OnSetScrollSpeed += LandScroll_OnSetScrollSpeed;
    }

    private void Start()
    {
        UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);

        PlayerControls.Instance.OnDeath += PlayerControls_OnDeath;
    }

    private void PlayerControls_OnDeath(object sender, Death.OnDeathArgs e)
    {
        if (e.spawnedOnDeath != null) Debug.Log(e.spawnedOnDeath.name);
        if (e.spawnedOnDeath == null) OnGameOver?.Invoke(this, EventArgs.Empty);
        else
        {
            explosion = e.spawnedOnDeath.GetComponent<Explosion>();
            Debug.Log(explosion.name);
            if (explosion != null) explosion.OnExplosionEnded += PlayerExplosion_OnExplosionEnded;
        }
    }

    private void PlayerExplosion_OnExplosionEnded(object sender, EventArgs e)
    {
        Debug.Log("explosion ended");
        explosion.OnExplosionEnded -= PlayerExplosion_OnExplosionEnded;
        OnGameOver?.Invoke(this, EventArgs.Empty);
    }

    private void LandScroll_OnScroll(object sender, float e)
    {
        distanceTraveledInKm += e * distanceToKm;
        OnDistanceTraveledInKm?.Invoke(this, distanceTraveledInKm);
    }

    private void LandScroll_OnSetScrollSpeed(object sender, float e)
    {
        currentScrollSpeed = e;
        OnScrollSpeedChanged?.Invoke(this, e);
    }
}
