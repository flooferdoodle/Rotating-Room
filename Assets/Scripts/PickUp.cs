using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public GameObject currentPickup;
    public Transform player; // Reference to the player

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && currentPickup != null)
        {
            Debug.Log("Picked up: " + currentPickup.name);
            GenerateHeldObject(currentPickup);
            Destroy(currentPickup);
            currentPickup = null;
        }
    }

    void GenerateHeldObject(GameObject original)
    {
        // Get unique offset and scale from PickupProperties
        PickupProperties properties = original.GetComponent<PickupProperties>();
        Vector3 pickupOffset = properties != null ? properties.pickupOffset : Vector3.zero;
        Vector3 pickupScale = properties != null ? properties.pickupScale : Vector3.one;

        // Instantiate new object (NOT parented yet!)
        GameObject heldObject = Instantiate(original);

        // Remove unnecessary components
        if (heldObject.GetComponent<Rigidbody>())
            Destroy(heldObject.GetComponent<Rigidbody>());
        if (heldObject.GetComponent<Collider>())
            Destroy(heldObject.GetComponent<Collider>());

        // Set correct world position before parenting
        heldObject.transform.position = player.position;

        // Apply correct scale BEFORE parenting
        heldObject.transform.localScale = pickupScale;

        // Set as a child of the player AFTER setting scale & position
        heldObject.transform.SetParent(player);

        // Now apply correct local position relative to the player
        heldObject.transform.localPosition = pickupOffset;

        Debug.Log("Generated held object: " + heldObject.name);
    }

}
