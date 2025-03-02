using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupProperties : MonoBehaviour
{
    
    public Vector3 pickupOffset = new Vector3(0f, 0f, 0f); // Default offset
    public float pickupScale = 1f; // Default scale

    public DimensionButton dimensionOfOrigin;
    public float colTriggerScale = 1.2f;
}