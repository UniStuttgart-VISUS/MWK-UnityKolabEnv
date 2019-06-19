using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(PhotonView))]
public class PlayerPosFromVRCamera : MonoBehaviourPun
{
    public Camera vrCamera;
    public Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        if (vrCamera == null)
        {
            vrCamera = Camera.main;
        }
        
        if (photonView.IsMine)
        {
            transform.Find("HeadSphere").Find("HMD").localScale = new Vector3(0.0f,0.0f,0.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = vrCamera.transform.position +  offset;
        transform.Find("HeadSphere").rotation = vrCamera.transform.rotation;
    }
}
