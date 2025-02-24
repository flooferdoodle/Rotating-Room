using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PickUp : MonoBehaviour
{
    public GameObject currentPickup;

    // Called when another collider enters the trigger attached to this object.
    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is tagged as "Pickup".
        if (other.CompareTag("Pickup"))
        {
            currentPickup = other.gameObject;
            Debug.Log("Entered pickup zone: " + currentPickup.name);
        }
    }

    // Called when another collider exits the trigger attached to this object.
    private void OnTriggerExit(Collider other)
    {
        // Clear the reference if the object leaving is the current pickup.
        if (other.gameObject == currentPickup)
        {
            Debug.Log("Left pickup zone: " + currentPickup.name);
            currentPickup = null;
        }
    }

    // Update is called once per frame.
    private void Update()
    {
        // Check if the "E" key is pressed and a pickup is available.
        if (Input.GetKeyDown(KeyCode.E) && currentPickup != null)
        {
            Debug.Log("Picked up: " + currentPickup.name);
            // Simulate pickup by destroying the object.
            Destroy(currentPickup);
            currentPickup = null;
        }
    }
}