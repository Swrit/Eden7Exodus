using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretRapidFire : TurretBase, IObjectReset
{
    [Serializable]
    private struct ShootPointWithTrajectory
    {
        public Transform shootPoint;
        public LineRenderer trajectoryLine;
    }

    [SerializeField] private List<ShootPointWithTrajectory> shootPointWithTrajectories = new List<ShootPointWithTrajectory>();
    [SerializeField] private GameObject reticle;
    private float projGravity;
    private float projStartSpeed;

    private bool isShooting = false;

    [SerializeField] private float reloadTime = 0.5f;
    [SerializeField] private float offsetY = 0.1f;
    [SerializeField] private float angleLimit = 10f;
    protected float reloadTimer = 0f;

    public void ResetObject(bool preSpawn = false)
    {
        if (preSpawn) return;
        reloadTimer = 0f;
        targetPoint = Vector3.zero;
    }

    private void Start()
    {
        ProjectileFlight pf = projectilePrefab.GetComponent<ProjectileFlight>();
        projGravity = pf.Gravity;
        projStartSpeed = pf.Speed;
    }

    private void Update()
    {
        if (reloadTimer > 0) reloadTimer -= Time.deltaTime;
        if (isShooting && reloadTimer <= 0)
        {
            Shoot();
            reloadTimer = reloadTime;
        }
        
    }

    private void Shoot()
    {
        foreach(ShootPointWithTrajectory spwt in shootPointWithTrajectories)
        {
            ProjectileFlight pj = ObjectPoolManager.Instance.RequestObjectAt(projectilePrefab.gameObject, spwt.shootPoint.position).GetComponent<ProjectileFlight>();
            pj.SetDirection(turretGun.forward);
        }
        if (shootSound != null) SoundManager.Instance.PlaySound(shootSound);

    }

    public override void AimAt(Vector3 targetPos)
    {
        targetPoint = targetPos;
        targetPoint.y += offsetY;
        RotateTurretBase(targetPoint);
        AngleGun(targetPoint);
        DrawTrajectory();
        reticle.transform.position = targetPoint;
    }

    public override void StartShooting()
    {
        isShooting = true;
        HideTrajectory();
    }

    public override void StopShooting()
    {
        isShooting = false;
        ShowTrajectory();
    }

    public override void DeactivateTurret()
    {
        StopShooting();
        HideTrajectory();
        reticle.gameObject.SetActive(false);
    }

    public override void ActivateTurret()
    {
        ShowTrajectory();
        reticle.gameObject.SetActive(true);
    }

    private void AngleGun(Vector3 targetPos)
    {
        float sin = (targetPos.y - turretGun.position.y)/ Vector3.Distance(targetPos, turretGun.position);
        float angle = Mathf.Asin(sin) * Mathf.Rad2Deg;
        if (float.IsNaN(angle)) angle = 0;
        angle = Mathf.Clamp(angle, -angleLimit, angleLimit);
        turretGun.localRotation = Quaternion.Euler(-angle, turretGun.localRotation.y, turretGun.localRotation.z);
    }

    protected void DrawTrajectory()
    {
        foreach (ShootPointWithTrajectory spwt in shootPointWithTrajectories)
        {
            Vector3[] trajectory = new Vector3[2];
            Vector3 offset = spwt.shootPoint.position - turretGun.position;
            trajectory[0] = spwt.shootPoint.position;
            trajectory[1] = spwt.shootPoint.position + ((targetPoint - turretGun.position) * 2);
            spwt.trajectoryLine.positionCount = trajectory.Length;
            spwt.trajectoryLine.SetPositions(trajectory);
        }
    }

    private void HideTrajectory()
    {
        foreach (ShootPointWithTrajectory spwt in shootPointWithTrajectories)
        {
            spwt.trajectoryLine.gameObject.SetActive(false);
        }
    }

    private void ShowTrajectory()
    {
        foreach (ShootPointWithTrajectory spwt in shootPointWithTrajectories)
        {
            spwt.trajectoryLine.gameObject.SetActive(true);
        }
    }
}
