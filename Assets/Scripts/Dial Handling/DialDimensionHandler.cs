using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(DialControl))]
public class DialDimensionHandler : MonoBehaviour
{
    public GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    public EventSystem m_EventSystem;

    public DimensionButton DefaultDimension;
    public Image topHalfTex;
    public Image botHalfTex;

    public DimensionButton botDimension;
    public DimensionButton topDimension;

    public Color DeselectTint = new Color(0.8f, 0.8f, 0.8f);

    private DimensionButton _selected = null;
    private DialControl _dialControl;

    // Start is called before the first frame update
    void Start()
    {
        _dialControl = GetComponent<DialControl>();
        topHalfTex.transform.parent.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
        botHalfTex.transform.parent.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;

        // Reset dial with defaults
        if (botDimension == null) botDimension = DefaultDimension;
        if (topDimension == null) topDimension = DefaultDimension;

        UpdateDialSprites();
    }

    // Update is called once per frame
    void Update()
    {
        if(_selected != null)
        {
            // Selection mode

            // Mouseover selected half highlights
            topHalfTex.color = DeselectTint;
            botHalfTex.color = DeselectTint;
            GameObject dialHit = GetDialMouseOver();
            if(dialHit != null)
            {
                dialHit.transform.GetChild(0).GetComponent<Image>().color = Color.white;
            }

            if(Input.GetMouseButtonDown(0))
            {
                // Make selection
                if(dialHit != null)
                {
                    if (dialHit.CompareTag("Dial Top")) topDimension = _selected;
                    else botDimension = _selected;
                    UpdateDialSprites();
                }

                // Disable selection mode
                ExitSelectionMode();
            }
        }
    }

    private void UpdateDialSprites()
    {
        topHalfTex.sprite = topDimension.dimension.dialSprite;
        botHalfTex.sprite = botDimension.dimension.dialSprite;
    }

    private void ExitSelectionMode()
    {
        _selected = null;
        _dialControl.Enable();
        topHalfTex.color = Color.white;
        botHalfTex.color = Color.white;
    }

    private GameObject GetDialMouseOver()
    {
        //Set up the new Pointer Event
        m_PointerEventData = new PointerEventData(m_EventSystem);
        //Set the Pointer Event Position to that of the mouse position
        m_PointerEventData.position = Input.mousePosition;

        //Create a list of Raycast Results
        List<RaycastResult> results = new List<RaycastResult>();

        //Raycast using the Graphics Raycaster and mouse click position
        m_Raycaster.Raycast(m_PointerEventData, results);

        //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.layer == 6) return result.gameObject;
        }
        return null;
    }

    

    /// <summary>
    /// Call from dimension buttons, tells handler to enter selection mode
    /// </summary>
    /// <param name="dimension"></param>
    public void SelectDimension(DimensionButton dimension)
    {
        _selected = dimension;

        // Disable normal dial controls
        _dialControl.Disable();

        // Apply tint to dial
        topHalfTex.color = DeselectTint;
        botHalfTex.color = DeselectTint;
    }
}
