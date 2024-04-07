using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretArtillery : TurretBase, IObjectReset
{
    [SerializeField] private LineRenderer trajectoryLine;
    [SerializeField] private GameObject reticle;
    [SerializeField] private GameObject targetMarkPrefab;
    private float projGravity;
    private float projStartSpeed;

    [SerializeField] private float reloadTime = 0.5f;
    protected float reloadTimer = 0f;

    public void ResetObject(bool preSpawn = false)
    {
        if (preSpawn) 
        {
            ProjectileFlight pf = projectilePrefab.GetComponent<ProjectileFlight>();
            projGravity = pf.Gravity;
            projStartSpeed = pf.Speed;
            return; 
        }
        reloadTimer = 0f;
        targetPoint = Vector3.zero;
    }

    private void Update()
    {
        if (reloadTimer > 0) reloadTimer -= Time.deltaTime;
    }

    public override void AimAt(Vector3 targetPos)
    {
        targetPoint = targetPos;
        RotateTurretBase(targetPoint);
        AngleGun(targetPoint);
        DrawTrajectory();
        reticle.transform.position = targetPoint;
    }

    public override void StartShooting()
    {
        if (reloadTimer > 0) return;

        Shoot();
        reloadTimer = reloadTime;
    }

    private void Shoot()
    {
        ProjectileFlight pj = ObjectPoolManager.Instance.RequestObjectAt(projectilePrefab.gameObject, turretGun.position, true).GetComponent<ProjectileFlight>();
        pj.SetDirection(turretGun.forward);

        DeathBound tm = ObjectPoolManager.Instance.RequestObjectAt(targetMarkPrefab, targetPoint, true).GetComponent<DeathBound>();
        tm.DeathBind(pj.GetComponent<Death>());

        if (shootSound != null) SoundManager.Instance.PlaySound(shootSound);
    }

    public override void StopShooting()
    {
        //not applicable for this turret
    }

    public override void DeactivateTurret()
    {
        trajectoryLine.gameObject.SetActive(false);
        reticle.gameObject.SetActive(false);
    }

    public override void ActivateTurret()
    {
        trajectoryLine.gameObject.SetActive(true);
        reticle.gameObject.SetActive(true);
    }

    private void AngleGun(Vector3 targetPos)
    {
        //Debug.Log(targetPos + " " + turretGun.position);
        Vector3 startPos = turretGun.position;
        float x = Vector3.Distance(new Vector3(startPos.x, 0, startPos.z), new Vector3(targetPos.x, 0, targetPos.z));
        float deltaH = startPos.y - targetPos.y;
        float phaseAngle = Mathf.Atan2(x, deltaH) * Mathf.Rad2Deg;
        float e1 = ((Mathf.Abs(projGravity) * x * x) / (projStartSpeed * projStartSpeed)) - deltaH;
        //Debug.Log(x + " " + deltaH + " " + phaseAngle + " " + e1);
        e1 /= Mathf.Sqrt((deltaH * deltaH) + (x * x));
        //Debug.Log(e1);
        if ((e1 > 1) || (e1 < -1))
        {
            //target too far
            Debug.Log("Target too far");
            return;
        }
        float e2 = Mathf.Acos(e1) * Mathf.Rad2Deg;
        e2 += phaseAngle;
        e2 /= 2;
        
        turretGun.localRotation = Quaternion.Euler(-e2, turretGun.localRotation.y, turretGun.localRotation.z);
    }

    protected void DrawTrajectory()
    {
        List<Vector3> trajPoints = new List<Vector3>();
        bool apexReached = false;
        bool targetReached = false;
        Vector3 currentPoint = turretGun.position;
        Vector3 prevPoint = turretGun.position;
        float timePassed = 0f;
        float timeStep = 0.05f;
        Vector3 currentV = turretGun.forward.normalized * projStartSpeed;
        while (targetReached == false)
        {
            prevPoint = currentPoint;
            currentPoint = turretGun.position + (currentV * timePassed) + (Vector3.up * projGravity * timePassed * timePassed * 0.5f);

            timePassed += timeStep;

            if (currentPoint.y < prevPoint.y) apexReached = true;
            if (apexReached && (currentPoint.y < targetPoint.y))
            {
                currentPoint = targetPoint;
                targetReached = true;
            }
            trajPoints.Add(currentPoint);
        }
        trajectoryLine.positionCount = trajPoints.Count;
        trajectoryLine.SetPositions(trajPoints.ToArray());
    }


}
