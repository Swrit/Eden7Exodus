using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SpawnableSO : ScriptableObject
{
    public float Rarity { get { return rarity; } }
    public Vector2Int AmountRange { get { return new Vector2Int(minAmount, maxAmount); } }
    public float TimeOutAfterSpawn { get { return timeOutAfterSpawn; } }
    public float CanSpawnAfterDistance { get { return canSpawnAfterDistance; } }

    [SerializeField] [Range(1, 3)] private int minAmount;
    [SerializeField] [Range(1, 3)] private int maxAmount;

    [SerializeField] private float rarity = 1f;

    [SerializeField] private bool canSpawnOnUnswitchableRail = true;
    [SerializeField] private List<RailType> canSpawnOnRailTypes = new List<RailType>();
    [SerializeField] private float timeOutAfterSpawn = 0f;
    [SerializeField] private float canSpawnAfterDistance = 0f;

    [SerializeField] private List<GameObject> spawnablePrefabs = new List<GameObject>();

    public GameObject GetPrefabToSpawn()
    {
        if (spawnablePrefabs.Count == 1) return spawnablePrefabs[0];
        //if (spawnablePrefabs.Count == 0) return null;

        int index = UnityEngine.Random.Range(0, spawnablePrefabs.Count);
        return spawnablePrefabs[index];
    }

    public bool RailSuitable(RailSegment railSegment)
    {
        if (!railSegment.CanSpawnEnemies) return false;
        if (!canSpawnOnUnswitchableRail && !railSegment.CanSwitchRails) return false;
        if (!canSpawnOnRailTypes.Contains(railSegment.RailType)) return false;

        return true;
    }
}
