using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMenu : UIWindowBase
{
    private const string RECORD_DISTANCE_STRING = "RecordDistance";
    private const string GAME_SCENE_STRING = "GameScene";
    private const string MAIN_MENU_SCENE_STRING = "MainMenuScene";

    [Serializable]
    private class MenuButton
    {
        public Button Button { get { return button; } }
        public UnityEvent Action { get { return action; } }

        [SerializeField] private Button button;
        [SerializeField] private UnityEvent action;
    }

    [SerializeField] private GameObject currentDistanceLine;
    [SerializeField] private GameObject recordDistanceLine;
    [SerializeField] private GameObject newRecordLine;

    [SerializeField] private TextMeshProUGUI currentDistanceText;
    [SerializeField] private TextMeshProUGUI recordDistanceText;

    [SerializeField] private List<MenuButton> menuButtons;

    [SerializeField] private bool updateRecordOnShow = false;

    private float currentDistance = 0f;
    private float recordDistance = 0f;

    private UIManager uIManager = null;


    // Start is called before the first frame update
    void Awake()
    {
        foreach (MenuButton mb in menuButtons)
        {
            mb.Button.onClick.AddListener(mb.Action.Invoke);
        }

        if (uIManager == null) currentDistanceLine.SetActive(false);

        LoadRecord();
        newRecordLine.SetActive(false);
    }

    public override void UIManagerRegister(UIManager uIManager)
    {
        this.uIManager = uIManager;
        currentDistanceLine.SetActive(true);
        uIManager.OnTravelDistance += UIManager_OnTravelDistance;
    }

    private void UIManager_OnTravelDistance(object sender, float e)
    {
        currentDistance = e;
        currentDistanceText.text = currentDistance.ToString("F2") + "km";
    }

    public void CloseWindow()
    {
        if (uIManager != null)
        {
            uIManager.CloseWindow(this);
        }
    }

    public void StartGame()
    {
        ResetTime();
        SceneManager.LoadScene(GAME_SCENE_STRING);
    }

    public void MainMenu()
    {
        ResetTime();
        SceneManager.LoadScene(MAIN_MENU_SCENE_STRING);
    }

    public void ClearRecord()
    {
        PlayerPrefs.SetFloat(RECORD_DISTANCE_STRING, 0f);
        LoadRecord();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void LoadRecord()
    {
        recordDistance = PlayerPrefs.GetFloat(RECORD_DISTANCE_STRING, 0f);
        recordDistanceText.text = recordDistance.ToString("F2") + "km";
    }

    private void UpdateRecord()
    {
        if (currentDistance > recordDistance)
        {
            newRecordLine.SetActive(true);
            PlayerPrefs.SetFloat(RECORD_DISTANCE_STRING, currentDistance);
        }
    }

    private void ResetTime()
    {
        Time.timeScale = 1f;
    }

    public override void Show()
    {
        if (updateRecordOnShow) UpdateRecord();
        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }

    public override bool ShouldPause()
    {
        return true;
    }
}
