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

    private void Start()
    {
        cutsceneManager = FindObjectOfType<CutsceneManager>(); // Find Cutscene Manager in the scene
    }

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
        Debug.Log("cut tscenes length" + cutsceneImages.Length);
        Debug.Log("cut tscenes manager" + cutsceneManager != null);
        if (cutsceneManager != null && cutsceneImages.Length > 0)
        {
            cutsceneManager.StartCutscene(cutsceneImages, autoPlay);
        }

    }
}
