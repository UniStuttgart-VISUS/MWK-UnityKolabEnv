using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.Vive;
using UnityEngine;

public class HandlePointerVisibility : MonoBehaviour
{
    public GameObject[] pointersToHide;
    private Collider m_Collider;
    
    // Start is called before the first frame update
    void Start()
    {
        m_Collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < pointersToHide.Length; i++)
        {
            if (m_Collider.bounds.Contains(pointersToHide[i].transform.position))
            {
                pointersToHide[i].transform.Find("Reticle").localScale = new Vector3(0f,0f,0f );
                pointersToHide[i].transform.Find("GuideLine").localScale = new Vector3(0f,0f,0f );
            }
            else
            {
                pointersToHide[i].transform.Find("Reticle").localScale = new Vector3(1f,1f,1f );
                pointersToHide[i].transform.Find("GuideLine").localScale = new Vector3(1f,1f,1f );
            }
        }
    }
}
