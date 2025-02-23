using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialAngleHandler : MonoBehaviour
{
    public Transform dialHandTemplate;
    public Transform roomTemplate;
    public int numSplits;
    public List<Material> sliceMaterials;
    public float blendAmount = 0.1f;
    private List<float> angles;
    private DialControl dialControl;

    // Start is called before the first frame update
    void Start()
    {
        dialControl = GetComponent<DialControl>();

        dialHandTemplate.gameObject.SetActive(false);
        roomTemplate.gameObject.SetActive(false);

        if (numSplits <= 1)
        {
            numSplits = 0;
            Transform hand = Instantiate(dialHandTemplate);
            hand.gameObject.SetActive(true);
            Transform roomCopy = Instantiate(roomTemplate);
            roomCopy.gameObject.SetActive(true);
            roomCopy.parent = null;
            for (int j = 0; j < roomCopy.childCount; j++)
            {
                roomCopy.GetChild(j).GetComponent<Renderer>().material = sliceMaterials[0];
            }
            angles = new List<float>(1) { 0f };
            return;
        }
        if (numSplits > sliceMaterials.Count) numSplits = sliceMaterials.Count;

        // Create split objects
        float angleOffset = 360f / numSplits;
        float currAngle = 0f;
        angles = new List<float>(numSplits);
        for (int i = 0; i < numSplits; i++)
        {
            Transform hand = Instantiate(dialHandTemplate);
            hand.gameObject.SetActive(true);
            hand.SetParent(transform);
            hand.localPosition = Vector3.zero;
            Transform roomCopy = Instantiate(roomTemplate);
            roomCopy.gameObject.SetActive(true);
            roomCopy.parent = null;
            for(int j = 0; j < roomCopy.childCount; j++)
            {
                roomCopy.GetChild(j).GetComponent<Renderer>().material = sliceMaterials[i];
            }

            angles.Add(currAngle);
            hand.localRotation = Quaternion.Euler(0f, 0f, currAngle);

            currAngle += angleOffset;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Update the angles on materials
        float dialAngle = (dialControl.GetAngle() + 360f) % 360f;

        for(int i = 0; i < numSplits; i++)
        {
            float minAngle = (dialAngle + angles[i] + 360f) % 360f;
            float maxAngle = (dialAngle + angles[(i+1)%numSplits] + 360f) % 360f;
            Debug.DrawRay(Vector3.zero, 10f * new Vector3(Mathf.Cos(minAngle * Mathf.Deg2Rad), 0f, Mathf.Sin(minAngle * Mathf.Deg2Rad)));
            Debug.DrawRay(Vector3.zero, 10f * new Vector3(Mathf.Cos(maxAngle * Mathf.Deg2Rad), 0f, Mathf.Sin(maxAngle * Mathf.Deg2Rad)));

            sliceMaterials[i].SetFloat("_MinAngle", minAngle);
            sliceMaterials[i].SetFloat("_MaxAngle", maxAngle);
            sliceMaterials[i].SetFloat("_BlendAmount", blendAmount);
        }
    }
}
