using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimensionButton : MonoBehaviour
{
    public enum DimensionState
    {
        Full,
        None,
        HalfPos,
        HalfNeg
    }

    [System.Serializable]
    public class Dimension
    {
        public Transform DimensionRoom;

        public Material floorMatBase;
        public Material objectMatBase;

        // Mask Collider script for masking hitboxes
        public MaskColliders mask;

        // Dial Texture for switching dimensions
        public Sprite dialSprite;

        public DimensionState state;

        public float GetAngle() { return floorMatBase.GetFloat("_AngleOffset"); }
        public float GetSlice() { return floorMatBase.GetFloat("_SliceAngle"); }
        public void SetAngle(float theta, float slice)
        {
            SetAngle(theta);
            floorMatBase.SetFloat("_SliceAngle", slice);
            objectMatBase.SetFloat("_SliceAngle", slice);
        }
        public void SetAngle(float theta)
        {
            floorMatBase.SetFloat("_AngleOffset", theta);
            objectMatBase.SetFloat("_AngleOffset", theta);
            mask.SetAngle(theta);
        }
        public void SetBlend(float blendAmount)
        {
            floorMatBase.SetFloat("_BlendAmount", blendAmount);
            objectMatBase.SetFloat("_BlendAmount", blendAmount);
        }
    }

    public Dimension dimension;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
