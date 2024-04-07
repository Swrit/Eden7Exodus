using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEnemyHealthBar : MonoBehaviour, IObjectReset
{

    [SerializeField] private GameObject enemy;
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject canvas;

    public void ResetObject(bool preSpawn = false)
    {
        slider.value = 1f;
        if (preSpawn) return;

        Hide();
    }
    // Start is called before the first frame update
    void Start()
    {
        Health health = enemy.GetComponent<Health>();
        if (health != null) health.OnHealthChange += Health_OnHealthChange;
        Hide();
    }

    private void Health_OnHealthChange(object sender, Vector2 e)
    {
        float hp = e.x / e.y;
        slider.value = hp;
        if ((hp == 1) || (hp == 0)) Hide();
        else Show();
    }

    private void Hide()
    {
        canvas.SetActive(false);
    }

    private void Show()
    {
        canvas.SetActive(true);
    }


}
