using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupProperties : MonoBehaviour
{
    
    public Vector3 pickupOffset = new Vector3(18.6f, 9.3f, 3.3f); // Default offset
    public Vector3 pickupScale = new Vector3(0.5f, 0.5f, 0.5f); // Default scale

    void Start()
    {
        //pickupOffset = transform.localPosition; // Save correct local offset
    }
}