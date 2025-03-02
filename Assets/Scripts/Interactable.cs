using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public GameObject requiredItem;
    public bool needsItem = false;
    public bool oneTimeUse = false;

    private bool isUsed = false;

    public CutsceneManager cutsceneManager;
    public Sprite[] cutsceneImages; // Cutscene images to play
    public bool autoPlay = false; // Should this cutscene autoplay?
    public bool needsCode = false;
    private string neededCode = "";

    public GameObject spawnedObject;

    private void Start()
    {
        cutsceneManager = FindObjectOfType<CutsceneManager>();
        //Debug.Log("interactable" + needsCode);
    }

    public virtual void Interact(GameObject player)
    {
        if (oneTimeUse && isUsed) return;

        PlayerPickup thisPickUp = player.GetComponent<PlayerPickup>();
        GameObject holding = thisPickUp._currentInteractable;


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
        Debug.Log("cut scenes length" + cutsceneImages.Length);
        Debug.Log("cutt scenes manager" + cutsceneManager != null);
        if (cutsceneManager != null)
        {
            // && cutsceneImages.Length > 0
            cutsceneManager.StartCutscene(cutsceneImages, autoPlay, needsCode, neededCode, spawnedObject);
        }

    }
}
