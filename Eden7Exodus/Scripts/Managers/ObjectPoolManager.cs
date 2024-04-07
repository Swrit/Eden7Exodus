using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { private set; get; }

    private Dictionary<int, List<GameObject>> activePool = new Dictionary<int, List<GameObject>>();
    private Dictionary<int, List<GameObject>> inactivePool = new Dictionary<int, List<GameObject>>();

    [SerializeField] private LandScroll landScroll;

    //private Dictionary<GameObject, List<Transform>> holders = new Dictionary<GameObject, List<Transform>>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public GameObject RequestObject(GameObject prefab)
    {
        if (prefab == null) return null;

        GameObject returnObject = GetObject(prefab);
        returnObject.SetActive(true);
        return returnObject;
    }

    public GameObject RequestObjectAt(GameObject prefab, Vector3 position, bool scrollbound = false)
    {
        if (prefab == null) return null;

        GameObject returnObject = GetObject(prefab);
        returnObject.transform.position = position;
        if (scrollbound) landScroll.ScrollbindObject(returnObject);
        returnObject.SetActive(true);
        return returnObject;
    }

    private GameObject GetObject(GameObject prefab)
    {
        int prefabID = prefab.GetInstanceID();

        CreateDictionaryKey(prefabID);

        int inactivePoolCount = inactivePool[prefabID].Count;

        GameObject returnObject;
        if (inactivePoolCount > 0)
        {
            returnObject = inactivePool[prefabID][inactivePoolCount - 1];
            inactivePool[prefabID].Remove(returnObject);
            activePool[prefabID].Add(returnObject);
        }
        else
        {
            returnObject = Instantiate(prefab);
            activePool[prefabID].Add(returnObject);
        }

        ResetObject(returnObject, true);
        returnObject.transform.SetParent(null);
        return returnObject;
    }

    public void Deactivate(GameObject objectToDeactivate)
    {
        if (AlreadyInactive(objectToDeactivate)) return;
        if (objectToDeactivate == null) return;

        ResetObject(objectToDeactivate);
        landScroll.UnbindObject(objectToDeactivate);
        objectToDeactivate.SetActive(false);
        objectToDeactivate.transform.SetParent(this.transform);
        int prefabID = GetPrefabKey(objectToDeactivate);

        if (prefabID == 0)
        {
            Destroy(objectToDeactivate);
            return;
        }

        activePool[prefabID].Remove(objectToDeactivate);
        inactivePool[prefabID].Add(objectToDeactivate);
    }

    private void CreateDictionaryKey(int prefabID)
    {
        if (!activePool.ContainsKey(prefabID)) activePool[prefabID] = new List<GameObject>();
        if (!inactivePool.ContainsKey(prefabID)) inactivePool[prefabID] = new List<GameObject>();
    }

    private void ResetObject(GameObject objectToReset, bool preSpawn = false)
    {
        IObjectReset[] resets = objectToReset.GetComponentsInChildren<IObjectReset>();
        foreach (IObjectReset ior in resets) ior.ResetObject(preSpawn);
    }

    private int GetPrefabKey(GameObject objectToFind)
    {
        foreach (KeyValuePair<int, List<GameObject>> entry in activePool)
        {
            if (entry.Value.Contains(objectToFind)) return entry.Key;
        }
        return 0;
    }

    private bool AlreadyInactive(GameObject objectToFind)
    {
        foreach (KeyValuePair<int, List<GameObject>> entry in inactivePool)
        {
            if (entry.Value.Contains(objectToFind)) return true;
        }
        return false;
    }
}
