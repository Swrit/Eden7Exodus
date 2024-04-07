using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAlongRail : MonoBehaviour, IObjectReset
{
    public event EventHandler OnEnterDepot;
    private enum RailMoveState
    {
        initialScan,
        followRail,
        switchRail,
        catchUpToRail,
        fallOffRail,
    }
    private RailMoveState railMoveState = RailMoveState.initialScan;
    [SerializeField] [Range(-1, 1)] private int horizontalDirection;
    [SerializeField][Range(0, float.PositiveInfinity)] private float horizontalSpeed;
    [SerializeField] private float railSwitchVerticalSpeed;
    [SerializeField] private float gravity;
    private float fallSpeed = 0f;
    [SerializeField] private float fallTurnSpeed;

    private float railScanDistance = 4f;
    private float initialScanRadius = 0.5f;

    private RailSegment currentRailSegment;
    private RailSegment targetRail;
    [SerializeField] private Transform baseModel;
    private Rigidbody rb;

    [SerializeField] private List<ParticleSystem> trails = new List<ParticleSystem>();

    public void ResetObject(bool preSpawn = false)
    {
        fallSpeed = 0f;
        railMoveState = RailMoveState.initialScan;
        currentRailSegment = null;
        transform.forward = new Vector3(0, 0, Mathf.Sign(horizontalDirection));
        if (rb == null) rb = GetComponent<Rigidbody>();
        CorrectHorizontalSpeed(GameManager.Instance.CurrentScrollSpeed);
    }

    private void Start()
    {
        GameManager.Instance.OnScrollSpeedChanged += GameManager_OnScrollSpeedChanged;
    }

    private void GameManager_OnScrollSpeedChanged(object sender, float e)
    {
        CorrectHorizontalSpeed(e);
    }

    private void CorrectHorizontalSpeed(float scrollSpeed)
    {
        switch (horizontalDirection)
        {
            case -1:
                rb.velocity = Vector3.back * (horizontalSpeed + scrollSpeed);
                break;
            case 1:
                rb.velocity = Vector3.forward * (horizontalSpeed - scrollSpeed);
                break;
            default:
                rb.velocity = Vector3.zero;
                break;
        }
    }

    public void SetController(IRailMovementController controller)
    {
        controller.OnControlUp += MovementControls_OnUp;
        controller.OnControlDown += MovementControls_OnDown;
    }

    private void MovementControls_OnUp(object sender, System.EventArgs e)
    {
        InputDirection(-1);
    }
    private void MovementControls_OnDown(object sender, System.EventArgs e)
    {
        InputDirection(1);
    }

    private void InputDirection(int dir)
    {
        if ((railMoveState == RailMoveState.followRail) && (currentRailSegment.CanSwitchRails))
        {
            TryTurn(dir);
        }
    }

    void FixedUpdate()
    {
        switch (railMoveState)
        {
            case RailMoveState.initialScan:
                InitialScan();
                break;
            case RailMoveState.followRail:
                //Debug.Log("rs " + currentRailSegment.gameObject.name);
                SetCurrentRailSegment(currentRailSegment.GetCurrentSegment(transform.position, horizontalDirection));
                if (railMoveState == RailMoveState.fallOffRail) break;

                rb.MovePosition(currentRailSegment.GetObjectPositionAtZ(transform.position));
                TurnBase(RailVectorWithDirectionCorrection(currentRailSegment));
                break;
            case RailMoveState.switchRail:
                Vector3 movePos = Vector3.MoveTowards(transform.position, new Vector3(targetRail.transform.position.x, 0, transform.position.z), railSwitchVerticalSpeed * Time.deltaTime);

                TurnBase(MidSwitchAngle());

                if (transform.position.x == targetRail.transform.position.x)
                {
                    railMoveState = RailMoveState.followRail;
                    SetCurrentRailSegment(targetRail);
                }
                else rb.MovePosition(movePos);

                break;

            case RailMoveState.catchUpToRail:
                break;

            case RailMoveState.fallOffRail:
                float angle = Vector3.SignedAngle(baseModel.forward, Vector3.down, baseModel.right);
                float turnCap = fallTurnSpeed * Time.deltaTime;
                baseModel.Rotate(baseModel.right, Mathf.Clamp(angle, -turnCap, turnCap));
                
                fallSpeed += gravity * Time.deltaTime;
                transform.position = (transform.position + (Vector3.up * fallSpeed * Time.deltaTime));
                break;
        }

    }

    private Vector3 RailVectorWithDirectionCorrection(RailSegment railSegment)
    {
        return (railSegment.GetVector() * Mathf.Sign(horizontalDirection));
    }

    private void InitialScan()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, initialScanRadius);
        foreach (Collider col in colliders)
        {
            //Debug.Log(col.gameObject.name);
            RailSegment rs = col.GetComponent<RailCollider>()?.RailSegment;
            if (rs != null)
            {
                SetCurrentRailSegment(rs);
                //Debug.Log("rail " + rs.gameObject.name);
                railMoveState = RailMoveState.followRail;
                break;
            }
        }
    }

    private bool TryTurn(int dir)
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, new Vector3(dir, 0, 1), railScanDistance);
        foreach (RaycastHit hit in hits)
        {
            RailSegment rs = hit.collider.GetComponent<RailCollider>()?.RailSegment;
            if ((rs == null) || (rs == currentRailSegment)) continue;
            targetRail = rs;
            break;
        }
        if (targetRail == null) return false;

        railMoveState = RailMoveState.switchRail;
        return true;
    }

    private Vector3 MidSwitchAngle()
    {
        if (targetRail == null) return Vector3.forward;

        float distTraveled = transform.position.x - currentRailSegment.transform.position.x;
        float distTotal = targetRail.transform.position.x - currentRailSegment.transform.position.x;
        float deltaX = (distTraveled / distTotal);
        float angle = ((-4 * (deltaX * deltaX)) + (4 * deltaX)) * Mathf.Sign(distTraveled);
        return new Vector3(angle, 0, 1);
    }

    private void TurnBase(Vector3 dir)
    {
        baseModel.transform.forward = dir;
    }

    private void SetCurrentRailSegment(RailSegment rs)
    {
        if (currentRailSegment == rs) return;
        currentRailSegment = rs;
        if (rs == null)
        {
            railMoveState = RailMoveState.fallOffRail;
            SetDustTrail(false);
            return;
        }
        SetDustTrail(rs.EmitDustTrail);
        if (rs.CallDepot) OnEnterDepot?.Invoke(this, EventArgs.Empty);
    }

    private void SetDustTrail(bool setting)
    {
        foreach (ParticleSystem ps in trails)
        {
            if (setting) ps.Play(true);
            else ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (new Vector3(1, 0, 1).normalized * 3));
        Gizmos.DrawLine(transform.position, transform.position + (new Vector3(-1, 0, 1).normalized * 3));
    }

}
