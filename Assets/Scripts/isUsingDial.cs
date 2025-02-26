using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isUsingDial : MonoBehaviour
{
    public bool dialUse;
    // Start is called before the first frame update
    void Start()
    {
        dialUse = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            dialUse = !dialUse;
            Debug.Log("dialUse is now: " + dialUse);
        }

    }
}
