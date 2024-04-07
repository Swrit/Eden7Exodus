using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadChunk : MonoBehaviour, IObjectReset
{
    public ChunkListSO CanBeFollowedBy { get { return canBeFollowedBy; } }

    [Serializable]
    private class Lane
    {
        [SerializeField] public List<LaneSegment> laneSegmentOptions = new List<LaneSegment>();
        public int index;
        public bool hasCrossable = false;
        public bool hasUncrossable = false;
        public bool single;
        public void Analyze()
        {
            single = (laneSegmentOptions.Count < 2);
            foreach (LaneSegment ls in laneSegmentOptions)
            {
                if (ls.IsCrossable) hasCrossable = true;
                else hasUncrossable = true;
                ls.gameObject.SetActive(false);
            }
        }

        public bool TryGetWithCrossability (bool crossable, out LaneSegment segment)
        {
            if ((crossable & !hasCrossable) || (!crossable & !hasUncrossable))
            {
                segment = GetAny();
                return false;
            }

            List<LaneSegment> options = new List<LaneSegment>();
            foreach (LaneSegment ls in laneSegmentOptions)
            {
                if (ls.IsCrossable == crossable) options.Add(ls);
            }
            segment = GetAny(options);
            return true;
        }

        public LaneSegment GetAny()
        {
            return GetAny(laneSegmentOptions);
        }

        private LaneSegment GetAny(List<LaneSegment>laneChoices)
        {
            return laneChoices[UnityEngine.Random.Range(0, laneChoices.Count)];
        }
    }

    [SerializeField] private Lane[] lanes = new Lane[3];
    private LaneSegment[] activeLanes = new LaneSegment[3];
    [SerializeField] private bool mustHaveCrossable = true;
    [SerializeField] private bool mustHaveUncrossable = false;
    private bool lanesAnalyzed = false;
    private bool randomizable = true;

    [SerializeField] private ChunkListSO canBeFollowedBy;

    public void ResetObject(bool preSpawn = false)
    {
        AnalyzeLanes();
        Randomize();
    }

    public Vector3 EndPoint()
    {
        Vector3 result = transform.position;
        foreach (LaneSegment ls in activeLanes)
        {
            float railEndZ = ls.EndRail.EndPoint.position.z;
            if (railEndZ > result.z) result.z = railEndZ;
        }
        return result;
    }

    public void ConnectToNext(RoadChunk nextChunk)
    {

        for (int i = 0; i < activeLanes.Length; i++)
        {
            activeLanes[i].EndRail.ConnectToNext(nextChunk.activeLanes[i].StartRail);
        }
    }

    private void Randomize()
    {
        if (!randomizable) return;

        //Clear current lanes
        foreach (LaneSegment ls in activeLanes)
        {
            if (ls == null) continue;
            ls.gameObject.SetActive(false);
        }
        activeLanes = new LaneSegment[activeLanes.Length];
        List<int> randomizableLaneIndexes = new List<int>();

        //Set single-choice lanes if any, collect randomizable lane indexes
        bool gotCrossable = false;
        bool gotUncrossable = false;
        for (int i = 0; i < lanes.Length; i++)
        {
            if (lanes[i].single)
            {
                SetActiveLane(i, lanes[i].laneSegmentOptions[0]);
                if (activeLanes[i].IsCrossable) gotCrossable = true;
                else gotUncrossable = true;
            }
            else
            {
                randomizableLaneIndexes.Add(i);
            }
        }
        if (randomizableLaneIndexes.Count == 0) return;
        
        //Randomize indexes order
        for (int i = randomizableLaneIndexes.Count - 1; i > 0; i--)
        {
            int temp = randomizableLaneIndexes[i];
            int swapPlace = UnityEngine.Random.Range(0, i + 1);
            randomizableLaneIndexes[i] = randomizableLaneIndexes[swapPlace];
            randomizableLaneIndexes[swapPlace] = temp;
        }

        //Set randomized lanes
        foreach (int i in randomizableLaneIndexes)
        {
            if (mustHaveCrossable && !gotCrossable)
            {
                gotCrossable = lanes[i].TryGetWithCrossability(true, out LaneSegment ls);
                SetActiveLane(i, ls);
                continue;
            }
            if (mustHaveUncrossable && !gotUncrossable)
            {
                gotCrossable = lanes[i].TryGetWithCrossability(false, out LaneSegment ls);
                SetActiveLane(i, ls);
                continue;
            }
            SetActiveLane(i, lanes[i].GetAny());
        }

    }

    private void AnalyzeLanes()
    {
        if (lanesAnalyzed) return;

        int singles = 0;

        for (int i = 0; i < lanes.Length; i++)
        {
            lanes[i].Analyze();
            lanes[i].index = i;
            if (lanes[i].single) 
            {
                SetActiveLane(i, lanes[i].laneSegmentOptions[0]);
                singles++;
            }
        }
        if (singles == lanes.Length) randomizable = false;

        lanesAnalyzed = true;
    }

    private void SetActiveLane(int index, LaneSegment laneSegment)
    {
        activeLanes[index] = laneSegment;
        activeLanes[index].gameObject.SetActive(true);
    }

}
