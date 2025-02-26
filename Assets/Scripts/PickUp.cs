using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public GameObject heldObject;
    public GameObject currentPickup;
    public Transform player; // Reference to the player
    private isUsingDial dialScript;

    //Stored components of held objects


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            currentPickup = other.gameObject;
            Debug.Log("Entered pickup zone: " + currentPickup.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == currentPickup)
        {
            Debug.Log("Left pickup zone: " + currentPickup.name);
            currentPickup = null;
        }
    }

    private void Start()
    {
        if(GetComponent<isUsingDial>() != null)
        {
            dialScript = GetComponent<isUsingDial>();
        }
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && currentPickup != null && !dialScript.dialUse)
        {
            if (heldObject != null)
            {
                DropHeldObject(); // Drop the currently held object
            }

            Debug.Log("Picked up: " + currentPickup.name);
            GenerateHeldObject(currentPickup);
            Destroy(currentPickup);
            currentPickup = null;
        }

        if (dialScript.dialUse)
        {
            DropHeldObject();
        }
    }

    void GenerateHeldObject(GameObject original)
    {
        // Get unique offset and scale from PickupProperties
        PickupProperties properties = original.GetComponent<PickupProperties>();
        Vector3 pickupOffset = properties != null ? properties.pickupOffset : Vector3.zero;
        Vector3 pickupScale = properties != null ? properties.pickupScale : Vector3.one;

        // Instantiate new object (NOT parented yet!)
        GameObject newHeldObject = Instantiate(original);

        // Set correct world position before parenting
        newHeldObject.transform.position = player.position;

        // Apply correct scale BEFORE parenting
        newHeldObject.transform.localScale = pickupScale;

        Rigidbody originalRb = original.GetComponent<Rigidbody>();
        if(originalRb != null)
        {

        }

        // Remove unnecessary components (Fix: Ensure newHeldObject is referenced properly)
        if (newHeldObject.GetComponent<Rigidbody>())
            Destroy(newHeldObject.GetComponent<Rigidbody>());
        if (newHeldObject.GetComponent<Collider>())
            Destroy(newHeldObject.GetComponent<Collider>());

        // Set as a child of the player AFTER setting scale & position
        newHeldObject.transform.SetParent(player);

        // Now apply correct local position relative to the player
        newHeldObject.transform.localPosition = pickupOffset;

        // Assign the new held object reference
        heldObject = newHeldObject;
        Debug.Log("Generated held object: " + heldObject.name);
    }

    void DropHeldObject()
    {
        if (heldObject != null)
        {
            Debug.Log("Dropping held object: " + heldObject.name);

            // Remove it from being a child of the player
            heldObject.transform.SetParent(null);

            // Add Rigidbody and Collider back so it falls naturally
            Rigidbody rb = heldObject.AddComponent<Rigidbody>();
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            PickupProperties properties = heldObject.GetComponent<PickupProperties>();
            BoxCollider col = heldObject.AddComponent<BoxCollider>(); // Assuming a BoxCollider, modify if needed
            BoxCollider colTrigger = heldObject.AddComponent<BoxCollider>();
            colTrigger.isTrigger = true;
            colTrigger.size = col.size * properties.colTriggerScale;
            colTrigger.center = col.center;

            // Reset the reference BEFORE picking up the new object
            heldObject = null;
        }
    }
}

