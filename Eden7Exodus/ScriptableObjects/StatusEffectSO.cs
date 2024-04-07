using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class StatusEffectSO : ScriptableObject
{
    public StatusEffectNature Nature { get { return nature; } }
    public List<StatusEffectAction> Actions { get { return actions; } }
    public StatusEffectCumulativeEffect CumulativeEffect { get { return cumulativeEffect; } }
    public bool IsTimed { get { return isTimed; } }

    [SerializeField] private string statusName;
    [SerializeField] private StatusEffectNature nature;
    [SerializeField] private List<StatusEffectAction> actions = new List<StatusEffectAction>();
    [SerializeField] private StatusEffectCumulativeEffect cumulativeEffect;
    [SerializeField] private bool isTimed;
}
public enum StatusEffectNature
{
    positive,
    negative,
    neutral,
}

public enum StatusEffectAction
{
    disableControls,
    disableWeapon,
    EMPSparkles,
}
public enum StatusEffectCumulativeEffect
{
    add,
    refresh,
    none,
}
