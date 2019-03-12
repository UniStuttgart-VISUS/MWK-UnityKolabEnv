using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPosFromVRCamera : MonoBehaviour
{
    public Camera vrCamera;
    // Start is called before the first frame update
    void Start()
    {
        if (vrCamera == null)
        {
            vrCamera = Camera.main;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = vrCamera.transform.position;
        transform.Find("HeadSphere").rotation = vrCamera.transform.rotation;
    }
}
