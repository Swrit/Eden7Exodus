using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDepotButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public event EventHandler<DepotChoiceSO> OnMouseOver;
    public event EventHandler<DepotChoiceSO> OnMouseExit;
    public event EventHandler<UIDepotButton> OnSelected;

    public DepotChoiceSO AssignedChoice { get { return assignedChoice; } }

    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private GameObject highlight;
    private DepotChoiceSO assignedChoice;
    [SerializeField] private bool clickable;
    [SerializeField] private Button button;

    private void Start()
    {
        button.onClick.AddListener(Click);
        highlight.SetActive(false);
    }

    public void AssignChoice(DepotChoiceSO depotChoiceSO)
    {
        assignedChoice = depotChoiceSO;
        buttonText.text = depotChoiceSO.ChoiceName;
    }

    public void Click()
    {
        if (!clickable) return;
        OnSelected?.Invoke(this, this);
    }

    public void SetHighlight(bool setting)
    {
        highlight.SetActive(setting);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnMouseOver?.Invoke(this, assignedChoice);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnMouseExit?.Invoke(this, assignedChoice);
    }
}
