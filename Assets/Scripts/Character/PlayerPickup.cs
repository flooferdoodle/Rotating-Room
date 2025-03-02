using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    private GameObject _heldObject = null;
    private GameObject _activeObject = null;
    private bool _objectIsPickup = false;
    public float DefaultPickupItemRescale = 15f;

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
            if(_objectIsPickup) AttemptPickup();
        }
    }

    public void DropCurrentObject()
    {
        if (_heldObject != null)
        {
            // Drop object at current location
            _heldObject.transform.position = new Vector3(
                heldObjectRenderer.transform.position.x, heldObjectRenderer.transform.position.y, _heldObject.transform.position.z
            );

            // Remove sprite
            heldObjectRenderer.sprite = null;

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
        Material spriteShader = obj.GetComponentInChildren<SpriteRenderer>().sharedMaterial;
        float angle = spriteShader.GetFloat("_AngleOffset");
        // TODO: Change from accessing the shader to accessing the Dimension from DimensionButton instance that this object is in

        return false;
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
        if (other.CompareTag("Pickup"))
        {
            _pickupObjectsInTrigger.Add(other.gameObject);
        }
        else if (other.CompareTag("Interactable"))
        {
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
