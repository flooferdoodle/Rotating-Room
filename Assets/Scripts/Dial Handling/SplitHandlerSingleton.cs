using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitHandlerSingleton : MonoBehaviour
{
    // Singleton Setup
    #region Singleton
    private static SplitHandlerSingleton _instance;
    public static SplitHandlerSingleton Instance { get { return _instance; } }

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else
        {
            _instance = this;
        }
    }
    #endregion

    public DialControl Dial;

    

    private float _angle;
    public float GetAngle() { return _angle % 360f; }

    public struct RoomMaterialData {
        // Underlying texture data for swapping
        Material floorMatBase;
        Material objectMatBase;
        MaskColliders mask;
    }

    public List<RoomMaterialData> roomsMaterialData;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Update angle
        _angle = Dial.GetAngle();

        // Debug, draw the split line
        Vector3 splitLine = new Vector3(Mathf.Cos(_angle), Mathf.Sin(_angle), 0f);
        Debug.DrawRay(Vector3.zero, splitLine * 10f, Color.cyan);
    }
}
