using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBeam : TurretBase, IObjectReset
{

    private const string SHADER_NAME_STRING = "HeatShader";
    private const string SHADER_FLOAT_STRING = "_HeatLevel";
    [SerializeField] private Transform shootPoint;
    [SerializeField] private LineRenderer trajectoryLine;
    [SerializeField] private ProjectileBeam projectileBeam;
    private ProjectileBeam currentBeam;
    [SerializeField] private Renderer gunRenderer;
    private Material heatsinkMaterial;
    [SerializeField] private AudioSource beamSound;

    [SerializeField] private GameObject reticle;
    [SerializeField] private float offsetY = 0.1f;
    [SerializeField] private float angleLimit = 10f;

    private bool isShooting = false;

    [SerializeField] private float maxHeat = 100f;
    [SerializeField] private float heatingRate = 20f;
    [SerializeField] private float coolingRate = 15f;
    [SerializeField] private float overheatPenalty = 0.5f;
    private bool isOverheated = false;
    private float heatLevel = 0f;

    public void ResetObject(bool preSpawn = false)
    {
        if (preSpawn) return;
        beamSound.UnPause();
        beamSound.Stop();
        isOverheated = false;
        heatLevel = 0f;
        targetPoint = Vector3.zero;
        StopShooting();
    }

    private void Start()
    {
        foreach (Material m in gunRenderer.materials)
        {
            if (m.shader.name.EndsWith(SHADER_NAME_STRING))
            {
                heatsinkMaterial = m;
                //Debug.Log(heatsinkMaterial.name);
                break;
            }
        }

        UIManager.Instance.OnPauseChange += UIManager_OnPauseChange;
    }

    private void UIManager_OnPauseChange(object sender, bool e)
    {
        if (e && isShooting) beamSound.Pause();
        if (!e) beamSound.UnPause();
    }

    private void Update()
    {
        if (isShooting)
        {
            heatLevel = Mathf.Min(maxHeat, heatLevel + (heatingRate * Time.deltaTime));
            if (heatLevel == maxHeat) SetOverheat(true);
        }
        else
        {
            float cooling = isOverheated ? (coolingRate * overheatPenalty) : coolingRate;
            heatLevel = Mathf.Max(0, heatLevel - (cooling * Time.deltaTime));
            if (isOverheated && (heatLevel == 0)) SetOverheat(false);
        }

        float heatRelative = heatLevel / maxHeat;
        beamSound.pitch = 1 + (3 * heatRelative);
        if (heatRelative == 0 && !isShooting) beamSound.Stop();
        //Debug.Log("heat " + heatRelative);
        heatsinkMaterial.SetFloat(SHADER_FLOAT_STRING, heatRelative);
    }

    private void SetOverheat(bool oh)
    {
        if (oh && isShooting) StopShooting();
        isOverheated = oh;
    }

    private void Shoot()
    {
        currentBeam = ObjectPoolManager.Instance.RequestObjectAt(projectileBeam.gameObject, shootPoint.position).GetComponent<ProjectileBeam>();
        currentBeam.transform.SetParent(shootPoint, true);
        currentBeam.transform.position = shootPoint.position;
        currentBeam.SetDirection(shootPoint.forward);
        beamSound.Play();
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
        if (isOverheated || isShooting) return;

        Shoot();
        isShooting = true;
        HideTrajectory();
    }

    public override void StopShooting()
    {
        if (!isShooting) return;
        ObjectPoolManager.Instance.Deactivate(currentBeam.gameObject);
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
        float sin = (targetPos.y - turretGun.position.y) / Vector3.Distance(targetPos, turretGun.position);
        float angle = Mathf.Asin(sin) * Mathf.Rad2Deg;
        if (float.IsNaN(angle)) angle = 0;
        angle = Mathf.Clamp(angle, -angleLimit, angleLimit);
        turretGun.localRotation = Quaternion.Euler(-angle, turretGun.localRotation.y, turretGun.localRotation.z);
    }

    protected void DrawTrajectory()
    {
        Vector3[] trajectory = new Vector3[2];
        Vector3 offset = shootPoint.position - turretGun.position;
        trajectory[0] = shootPoint.position;
        trajectory[1] = shootPoint.position + ((targetPoint - shootPoint.position) * 2);
        trajectoryLine.positionCount = trajectory.Length;
        trajectoryLine.SetPositions(trajectory);
    }

    private void HideTrajectory()
    {
        trajectoryLine.gameObject.SetActive(false);
    }

    private void ShowTrajectory()
    {
        trajectoryLine.gameObject.SetActive(true);
    }
}
