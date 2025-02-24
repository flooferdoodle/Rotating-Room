using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitHandlerSingleton : MonoBehaviour
{
    // Singleton Setup
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


    public int numRegions = 3;
    public List<Texture> wallTextures;
    public List<Texture> floorTextures;


    public Transform roomParents;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
