using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailCollider : MonoBehaviour
{
    public RailSegment RailSegment { get { return railSegment; } }

    [SerializeField] private RailSegment railSegment;
    
}
