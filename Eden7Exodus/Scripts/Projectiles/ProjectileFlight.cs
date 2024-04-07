using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileFlight : MonoBehaviour
{
    public float Speed { get { return speed; } }
    public float Gravity { get { return gravity; } }

    private Vector3 direction;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float gravity = -10f;
    public void SetDirection(Vector3 dir)
    {
        transform.forward = dir.normalized;
        rb.velocity = transform.forward * speed;
        //Debug.Log(dir + " " + transform.forward + " " + rb.velocity);
        //transform.LookAt(transform.position + rb.velocity);
    }

    private void FixedUpdate()
    {
        //Debug.Log(transform.forward);
        if (gravity != 0)
        {
            rb.velocity += Vector3.up * gravity * Time.fixedDeltaTime;
            transform.LookAt(transform.position + rb.velocity);   
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + rb.velocity);
    }

}
