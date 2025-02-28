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

    public List<DimensionButton> dimensionsToReset;

    public Color DeselectTint = new Color(0.8f, 0.8f, 0.8f);
    public float blendAmount = 5f;
    public float wipeDuration = 0.5f;

    private DimensionButton _selected = null;
    private DialControl _dialControl;
    private float _angle;

    private bool _transitioning = false;

    // Start is called before the first frame update
    void Start()
    {


        _dialControl = GetComponent<DialControl>();
        topHalfTex.transform.parent.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
        botHalfTex.transform.parent.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;

        // Reset dial with defaults
        foreach (DimensionButton db in dimensionsToReset)
        {
            db.dimension.SetAngle(0f, 0f);
            db.dimension.SetBlend(blendAmount);
        }

        _angle = _dialControl.GetAngle();
        botDimension = DefaultDimension;
        topDimension = DefaultDimension;
        DefaultDimension.dimension.SetAngle(_angle, 360f);
        DefaultDimension.dimension.SetBlend(blendAmount);

        UpdateDialSprites();
    }

    // Update is called once per frame
    void Update()
    {
        _angle = _dialControl.GetAngle();
        if (_transitioning) return;
        if (_selected != null) SelectionMode();
        else UpdateAngles();
    }

    private void UpdateAngles()
    {
        UpdateDimensionAngle(topDimension);
        UpdateDimensionAngle(botDimension);
    }
    private void UpdateDimensionAngle(DimensionButton dimension)
    {

        switch (dimension.dimension.state)
        {
            default:
            case DimensionButton.DimensionState.None:
            case DimensionButton.DimensionState.Full:
            case DimensionButton.DimensionState.HalfPos:
                dimension.dimension.SetAngle(_angle);
                break;
            
            case DimensionButton.DimensionState.HalfNeg:
                dimension.dimension.SetAngle(_angle + 180f);
                break;
        }
    }
    private void UpdateDimensionState(DimensionButton dimension, DimensionButton.DimensionState targetState, bool clockwiseWipe)
    {
        // Update collision instantly
        dimension.dimension.mask.SetState(targetState);
        dimension.dimension.SetBlend(blendAmount);

        // Run coroutine for visual wipe
        float targetSliceAmount = 0f;

        switch (targetState)
        {
            case DimensionButton.DimensionState.Full: targetSliceAmount = 360f; break;
            case DimensionButton.DimensionState.None: targetSliceAmount = 0f; break;
            case DimensionButton.DimensionState.HalfPos: targetSliceAmount = 180f; break;
            case DimensionButton.DimensionState.HalfNeg: targetSliceAmount = 180f; break;
        }

        StartCoroutine(SetDimensionSliceAmount(dimension, targetSliceAmount, clockwiseWipe));

        dimension.dimension.state = targetState;
    }

    private void SelectionMode()
    {
        // Mouseover selected half highlights
        topHalfTex.color = DeselectTint;
        botHalfTex.color = DeselectTint;
        GameObject dialHit = GetDialMouseOver();
        if (dialHit != null)
        {
            dialHit.transform.GetChild(0).GetComponent<Image>().color = Color.white;
        }

        if (Input.GetMouseButtonDown(0))
        {
            // Make selection
            if (dialHit != null)
            {
                bool switchTop = dialHit.CompareTag("Dial Top");
                if((switchTop ? topDimension : botDimension) == _selected)
                {
                    // selected dimension is identical, so stop
                    ExitSelectionMode();
                    return;
                }

                _transitioning = true;
                // Remove existing slice
                DimensionButton.DimensionState targetState = DimensionButton.DimensionState.None;
                if (topDimension == botDimension)
                {
                    if (switchTop)
                    {
                        targetState = DimensionButton.DimensionState.HalfNeg;
                        topDimension.dimension.SetAngle(_angle + 180f);
                    }
                    else
                    {
                        targetState = DimensionButton.DimensionState.HalfPos;
                    }
                }
                
                UpdateDimensionState(switchTop ? topDimension : botDimension, targetState, false);

                // Add new slice
                targetState = switchTop ? DimensionButton.DimensionState.HalfPos : DimensionButton.DimensionState.HalfNeg;
                if (_selected == (switchTop ? botDimension : topDimension))
                    targetState = DimensionButton.DimensionState.Full;
                else
                {
                    // Reset hidden slice
                    _selected.dimension.SetAngle(_angle + (switchTop ? 180f : 0f), 0f);
                    _selected.dimension.SetBlend(blendAmount);
                }
                
                UpdateDimensionState(_selected, targetState, true);


                if (switchTop) topDimension = _selected;
                else botDimension = _selected;
                UpdateDialSprites();
            }

        }
    }

    // Coroutine to wipe away a segment
    IEnumerator SetDimensionSliceAmount(DimensionButton dimension, float targetSliceAmount, bool clockwise)
    {
        float startAngle = dimension.dimension.GetAngle();
        float startSlice = dimension.dimension.GetSlice();
        float fullStep = targetSliceAmount - startSlice;

        float targetAngle = clockwise ? startAngle - fullStep : startAngle;
        float targetSlice = targetSliceAmount;


        Debug.Log("Angle: " + startAngle + "->" + targetAngle);
        Debug.Log("Slice: " + startSlice + "->" + targetSlice);

        for (float t = 0; t <= wipeDuration; t += Time.deltaTime)
        {
            float scaled_t = t / wipeDuration;
            float currAngle = Mathf.Lerp(startAngle, targetAngle, scaled_t);
            float currSlice = Mathf.Lerp(startSlice, targetSlice, scaled_t);
            dimension.dimension.SetAngle(currAngle, currSlice);
            yield return null;
        }
        dimension.dimension.SetAngle(targetAngle, targetSlice);

        ExitSelectionMode();
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
        _transitioning = false;
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
        if (_transitioning) return;
        _selected = dimension;

        // Disable normal dial controls
        _dialControl.Disable();

        // Apply tint to dial
        topHalfTex.color = DeselectTint;
        botHalfTex.color = DeselectTint;
    }
}
