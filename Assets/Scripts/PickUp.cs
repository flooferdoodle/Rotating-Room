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

        // Instantiate new object at player's position + unique offset
        GameObject heldObject = Instantiate(original, player);
        heldObject.transform.position = player.position; // Set correct local position
        //heldObject.transform.localScale = pickupScale;

        // Remove unnecessary components
        if (heldObject.GetComponent<Rigidbody>())
            Destroy(heldObject.GetComponent<Rigidbody>());
        if (heldObject.GetComponent<Collider>())
            Destroy(heldObject.GetComponent<Collider>());


        // Set as a child of the player
        heldObject.transform.SetParent(player);
        heldObject.transform.localPosition = pickupOffset;

        Debug.Log("Generated held object: " + heldObject.name);
    }
}
