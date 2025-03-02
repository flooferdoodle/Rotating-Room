using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public GameObject _heldObject = null;
    private GameObject _activeObject = null;

    public GameObject _currentInteractable = null;
    private bool _objectIsPickup = false;

    public SpriteRenderer heldObjectRenderer;
    public Material SelectionEffectMaterial;
    private Material _unselectedMaterial;

    private HashSet<GameObject> _pickupObjectsInTrigger;
    private HashSet<GameObject> _interactablesInTrigger;

    private void UpdateSelectionEffectMaterialSlice()
    {
        if (_activeObject == null) return;
        SelectionEffectMaterial.SetFloat("_AngleOffset", _unselectedMaterial.GetFloat("_AngleOffset"));
        SelectionEffectMaterial.SetFloat("_SliceAngle", _unselectedMaterial.GetFloat("_SliceAngle"));
        SelectionEffectMaterial.SetFloat("_BlendAmount", _unselectedMaterial.GetFloat("_BlendAmount"));
    }

    // Start is called before the first frame update
    void Start()
    {
        _pickupObjectsInTrigger = new HashSet<GameObject>();
        _interactablesInTrigger = new HashSet<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        GetClosestObject();
        if (Input.GetKeyDown(KeyCode.E) && _activeObject != null)
            
        {
            DialDimensionHandler.Instance.ExitSelectionMode();
            if(_objectIsPickup) AttemptPickup();
            else
            {
                Interactable thisInteractable = _activeObject.GetComponent<Interactable>();
                _currentInteractable = _activeObject;
                thisInteractable.Interact(gameObject);
              
            }
        }

        //Debug.Log("Active: " + ((_activeObject != null) ? _activeObject.name : "null"));

        if(_heldObject != null)
        {
            //Debug.Log("Holding " + _heldObject.name);
        }
    }

    public void DropCurrentObject()
    {
        if (_heldObject != null)
        {
            Vector2 droppedPos = heldObjectRenderer.transform.position;

            // Drop object at current location
            _heldObject.transform.position = new Vector3(
                droppedPos.x, droppedPos.y, _heldObject.transform.position.z
            );

            // Remove sprite
            heldObjectRenderer.sprite = null;


            // Move object to new dimension (if necessary)
            DimensionButton newDim = DialDimensionHandler.Instance.GetDimensionForPosition(droppedPos);
            if (_heldObject.GetComponent<PickupProperties>().dimensionOfOrigin.dimension != newDim.dimension)
            {
                // Debug.Log("Swapped held object dimension to " + newDim.dimension.DimensionRoom.name);
                // Remove object from current MaskColliders
                _heldObject.GetComponentInParent<MaskColliders>().RemoveObject(_heldObject);

                // Add object to new dimension and MaskColliders
                Transform newDimObjects = newDim.dimension.DimensionRoom.Find("Objects");
                _heldObject.transform.parent = newDimObjects;
                newDimObjects.GetComponent<MaskColliders>().AddObject(_heldObject);

                // Set material to new dimension material
                _heldObject.GetComponentInChildren<SpriteRenderer>().sharedMaterial = newDim.dimension.objectMatBase;

                _heldObject.GetComponent<PickupProperties>().dimensionOfOrigin = newDim;
            }


            // Re-enable object
            _heldObject.SetActive(true);
            _heldObject = null;
        }
    }

    private void AttemptPickup()
    {
        DropCurrentObject();

        // Pickup new object
        _heldObject = _activeObject;
        _activeObject.GetComponentInChildren<SpriteRenderer>().material = _unselectedMaterial;
        GetClosestObject();
        heldObjectRenderer.sprite = _heldObject.GetComponentInChildren<SpriteRenderer>().sprite;

        // Update held item sprite
        PickupProperties properties = _activeObject.GetComponent<PickupProperties>();
        heldObjectRenderer.transform.localPosition = properties.pickupOffset;
        heldObjectRenderer.transform.localScale = new Vector3(properties.pickupScale, properties.pickupScale, properties.pickupScale);

        // Disable original object
        _heldObject.SetActive(false);
    }


    private bool IsObjectInActiveSlice(GameObject obj)
    {
        MaskColliders.QuadPoly box = obj.GetComponentInParent<MaskColliders>().FindGameobjectHitBox(obj);
        if(box == null)
        {
            Debug.Log("Failed to verify object slice");
            return true; // default to allowing object selection
        }
        Vector2 objCenter = box.GetCenter();


        return DialDimensionHandler.IsPosInDimension(objCenter, obj.GetComponent<PickupProperties>().dimensionOfOrigin.dimension);
    }

    private void GetClosestObject()
    {
        if(_activeObject != null)
        {
            _activeObject.GetComponentInChildren<SpriteRenderer>().sharedMaterial = _unselectedMaterial;
        }

        _activeObject = null;
        float minDist = Mathf.Infinity;

        // TODO: Exclude objects who's collider midpoints are outside of the dimension
        foreach(GameObject obj in _pickupObjectsInTrigger)
        {
            if (!IsObjectInActiveSlice(obj))
            {
                //Debug.Log("Ignored for not being in active slice");
                continue;
            }

            float dist = (obj.transform.position - transform.position).sqrMagnitude;
            if(dist < minDist)
            {
                _activeObject = obj;
                minDist = dist;
                _objectIsPickup = true;
            }
        }
        foreach(GameObject obj in _interactablesInTrigger)
        {
            if (!IsObjectInActiveSlice(obj)){
                //Debug.Log("discluded from active slice");
                continue;

            }

            float dist = (obj.transform.position - transform.position).sqrMagnitude;
            if (dist < minDist)
            {
                _activeObject = obj;
                minDist = dist;
                _objectIsPickup = false;
            }
        }

        if (_activeObject != null) {
            _unselectedMaterial = _activeObject.GetComponentInChildren<SpriteRenderer>().sharedMaterial;
            _activeObject.GetComponentInChildren<SpriteRenderer>().sharedMaterial = SelectionEffectMaterial;
        }

        UpdateSelectionEffectMaterialSlice();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.GetComponent<PickupProperties>() == null)
        {
            //Debug.Log("Skipped object with no pickup property");
            return;
        }
        if (other.CompareTag("Pickup"))
        {
            _pickupObjectsInTrigger.Add(other.gameObject);
        }
        else if (other.CompareTag("Interactable"))
        {
            Debug.Log("Detected trigger enter interactable");
            _interactablesInTrigger.Add(other.gameObject);
            
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {

        if (other.CompareTag("Pickup"))
        {
            _pickupObjectsInTrigger.Remove(other.gameObject);
        }
        else if (other.CompareTag("Interactable"))
        {
            _interactablesInTrigger.Remove(other.gameObject);
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
