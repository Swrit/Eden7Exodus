using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    static public SoundManager Instance { get; private set; }

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource musicSource;


    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        UIManager uIManager = UIManager.Instance;
        if (uIManager != null)
        {
            uIManager.OnPauseChange += UIManager_OnPauseChange;
        }
    }



    public void PlaySound(AudioClip soundfile)
    {
        audioSource.PlayOneShot(soundfile);
    }

    private void UIManager_OnPauseChange(object sender, bool e)
    {
        if (e) musicSource.volume *= 0.5f;
        else musicSource.volume *= 2f;
    }
}
