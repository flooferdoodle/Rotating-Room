using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{

    //what are you currently holding
    public GameObject heldObject;
    //what are you currently colliding with
    public GameObject currentPickup;
    public GameObject currentInteractable;
    public GameObject thisPlayer;

    public Transform player;
    public bool isHolding;

    //boolean for testing -- are you using the dial
    private isUsingDial dialScript;

    //throws debug messages & updates currentPickup
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            currentPickup = other.transform.parent.gameObject;
            Debug.Log("Entered pickup zone: " + currentPickup.name);
        }

        if (other.CompareTag("Interactable"))
        {
            currentInteractable = other.transform.parent.gameObject;
            Debug.Log("Entered interact zone: " + currentInteractable.name);
        }
    }

    //throws debug & updates current pickup
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent != null)
        {
            if (other.transform.parent.gameObject == currentPickup) currentPickup = null;
            if (other.transform.parent.gameObject == currentInteractable) currentInteractable = null;
        }
    }

    //initialize script that you get dial bool from

    private void Start()
    {
        if(heldObject != null)
        {
            isHolding = true;
        }
        else
        {
            isHolding = false;
        }
        if(GetComponent<isUsingDial>() != null)
        {
            dialScript = GetComponent<isUsingDial>();
        }

        thisPlayer = gameObject;
        
    }

    private void Update()
    {
        //if user interacts check if in interact zone and if using dial
        if (Input.GetKeyDown(KeyCode.E) && currentPickup != null && !dialScript.dialUse)
        {
            // Drop the currently held object
            if (isHolding)
            {
                DropHeldObject(); 
            }

            //generate held version of the object
            //update bool isHolding

            Debug.Log("Picked up: " + currentPickup.name);
            GenerateHeldObject(currentPickup);
            Destroy(currentPickup);
            currentPickup = null;
        }

        if (dialScript.dialUse)
        {
            DropHeldObject();
        }

        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null && !dialScript.dialUse)
        {
            Interactable thisInteractable = currentInteractable.GetComponent<Interactable>();
            thisInteractable.Interact(thisPlayer);
        }
    }

    void GenerateHeldObject(GameObject original)
    {
        // Get unique offset and scale from PickupProperties
        PickupProperties properties = original.GetComponent<PickupProperties>();
        Vector3 pickupOffset = properties != null ? properties.pickupOffset : Vector3.zero;
        Vector3 pickupScale = properties != null ? new Vector3(properties.pickupScale, properties.pickupScale, properties.pickupScale) : Vector3.one;
        isHolding = true;

        // Instantiate new object (NOT parented yet!)
        GameObject newHeldObject = Instantiate(original);

        // Set correct world position before parenting
        newHeldObject.transform.position = player.position;

        // Apply correct scale BEFORE parenting
        newHeldObject.transform.localScale = pickupScale;

        Collider[] colliders = newHeldObject.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            disableCollider(col);
        }

        Rigidbody rb = newHeldObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;  // Disables physics
            rb.useGravity = false;  // Prevents falling
        }

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
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false; // Re-enables physics
                rb.useGravity = true; // Allows it to fall
            }

            Collider[] colliders = heldObject.GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
            {
                enableCollider(col);
            }

            // Reset the reference BEFORE picking up the new object
            heldObject = null;
            isHolding = false;
        }
    }

    void disableCollider(Collider col)
    {
        if (col != null)
        {
            col.enabled = false;
        }
    }

    void enableCollider(Collider col)
    {
        if (col != null)
        {
            col.enabled = true;
        }
    }
    

    
}

