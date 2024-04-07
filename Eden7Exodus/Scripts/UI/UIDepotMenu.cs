using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDepotMenu : UIWindowBase
{
    [SerializeField] private DepotChoiceSO noChoice;
    [SerializeField] private DepotChoiceSO fullRepair;
    [SerializeField] private List<DepotChoiceSO> possibleChoices = new List<DepotChoiceSO>();

    [SerializeField] private Transform choiceButtonsHolder;
    [SerializeField] private GameObject choiceButtonPrefab;
    private List<UIDepotButton> choiceButtons = new List<UIDepotButton>();
    private List<UIDepotButton> disabledButtons = new List<UIDepotButton>();
    private DepotChoiceSO activeChoice;

    [SerializeField] private int randomChoicesPerDepot = 2;
    [SerializeField] private UIDepotButton playerTurretButton;
    private DepotChoiceSO playerCurrentTurret;
    [SerializeField] private Button finishButton;

    private Health playerHealth;
    private StatusEffects playerStatusEffects;
    private PlayerInventory playerInventory;

    [SerializeField] private TextMeshProUGUI ibTitle;
    [SerializeField] private TextMeshProUGUI ibText;
    [SerializeField] private Slider turrPowerSlider;
    [SerializeField] private Slider turrFirerateSlider;
    [SerializeField] private Slider turrrTargetSlider;

    private void Start()
    {
        finishButton.onClick.AddListener(FinalizeChoice);
        playerTurretButton.OnMouseOver += Uidb_OnMouseOver;
        playerTurretButton.OnMouseExit += Uidb_OnMouseExit;
    }

    private void GetPlayerObject()
    {
        GameObject player = PlayerControls.Instance.Player;
        playerHealth = player.GetComponent<Health>();
        playerStatusEffects = player.GetComponent<StatusEffects>();
        playerInventory = player.GetComponent<PlayerInventory>();
        playerCurrentTurret = playerInventory.ActiveTurret.TurretDescription;
        playerTurretButton.AssignChoice(playerCurrentTurret);
    }

    private void CreateChoices()
    {
        List<DepotChoiceSO> choices = new List<DepotChoiceSO>();
        choices.Add(fullRepair);
        List<DepotChoiceSO> possible = new List<DepotChoiceSO>(possibleChoices);
        possible.Remove(playerCurrentTurret);
        for (int i = 0; i < randomChoicesPerDepot; i++)
        {
            DepotChoiceSO dc = possible[UnityEngine.Random.Range(0, possible.Count)];
            possible.Remove(dc);
            choices.Add(dc);
        }
        choices.Add(noChoice);
        activeChoice = noChoice;

        foreach (DepotChoiceSO dc in choices)
        {
            choiceButtons.Add(CreateChoiceButton(dc));
        }
        choiceButtons[choiceButtons.Count - 1].Click();
    }

    private void ClearChoices()
    {
        foreach (UIDepotButton uidb in choiceButtons)
        {
            uidb.OnMouseOver -= Uidb_OnMouseOver;
            uidb.OnMouseExit -= Uidb_OnMouseExit;
            uidb.OnSelected -= Uidb_OnSelected;

            uidb.gameObject.SetActive(false);
            disabledButtons.Add(uidb);
            //ObjectPoolManager.Instance.Deactivate(uidb.gameObject);
        }
        choiceButtons.Clear();
    }

    private void FillInfobox(DepotChoiceSO depotChoiceSOibt)
    {
        ibTitle.text = depotChoiceSOibt.ChoiceName;
        ibText.text = depotChoiceSOibt.Description;
        Vector3 turrStats = depotChoiceSOibt.TurretStats;
        turrPowerSlider.value = turrStats.x;
        turrFirerateSlider.value = turrStats.y;
        turrrTargetSlider.value = turrStats.z;
    }

    private void EmptyInfobox()
    {
        ibTitle.text = "Select a service";
        ibText.text = "Select service to use and click \"Finish\". ";
        turrPowerSlider.value = 0;
        turrFirerateSlider.value = 0;
        turrrTargetSlider.value = 0;
    }

    private void FinalizeChoice()
    {
        Debug.Log(activeChoice.ChoiceType);
        switch (activeChoice.ChoiceType)
        {
            case DepotChoiceType.fullRepair:
                playerHealth.FullHeal();
                playerStatusEffects.Cure(StatusEffectNature.negative);
                break;
            case DepotChoiceType.turretChange:
                Debug.Log("try to swap");
                playerInventory.SwapTurret(activeChoice.TurretPrefab);
                break;
            case DepotChoiceType.noChoice:
                break;
        }
        UIManager.Instance.CloseWindow(this);
    }

    private UIDepotButton CreateChoiceButton(DepotChoiceSO depotChoiceSO)
    {
        UIDepotButton uidb;
        if (disabledButtons.Count == 0)
        {
            GameObject go = ObjectPoolManager.Instance.RequestObject(choiceButtonPrefab);
            go.transform.SetParent(choiceButtonsHolder, false);
            uidb = go.GetComponent<UIDepotButton>();
        }
        else
        {
            uidb = disabledButtons[0];
            disabledButtons.RemoveAt(0);
            uidb.gameObject.SetActive(true);
        }

        uidb.AssignChoice(depotChoiceSO);

        uidb.OnMouseOver += Uidb_OnMouseOver;
        uidb.OnMouseExit += Uidb_OnMouseExit;
        uidb.OnSelected += Uidb_OnSelected;

        return uidb;
    }

    private void Uidb_OnSelected(object sender, UIDepotButton e)
    {
        activeChoice = e.AssignedChoice;
        foreach (UIDepotButton uidb in choiceButtons)
        {
            uidb.SetHighlight(uidb == e);
        }
    }

    private void Uidb_OnMouseExit(object sender, DepotChoiceSO e)
    {
        EmptyInfobox();
    }

    private void Uidb_OnMouseOver(object sender, DepotChoiceSO e)
    {
        FillInfobox(e);
    }

    public override void Show()
    {
        GetPlayerObject();
        CreateChoices();
        gameObject.SetActive(true);
    }
    public override void Hide()
    {
        gameObject.SetActive(false);
        EmptyInfobox();
        ClearChoices();
    }

    public override bool ShouldPause()
    {
        return true;
    }

 
}
