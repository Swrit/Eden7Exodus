using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerHealthWindow : MonoBehaviour
{
    [SerializeField] private Slider mainSlider;
    [SerializeField] private Slider plusSlider;
    [SerializeField] private Slider minusSlider;
    [SerializeField] private TextMeshProUGUI percentText;
    [SerializeField] private bool instantSetOnEnable = false;

    private float trueHealthValue = 1f;
    private float valueChangeSpeed = 0.5f;
    private float percentShowing = 1f;

    private void Start()
    {
        UIManager.Instance.OnHealthChange += UIManager_OnHealthChange;
    }

    private void Update()
    {
        MoveSliderToTrueValue(mainSlider);
        MoveSliderToTrueValue(minusSlider);
        MoveSliderToTrueValue(plusSlider);
        UpdatePercentage();
    }

    private void UIManager_OnHealthChange(object sender, float e)
    {
        if (e < mainSlider.value) mainSlider.value = e;
        if (e != plusSlider.value) plusSlider.value = e;

        trueHealthValue = e;
    }

    private void MoveSliderToTrueValue(Slider slider)
    {
        slider.value = MoveToTrueValue(slider.value);
    }

    private float MoveToTrueValue(float varToChange)
    {
        if (varToChange == trueHealthValue) return trueHealthValue;

        return Mathf.MoveTowards(varToChange, trueHealthValue, valueChangeSpeed * Time.deltaTime);
    }

    private void UpdatePercentage(bool setToTruth = false)
    {
        if (percentShowing == trueHealthValue) return;
        percentShowing =  setToTruth ? trueHealthValue : MoveToTrueValue(percentShowing);
        percentText.text = (int)Mathf.Ceil(percentShowing * 100f) + "%";
    }

    private void OnEnable()
    {
        if (instantSetOnEnable)
        {
            mainSlider.value = trueHealthValue;
            plusSlider.value = trueHealthValue;
            minusSlider.value = trueHealthValue;
            UpdatePercentage(true);
        }
    }
}
