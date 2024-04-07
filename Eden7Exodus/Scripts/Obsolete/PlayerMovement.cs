using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private enum PlayerMoveState
    {
        followRail,
        switchRail,
    }
    private PlayerMoveState playerMoveState = PlayerMoveState.followRail;

    [SerializeField] private float verticalSpeed;

    [SerializeField] private RailSegment railSegment;
    private RailSegment targetRail;
    [SerializeField] private Transform baseModel;
    private Rigidbody rb;

    [SerializeField] private List<ParticleSystem> trails = new List<ParticleSystem>();

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        PlayerControls.Instance.OnControlUp += PlayerControls_OnUp;
        PlayerControls.Instance.OnControlDown += PlayerControls_OnDown;
    }

    private void PlayerControls_OnUp(object sender, System.EventArgs e)
    {
        InputDirection(-1);
    }
    private void PlayerControls_OnDown(object sender, System.EventArgs e)
    {
        InputDirection(1);
    }

    private void InputDirection(int dir)
    {
        //Debug.Log("input reg " + dir);
        if ((playerMoveState == PlayerMoveState.followRail) && (railSegment.CanSwitchRails))
        {
            Turn(dir);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (playerMoveState)
        {
            case PlayerMoveState.followRail:
                SetCurrentRailSegment(railSegment.GetCurrentSegment(transform.position, 0));
                rb.MovePosition(railSegment.GetObjectPositionAtZ(transform.position));
                TurnBase(railSegment.GetVector());

                break;
            case PlayerMoveState.switchRail:
                Vector3 movePos = Vector3.MoveTowards(transform.position, new Vector3(targetRail.transform.position.x, 0, transform.position.z), verticalSpeed * Time.deltaTime);
                //moveDir = moveDir * speed;
                //moveDir.z = speed;

                TurnBase(MidSwitchAngle());

                if (transform.position.x == targetRail.transform.position.x)
                {
                    playerMoveState = PlayerMoveState.followRail;
                    SetCurrentRailSegment(targetRail);
                }
                else rb.MovePosition(movePos);
                
                break;
        }
        
    }

    private void Turn(int dir)
    {
        //Debug.Log("input " + dir);
        RaycastHit[] hits = Physics.RaycastAll(transform.position, new Vector3(dir, 0, 1), 3f);
        foreach (RaycastHit hit in hits)
        {
            RailSegment rs = hit.collider.GetComponent<RailCollider>()?.RailSegment;
            if ((rs == null) || (rs == railSegment)) continue;
            //Debug.Log(rs.gameObject.name);
            targetRail = rs;
            break;
        }

        //targetRail = railSegment.GetTargetRail(dir);
        if (targetRail == null) return;

        //railSegment = null;
        playerMoveState = PlayerMoveState.switchRail;

        //float desiredX = targetRail.transform.position.x;

        //Vector3 newDir = new Vector3(Mathf.Sign(desiredX - transform.position.x), 0, 1).normalized;
        //baseModel.transform.forward = newDir;
        //moveDir = newDir;
        //rb.velocity = newDir;
    }

    private Vector3 MidSwitchAngle()
    {
        if (targetRail == null) return Vector3.forward;

        float distTraveled = transform.position.x - railSegment.transform.position.x;
        float distTotal = targetRail.transform.position.x - railSegment.transform.position.x;
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
        railSegment = rs;
        //Debug.Log(rs.EmitDustTrail);
        if (rs.EmitDustTrail)
        {
            foreach (ParticleSystem ps in trails)
            {
                ps.Play(true);
            }
            
        }
        else
        {
            foreach (ParticleSystem ps in trails)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (new Vector3(1, 0, 1).normalized * 3));
        Gizmos.DrawLine(transform.position, transform.position + (new Vector3(-1, 0, 1).normalized * 3));
    }

}
