using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalBombing : MonoBehaviour, IObjectReset
{
    [SerializeField] private ProjectileFlight projectilePrefab;
    [SerializeField] private GameObject warningMark;
    [SerializeField] private AudioClip warningSound;
    private DeathBound deathBind;
    [SerializeField] private float warningTime;
    [SerializeField] private Transform shootPoint;
    private float projectileSpeed;
    private float shootHeight;
    private float scrollSpeed;
    [SerializeField][Range(0f, 45f)] private float shootAngle = 30f;
    private bool primed = false;
    private float flightTime;
    private bool warningFired = false;
    private bool projectileFired = false;


    public void ResetObject(bool preSpawn = false)
    {
        if (preSpawn) return;
        primed = false;
        warningFired = false;
        projectileFired = false;
        warningMark.SetActive(false);
    }

    private void Start()
    {
        scrollSpeed = GameManager.Instance.CurrentScrollSpeed;
        GameManager.Instance.OnScrollSpeedChanged += GameManager_OnScrollSpeedChanged;

        deathBind = GetComponent<DeathBound>();

        shootHeight = shootPoint.position.y - transform.position.y;
        projectileSpeed = projectilePrefab.Speed;
    }

    private void FixedUpdate()
    {
        if (!primed)
        {
            Calculate();
            primed = true;
            return;
        }

        float timeToZero = transform.position.z / scrollSpeed;
        if ((!warningFired) && (timeToZero <= warningTime)) FireWarning();
        if ((!projectileFired) && (timeToZero <= flightTime)) FireProjectile();

        //Debug.Log(timeToZero + " " + transform.position.z + " " + flightTime + " " + scrollSpeed);
    }

    private void Calculate()
    {
        float flightDistance = shootHeight / Mathf.Cos(shootAngle * Mathf.Deg2Rad);
        flightTime = flightDistance / projectileSpeed;
    }

    private void FireWarning()
    {
        warningFired = true;
        warningMark.SetActive(true);

        if (warningSound!=null) SoundManager.Instance.PlaySound(warningSound);

        
    }
    private void FireProjectile()
    {
        projectileFired = true;
        GameObject projectile = ObjectPoolManager.Instance.RequestObjectAt(projectilePrefab.gameObject, shootPoint.position);
        projectile.GetComponent<ProjectileFlight>().SetDirection(new Vector3(shootPoint.position.x, 0, 0) - shootPoint.position);
        deathBind.DeathBind(projectile.GetComponent<Death>());
    }

    private void GameManager_OnScrollSpeedChanged(object sender, float e)
    {
        scrollSpeed = e;
    }


}
