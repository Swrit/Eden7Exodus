using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IObjectReset
{
    public event EventHandler<Vector2> OnHealthChange;

    [SerializeField] private float maxHealth = 5f;
    [SerializeField] private float currentHealth;

    private bool vulnerable = true;

    public void ResetObject(bool preSpawn = false)
    {
        if (!preSpawn) return;
        ChangeHealth(maxHealth);
    }

    public bool TryGetHit(float damage) //EntityType entityType)
    {
        if (vulnerable) ChangeHealth(currentHealth - damage);
        return true;
    }

    public void Heal(float heal)
    {
        ChangeHealth(currentHealth + heal);
    }

    public void FullHeal()
    {
        ChangeHealth(maxHealth);
    }

    private void ChangeHealth(float setHealth)
    {
        float newHealth = Mathf.Clamp(setHealth, 0, maxHealth);
        if (newHealth != currentHealth)
        {
            currentHealth = newHealth;
            //float ratio = (float)currentHealth / maxHealth;
            OnHealthChange?.Invoke(this, new Vector2(currentHealth, maxHealth));
        }

    }

    public void SetVulnerability(bool vulnerability)
    {
        vulnerable = vulnerability;
    }
}
