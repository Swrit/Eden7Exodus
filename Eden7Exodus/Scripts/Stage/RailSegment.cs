using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailSegment : MonoBehaviour, IObjectReset
{
    public event EventHandler<RailSegment> OnRailDespawned;

    public Transform EndPoint { get { return endPoint; } }
    public bool EmitDustTrail { get { return emitDustTrail; } }
    public bool CanSwitchRails { get { return canSwitchRails; } }
    public bool CanSpawnEnemies { get { return canSpawnEnemies; } }
    public bool CallDepot { get { return callDepot; } }
    public RailType RailType { get { return railType; } }

    [SerializeField] private RailType railType;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private RailSegment nextRail;
    [SerializeField] private RailSegment previousRail;
    [SerializeField] private bool canSwitchRails;
    [SerializeField] private bool emitDustTrail;
    [SerializeField] private bool canSpawnEnemies;
    [SerializeField] private bool callDepot = false;

    public void ResetObject(bool preSpawn = false)
    {
        if (preSpawn) return;
        OnRailDespawned?.Invoke(this, this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        if (startPoint != null && endPoint != null)
            Gizmos.DrawLine(startPoint.position, endPoint.position);
    }

    public Vector3 GetVector()
    {
        return (endPoint.position - startPoint.position).normalized;
    }

    public RailSegment GetCurrentSegment(Vector3 objectPos, int dir)
    {
        if (dir < 0)
        {
            if (objectPos.z > startPoint.position.z) return this;
            else return previousRail;
        }
        else
        {
            if (objectPos.z < endPoint.position.z) return this;
            else return nextRail;
        }
    }

    public Vector3 GetObjectPositionAtZ(Vector3 objectPos)
    {
        Vector3 deltaRail = endPoint.position - startPoint.position;
        float deltaZ = (objectPos.z - startPoint.position.z) / deltaRail.z;
        Vector3 result = (startPoint.position + (deltaRail * deltaZ));
        result.z = objectPos.z;
        return result;
    }
    public void ConnectToNext(RailSegment nextRail, bool interChunkConnection = true)
    {
        this.nextRail = nextRail;
        if (nextRail == null) return;

        nextRail.previousRail = this;
        if (interChunkConnection)
        {
            OnRailDespawned += nextRail.RailSegment_OnRailDespawned;
        }
    }

    private void RailSegment_OnRailDespawned(object sender, RailSegment e)
    {
        e.OnRailDespawned -= RailSegment_OnRailDespawned;
        if (e == previousRail) previousRail = null;
    }


}

public enum RailType
{
    road,
    bridge,
}
