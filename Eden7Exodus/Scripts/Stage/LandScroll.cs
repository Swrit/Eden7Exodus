using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandScroll : MonoBehaviour
{
    public event EventHandler<float> OnScroll;
    public event EventHandler<float> OnSetScrollSpeed;

    [SerializeField] private float scrollSpeed;
    [SerializeField] private float resetPosThreshold;
    [SerializeField] private float spawnThreshold;
    [SerializeField] private float despawnThreshold;
    [SerializeField] private int depotAfterNumber;
    private int chunksBeforeDepot;
    //[SerializeField] private float resetPosThreshold;

    [SerializeField] private RoadChunk chunkPrefab;
    [SerializeField] private List<RoadChunk> activeChunks = new List<RoadChunk>();
    private List<GameObject> scrollboundObjects = new List<GameObject>();

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private ChunkListSO startChunkList;

    // Start is called before the first frame update
    void Start()
    {
        SetScrollSpeed(scrollSpeed);
        chunksBeforeDepot = depotAfterNumber;

        Vector3 startPoint = new Vector3(0, 0, despawnThreshold);
        GameObject newChunkGO = ObjectPoolManager.Instance.RequestObjectAt(startChunkList.GetRandomChunk().gameObject, startPoint);
        newChunkGO.transform.SetParent(this.transform, true);
        RoadChunk newChunk = newChunkGO.GetComponent<RoadChunk>();
        activeChunks.Add(newChunk);
        while (activeChunks[activeChunks.Count - 1].transform.position.z < spawnThreshold)
        {
            AddChunk(activeChunks[activeChunks.Count - 1], startChunkList);
        }

        ObjectPoolManager.Instance.RequestObjectAt(playerPrefab, Vector3.zero);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float scrollDistance = scrollSpeed * Time.deltaTime;
        if (scrollDistance > 0) Scroll(scrollDistance);
    }

    private void Scroll(float distance)
    {
        transform.position = new Vector3(0, 0, transform.position.z - (distance));
        if (transform.position.z < resetPosThreshold)
        {
            ResetPosition();
        }

        if (activeChunks[0].EndPoint().z < despawnThreshold)
        {
            DespawnChunk();
        }

        if (activeChunks[activeChunks.Count - 1].transform.position.z < spawnThreshold)
        {
            AddChunk(activeChunks[activeChunks.Count - 1]);
        }

        for (int i = scrollboundObjects.Count - 1; i >= 0; i--)
        {
            if (scrollboundObjects[i].transform.position.z < despawnThreshold)
                ObjectPoolManager.Instance.Deactivate(scrollboundObjects[i]);
        }

        OnScroll?.Invoke(this, distance);
    }

    private void ResetPosition()
    {
        foreach (RoadChunk rc in activeChunks)
        {
            rc.transform.SetParent(null, true);
        }
        foreach (GameObject go in scrollboundObjects)
        {
            go.transform.SetParent(null, true);
        }
        transform.position = Vector3.zero;
        foreach (RoadChunk rc in activeChunks)
        {
            rc.transform.SetParent(this.transform, true);
        }
        foreach (GameObject go in scrollboundObjects)
        {
            go.transform.SetParent(this.transform, true);
        }
    }
    private void DespawnChunk(int index = 0)
    {
        //Destroy(activeChunks[index].gameObject);
        ObjectPoolManager.Instance.Deactivate(activeChunks[index].gameObject);
        activeChunks.RemoveAt(index);
    }

    private void AddChunk(RoadChunk lastChunk, ChunkListSO chunkListSO = null)
    {
        if (chunkListSO == null) chunkListSO = lastChunk.CanBeFollowedBy;
        GameObject chunkToSpawn;
        if (chunksBeforeDepot <= 0)
        {
            chunksBeforeDepot = depotAfterNumber;
            chunkToSpawn = chunkListSO.GetDepotChunk().gameObject;
        }
        else
        {
            chunksBeforeDepot--;
            chunkToSpawn = chunkListSO.GetRandomChunk().gameObject;
        }

        GameObject newChunkGO = ObjectPoolManager.Instance.RequestObjectAt(chunkToSpawn, lastChunk.EndPoint());
        newChunkGO.transform.SetParent(this.transform, true);
        RoadChunk newChunk = newChunkGO.GetComponent<RoadChunk>();
        activeChunks.Add(newChunk);
        lastChunk.ConnectToNext(newChunk);
    }
    public void ScrollbindObject(GameObject objectToBind)
    {
        objectToBind.transform.SetParent(this.transform, true);
        scrollboundObjects.Add(objectToBind);
    }

    public void UnbindObject(GameObject objectToUnbind)
    {
        int index = scrollboundObjects.IndexOf(objectToUnbind);
        if (index == -1) return;

        objectToUnbind.transform.SetParent(null, true);
        scrollboundObjects.RemoveAt(index);

    }

    private void SetScrollSpeed(float speed)
    {
        scrollSpeed = speed;
        OnSetScrollSpeed?.Invoke(this, speed);
    }
}
