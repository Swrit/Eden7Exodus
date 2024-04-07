using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTarget : MonoBehaviour
{
    public static MouseTarget Instance { get; private set; }

    public event EventHandler<Vector3> OnTargetChange;

    [SerializeField] private GameObject recticle;
    [SerializeField] private LayerMask mouseTargetPlane;
    [SerializeField] private Collider targetArea;
    private Bounds targetAreaBounds;
    private Plane targetPlane;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        targetAreaBounds = targetArea.bounds;
        targetPlane = new Plane(Vector3.up, targetArea.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.x = Mathf.Clamp(mousePos.x, 0, Screen.width);
        mousePos.y = Mathf.Clamp(mousePos.y, 0, Screen.height);
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        float enter;
        if (targetPlane.Raycast(ray, out enter))
        {
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);
            Vector3 targetPos = ray.GetPoint(enter);
            targetPos.z = Mathf.Clamp(targetPos.z, targetAreaBounds.min.z, targetAreaBounds.max.z);
            targetPos.x = Mathf.Clamp(targetPos.x, targetAreaBounds.min.x, targetAreaBounds.max.x);
            recticle.transform.position = targetPos;
            OnTargetChange?.Invoke(this, targetPos);
        }

    }
}
