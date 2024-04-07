using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffects : MonoBehaviour, IObjectReset
{
    public event EventHandler<List<StatusEffectAction>> OnStatusEffectChange;

    private List<StatusEffectInflicted> inflictedStatusEffects = new List<StatusEffectInflicted>();
    private List<StatusEffectAction> activeEffectActions = new List<StatusEffectAction>();

    public void ResetObject(bool preSpawn = false)
    {
        if (preSpawn) return;
        inflictedStatusEffects.Clear();
        UpdateActionsList();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimers();
    }

    private void UpdateTimers()
    {
        bool listChanged = false;
        for (int i = inflictedStatusEffects.Count - 1; i >= 0; i--)
        {
            if (inflictedStatusEffects[i].TimerRanOut(Time.deltaTime))
            {
                inflictedStatusEffects.RemoveAt(i);
                listChanged = true;
            }
        }
        if (listChanged) UpdateActionsList();
    }

    public void Inflict (StatusEffectInflicted newStatus)
    {
        //Find if already inflicted and cumulate
        foreach (StatusEffectInflicted sei in inflictedStatusEffects)
        {
            if (sei.StatusEffect == newStatus.StatusEffect)
            {
                sei.Cumulate(newStatus);
                return;
            }
        }
        //If not yet inflicted
        inflictedStatusEffects.Add(new StatusEffectInflicted(newStatus));
        UpdateActionsList();
    }

    public void Cure(StatusEffectSO statusEffectSO)
    {
        bool listChanged = false;
        for (int i = inflictedStatusEffects.Count - 1; i >= 0; i--)
        {
            if (inflictedStatusEffects[i].StatusEffect == statusEffectSO)
            {
                inflictedStatusEffects.RemoveAt(i);
                listChanged = true;
                break;
            }
        }
        if (listChanged) UpdateActionsList();
    }

    public void Cure(StatusEffectNature effectNature)
    {
        bool listChanged = false;
        for (int i = inflictedStatusEffects.Count - 1; i >= 0; i--)
        {
            if (inflictedStatusEffects[i].StatusEffect.Nature == effectNature)
            {
                inflictedStatusEffects.RemoveAt(i);
                listChanged = true;
            }
        }
        if (listChanged) UpdateActionsList();
    }

    private void UpdateActionsList()
    {
        activeEffectActions = new List<StatusEffectAction>();
        foreach (StatusEffectInflicted sei in inflictedStatusEffects)
        {
            foreach (StatusEffectAction sea in sei.StatusEffect.Actions)
            {
                if (!activeEffectActions.Contains(sea)) activeEffectActions.Add(sea);
            }
        }
        OnStatusEffectChange?.Invoke(this, activeEffectActions);
    }


}
