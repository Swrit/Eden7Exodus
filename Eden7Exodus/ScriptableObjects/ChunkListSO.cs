using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ChunkListSO : ScriptableObject
{
    [SerializeField] private List<RoadChunk> roadChunks;
    [SerializeField] private RoadChunk depot;

    public RoadChunk GetRandomChunk()
    {
        return roadChunks[UnityEngine.Random.Range(0, roadChunks.Count)];
    }

    public RoadChunk GetDepotChunk()
    {
        if (depot == null) return GetRandomChunk();
        return depot;
    }
 }
