using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialControl : MonoBehaviour
{
    private RectTransform dialTransform;
    private float currentAngle;

    public float GetAngle() { return dialTransform.localEulerAngles[2]; }

    private void Start()
    {
        dialTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            currentAngle = dialTransform.localEulerAngles[2];
        }

        if (Input.GetMouseButton(0)) // Detect mouse drag
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 dir = mousePos - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            dialTransform.localRotation = Quaternion.Euler(0, 0, (angle - currentAngle));
        }
    }
}
