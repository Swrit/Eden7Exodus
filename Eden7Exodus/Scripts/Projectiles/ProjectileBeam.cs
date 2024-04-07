using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBeam : MonoBehaviour, IObjectReset
{
    public event EventHandler<GameObject> OnHitObject;

    [SerializeField] private Transform beam;
    [SerializeField] private LayerMask hitLayers;
    private float maxDistance = 50f;

    public void ResetObject(bool preSpawn = false)
    {
        beam.localScale = new Vector3(beam.localScale.x, beam.localScale.y, 1);
    }

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance, hitLayers)) 
        {
            beam.localScale = new Vector3(beam.localScale.x, beam.localScale.y, hit.distance);
            OnHitObject?.Invoke(this, hit.collider.gameObject);
        }
        else
        {
            beam.localScale = new Vector3(beam.localScale.x, beam.localScale.y, maxDistance);
        }
    }
    
    public void SetDirection(Vector3 dir)
    {
        transform.forward = dir;
    }
}
