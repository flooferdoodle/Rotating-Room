using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialControl : MonoBehaviour
{
    public Material testMaterialEdit;

    private RectTransform dialTransform;
    private float currentAngle;
    private float mouseStartAngle;
    public float DetectionRadius = 10f;
    private bool clickedOn;
    private bool _enabled = true;

    public float GetAngle() { return dialTransform.localEulerAngles[2]; }

    public void Disable() { _enabled = false; }
    public void Enable() { _enabled = true; }

    private void Start()
    {
        dialTransform = transform.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (!_enabled) return;

        if (Input.GetMouseButtonDown(0))
        {
            TopDownCharacterController.Instance.UseDial();
            if ((Input.mousePosition - transform.position).sqrMagnitude < DetectionRadius * DetectionRadius)
            {
                currentAngle = dialTransform.localEulerAngles[2];
                mouseStartAngle = GetMouseAngle();
                clickedOn = true;
            }
        }

        if (Input.GetMouseButton(0) && clickedOn) // Detect mouse drag
        {
            TopDownCharacterController.Instance.UseDial();
            float mouseAngle = GetMouseAngle();
            dialTransform.localRotation = Quaternion.Euler(0, 0, mouseAngle - mouseStartAngle + currentAngle);
        }

        if (Input.GetMouseButtonUp(0)) clickedOn = false;

        //testMaterialEdit.SetFloat("_AngleOffset", GetAngle());
    }

    private float GetMouseAngle()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 dir = mousePos - transform.position;
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, DetectionRadius);
    }
}
