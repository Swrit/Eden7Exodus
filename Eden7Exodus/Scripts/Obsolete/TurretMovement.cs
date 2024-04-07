using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretMovement : MonoBehaviour
{
    [SerializeField] private Transform turretBase;
    [SerializeField] private Transform turretGun;
    [SerializeField] private LineRenderer trajectoryLine;

    [SerializeField] private GameObject projPrefab;
    private float projGravity;
    private float projStartSpeed;

    private Vector3 targetPoint = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        MouseTarget.Instance.OnTargetChange += MouseTarget_OnTargetChange;

        ProjectileFlight pf = projPrefab.GetComponent<ProjectileFlight>();
        projGravity = pf.Gravity;
        projStartSpeed = pf.Speed;
    }

    private void MouseTarget_OnTargetChange(object sender, Vector3 e)
    {
        targetPoint = e;
    }

    // Update is called once per frame
    void Update()
    {
        RotateTurretBase(targetPoint);
        CalcTrajectory(targetPoint);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ProjectileFlight pj = ObjectPoolManager.Instance.RequestObjectAt(projPrefab.gameObject, turretGun.position).GetComponent<ProjectileFlight>(); //Instantiate(projPrefab, turretGun.position, Quaternion.identity);
            pj.SetDirection(turretGun.forward);
        }
    }

    private void RotateTurretBase(Vector3 targetPos)
    {
        Vector3 tbDir = targetPos - turretBase.position;
        tbDir.y = 0;
        turretBase.forward = tbDir;
    }

    private void CalcTrajectory(Vector3 targetPos)
    {
        Vector3 startPos = turretGun.position;
        float x = Vector3.Distance(new Vector3(startPos.x, 0, startPos.z), new Vector3(targetPos.x, 0, targetPos.z));

        //float gravity = Mathf.Abs(projPrefab.Gravity);
        float deltaH = startPos.y - targetPos.y;
        float phaseAngle = Mathf.Atan2(x, deltaH) * Mathf.Rad2Deg;
        //float initV = projPrefab.Speed;
        //Debug.Log("x h " + x + " " + deltaH);
        //.Log("phaseAngle" + phaseAngle);
        float e1 = ((Mathf.Abs(projGravity) * x * x) / (projStartSpeed * projStartSpeed)) - deltaH;
        e1 /= Mathf.Sqrt((deltaH * deltaH) + (x * x));
        //Debug.Log("e1" + e1);
        if ((e1 > 1) || (e1 < -1))
        {
            Debug.Log("too far");
            return;
        }
        float e2 = Mathf.Acos(e1) * Mathf.Rad2Deg;
        e2 += phaseAngle;
        e2 /= 2;
        //Debug.Log("e2" + e2);
        //Debug.Log(e2 + " " + turretGun.localRotation.x);
        turretGun.localRotation = Quaternion.Euler(-e2, turretGun.localRotation.y, turretGun.localRotation.z);

        List<Vector3> trajPoints = new List<Vector3>();
        bool apexReached = false;
        bool targetReached = false;
        Vector3 currentPoint = turretGun.position;
        Vector3 prevPoint = turretGun.position;
        //trajPoints.Add(currentPoint);
        float timePassed = 0f;
        float timeStep = 0.05f;
        Vector3 currentV = turretGun.forward.normalized * projStartSpeed;
        while (targetReached == false)
        {
            prevPoint = currentPoint;
            currentPoint = turretGun.position + (currentV * timePassed) + (Vector3.up * projGravity * timePassed * timePassed * 0.5f);
            //currentV += Vector3.up * projPrefab.Gravity * timeStep;
            //currentPoint += currentV * timeStep;
            timePassed += timeStep;

            if (currentPoint.y < prevPoint.y) apexReached = true;
            if (apexReached && (currentPoint.y < targetPos.y))
            {
                currentPoint = targetPos;
                targetReached = true;
            }
            trajPoints.Add(currentPoint);
        }
        trajectoryLine.positionCount = trajPoints.Count;
        trajectoryLine.SetPositions(trajPoints.ToArray());
    }


}
