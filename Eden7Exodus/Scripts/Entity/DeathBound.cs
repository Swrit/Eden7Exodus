using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBound : MonoBehaviour, IObjectReset
{
    private List<Death> deathBinders = new List<Death>();
    private Death ownDeath;

    public void ResetObject(bool preSpawn = false)
    {
        if (preSpawn) return;
        Unbind();
    }

    private void Start()
    {
        ownDeath = GetComponent<Death>();
    }
    public void DeathBind(Death death)
    {
        if (death != null) death.OnDeath += Death_OnDeath;
    }

    private void Death_OnDeath(object sender, Death.OnDeathArgs e)
    {
        Unbind();
        Die();
    }

    private void Unbind()
    {
        foreach (Death d in deathBinders)
        {
            d.OnDeath -= Death_OnDeath;
        }
        deathBinders.Clear();
    }

    private void Die()
    {
        if (ownDeath != null) ownDeath.Die();
        else ObjectPoolManager.Instance.Deactivate(gameObject);
    }

}
