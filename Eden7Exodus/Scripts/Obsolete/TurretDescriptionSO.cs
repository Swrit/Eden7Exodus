using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TurretDescriptionSO : ScriptableObject
{
    public GameObject TurretObject { get { return turretObject; } }
    public string TurretName { get { return turretName; } }
    public string TurretDescription { get { return turretDescription; } }

    [SerializeField] private GameObject turretObject;
    [SerializeField] private string turretName;
    [SerializeField] private string turretDescription;
}
