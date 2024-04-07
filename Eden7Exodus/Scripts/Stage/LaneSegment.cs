using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneSegment : MonoBehaviour
{
    public RailSegment StartRail { get { return startRail; } }
    public RailSegment EndRail { get { return endRail; } }
    public bool IsCrossable { get { return isCrossable; } }

    [SerializeField] private RailSegment startRail;
    [SerializeField] private RailSegment endRail;

    [SerializeField] private bool isCrossable = true;
}
