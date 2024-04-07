using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIOverlay : UIWindowBase
{
    [SerializeField] private GameObject EMPEffect;
    [SerializeField] private GameObject CriticalEffect;
    [SerializeField] [Range(0, 1)] private float CriticalEffectBelow = 0.2f;

    [SerializeField] private TextMeshProUGUI distanceText;

    // Start is called before the first frame update
    void Start()
    {
        EMPEffect.SetActive(false);
        CriticalEffect.SetActive(false);

        UIManager.Instance.OnHealthChange += UIManager_OnHealthChange;
        UIManager.Instance.OnStatusEffectChange += UIManager_OnStatusEffectChange;
        UIManager.Instance.OnTravelDistance += UIManager_OnTravelDistance;
    }

    private void UIManager_OnHealthChange(object sender, float e)
    {
        if (e <= CriticalEffectBelow) CriticalEffect.SetActive(true);
        else CriticalEffect.SetActive(false);
    }
    private void UIManager_OnStatusEffectChange(object sender, List<StatusEffectAction> e)
    {
        if (e.Contains(StatusEffectAction.EMPSparkles)) EMPEffect.SetActive(true);
        else EMPEffect.SetActive(false);
    }
    private void UIManager_OnTravelDistance(object sender, float e)
    {
        distanceText.text = e.ToString("F2") + "km";
    }

    public override void Show()
    {
        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }

    public override bool ShouldPause()
    {
        return false;
    }
}
