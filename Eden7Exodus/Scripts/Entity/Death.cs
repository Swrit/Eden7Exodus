using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour, IObjectReset
{
    public event EventHandler<OnDeathArgs> OnDeath;
    public class OnDeathArgs : EventArgs
    {
        public GameObject spawnedOnDeath;
    }
    public bool IsDead { get { return isDead; } }

    [SerializeField] private GameObject spawnOnDeath;
    [SerializeField] private Transform spawnOnDeathPoint;
    private LayerMask killPlane;
    private bool isDead = false;

    public void ResetObject(bool preSpawn = false)
    {
        isDead = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        killPlane = (1 << LayerMask.NameToLayer("KillPlane"));

        Health health = GetComponent<Health>();
        if (health!=null) health.OnHealthChange += Health_OnHealthChange;

        if (spawnOnDeathPoint == null) spawnOnDeathPoint = this.transform;
    }

    private void Health_OnHealthChange(object sender, Vector2 e)
    {
        if (e.x == 0) Die();
    }

    public void Die()
    {
        GameObject deathSpawned = null;
        if (spawnOnDeath != null)
        {
            deathSpawned = ObjectPoolManager.Instance.RequestObjectAt(spawnOnDeath, spawnOnDeathPoint.position, true);
        }
        Erase(deathSpawned);
    }

    private void Erase(GameObject deathSpawned = null)
    {
        isDead = true;
        OnDeath?.Invoke(this, new OnDeathArgs { spawnedOnDeath = deathSpawned });
        ObjectPoolManager.Instance.Deactivate(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (killPlane.value == (killPlane.value | (1 << other.gameObject.layer)))
        {
            Debug.Log(gameObject.name + " death by killplane");
            Erase();
        }
    }


}
