using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<SpawnableSO> spawnableSOs = new List<SpawnableSO>();
    private List<SpawnableSO> cantSpawnYet = new List<SpawnableSO>();

    [Serializable]
    private class Spawnable
    {
        public SpawnableSO spawnableSO;
        public float timeOut = 0f;

        public Spawnable(SpawnableSO sso)
        {
            spawnableSO = sso;
        }

        public void TimerCountdown(float deltaTime)
        {
            if (timeOut > 0) timeOut -= deltaTime;
        }

        public void ResetTimer()
        {
            timeOut = spawnableSO.TimeOutAfterSpawn;
        }
    }
    private List<Spawnable> spawnables = new List<Spawnable>();

    private float spawnTimer = 5f;
    [SerializeField] private float waitOnFailedSpawn = 0.5f;
    [SerializeField] private Vector2 waitOnSuccessfulSpawnRange;
    private float distanceTraveled = 0f;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnDistanceTraveledInKm += GameManager_OnDistanceTraveledInKm;

        foreach (SpawnableSO sso in spawnableSOs)
        {
            cantSpawnYet.Add(sso);
        }
    }

    private void GameManager_OnDistanceTraveledInKm(object sender, float e)
    {
        distanceTraveled = e;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSpawnables();

        if (spawnTimer <= 0)
        {
            spawnTimer = TrySpawn() ? GetTimerWaitTime() : waitOnFailedSpawn;
        }
        else spawnTimer -= Time.deltaTime;
    }

    private bool TrySpawn()
    {
        List<RailSegment> rails = ScanRails();
        List<Spawnable> spawnPool = new List<Spawnable>();
        float selection = 0f;
        foreach (Spawnable s in spawnables)
        {
            if (s.timeOut > 0) continue;
            if (GetSuitableRails(rails, s.spawnableSO).Count < s.spawnableSO.AmountRange.x) continue;

            spawnPool.Add(s);
            selection += s.spawnableSO.Rarity;
        }
        if (spawnPool.Count == 0) return false;

        selection = UnityEngine.Random.Range(0, selection);
        foreach (Spawnable s in spawnPool)
        {
            selection -= s.spawnableSO.Rarity;
            if (selection > 0) continue;
            Spawn(s, rails);
            break;
        }
        return true;
    }

    private void Spawn(Spawnable spawnable, List<RailSegment> rails)
    {
        SpawnableSO sso = spawnable.spawnableSO;
        List<RailSegment> suitableRails = GetSuitableRails(rails, sso);
        int amount = 0;
        if (sso.AmountRange.x == sso.AmountRange.y) amount = sso.AmountRange.x;
        else
        {
            int minAmount = sso.AmountRange.x;
            int maxAmount = Mathf.Min(sso.AmountRange.y, suitableRails.Count);
            amount = UnityEngine.Random.Range(minAmount, maxAmount + 1);
        }
        
        for (int i = 0; i < amount; i++)
        {
            int railIndex = UnityEngine.Random.Range(0, suitableRails.Count);
            Vector3 pos = suitableRails[railIndex].GetObjectPositionAtZ(transform.position);
            ObjectPoolManager.Instance.RequestObjectAt(sso.GetPrefabToSpawn(), pos, true);
            suitableRails.RemoveAt(railIndex);
        }
        spawnable.ResetTimer();
    }

    private float GetTimerWaitTime()
    {
        return UnityEngine.Random.Range(waitOnSuccessfulSpawnRange.x, waitOnSuccessfulSpawnRange.y);
    }

    private List<RailSegment> ScanRails()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector3.right);
        List<RailSegment> railSegmentsFound = new List<RailSegment>();
        foreach (RaycastHit rch in hits)
        {
            RailCollider rs = rch.collider.GetComponent<RailCollider>();
            if (rs != null) railSegmentsFound.Add(rs.RailSegment);
        }
        return railSegmentsFound;
    }

    private bool RailSuitable(RailSegment railSegment, SpawnableSO sso)
    {
        return sso.RailSuitable(railSegment);
    }

    private List<RailSegment> GetSuitableRails(List<RailSegment> lrs, SpawnableSO sso)
    {
        List<RailSegment> suitableRails = new List<RailSegment>();
        foreach (RailSegment rs in lrs)
        {
            if (RailSuitable(rs, sso)) suitableRails.Add(rs);
        }
        return suitableRails;
    }

    private void UpdateSpawnables()
    {
        if (cantSpawnYet.Count > 0)
        {
            for (int i = cantSpawnYet.Count - 1; i >= 0; i--)
            {
                if (distanceTraveled > cantSpawnYet[i].CanSpawnAfterDistance)
                {
                    spawnables.Add(new Spawnable(cantSpawnYet[i]));
                    cantSpawnYet.RemoveAt(i);
                }
            }
        }
        foreach (Spawnable s in spawnables)
        {
            s.TimerCountdown(Time.deltaTime);
        }
    }
}