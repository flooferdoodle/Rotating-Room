using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimensionButton : MonoBehaviour
{
    [System.Serializable]
    public struct Dimension
    {
        // Material bases (not variants) for changing shadergraphs
        public Material floorMatBase;
        public Material objectMatBase;

        // Mask Collider script for masking hitboxes
        public MaskColliders mask;

        // Dial Texture for switching dimensions
        public Sprite dialSprite;
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
