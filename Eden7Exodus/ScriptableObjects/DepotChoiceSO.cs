using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DepotChoiceType
{
    fullRepair,
    turretChange,
    noChoice,
}

[CreateAssetMenu()]
public class DepotChoiceSO : ScriptableObject
{
    public DepotChoiceType ChoiceType { get { return choiceType; } }
    public string ChoiceName { get { return choiceName; } }
    public string Description { get { return description; } }
    public GameObject TurretPrefab { get { return turretPrefab; } }
    public Vector3 TurretStats { get { return new Vector3(turrPower / 5f, turrFirerate / 5f, turrTarget / 5f); } }

    [SerializeField] private DepotChoiceType choiceType;
    [SerializeField] private string choiceName;
    [SerializeField] private string description;
    [SerializeField] private GameObject turretPrefab;
    [SerializeField] [Range(0, 5)] private int turrPower;
    [SerializeField] [Range(0, 5)] private int turrFirerate;
    [SerializeField] [Range(0, 5)] private int turrTarget;
}
