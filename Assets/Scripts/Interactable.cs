using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public GameObject requiredItem;
    public bool needsItem = false;
    public bool oneTimeUse = false;
    private bool isUsed = false;

    public virtual void Interact(GameObject player)
    {
        if (oneTimeUse && isUsed) return;

        PickUp thisPickUp = player.GetComponent<PickUp>();
        GameObject holding = thisPickUp.heldObject;


        if (needsItem && requiredItem!=holding)
        {
            Debug.Log("You need " + requiredItem + " to interact!");
            return;
        }

        PerformInteraction();
        isUsed = true;
    }

    protected virtual void PerformInteraction()
    {
        Debug.Log("Interacted with " + gameObject.name);
    }
}
