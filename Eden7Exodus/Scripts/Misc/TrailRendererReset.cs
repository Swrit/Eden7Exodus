using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailRendererReset : MonoBehaviour, IObjectReset
{
    private TrailRenderer trailRenderer;

    private void Awake()
    {
        trailRenderer = GetComponent<TrailRenderer>();
    }

    public void ResetObject(bool preSpawn = false)
    {
        if (trailRenderer != null) trailRenderer.Clear();
    }

}
