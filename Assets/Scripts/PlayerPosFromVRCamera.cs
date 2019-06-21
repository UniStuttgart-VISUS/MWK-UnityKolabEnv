using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(PhotonView))]
public class PlayerPosFromVRCamera : MonoBehaviourPun
{
    public GameObject vrCamera;
    public Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        if (vrCamera == null)
        {
            GameObject[] cams = GameObject.FindGameObjectsWithTag("MainCamera");
            if (EnvConstants.DesktopMode)
            {
                vrCamera = GameObject.Find("Camera");
            }
            else
            {
                vrCamera = GameObject.Find("CamLeft");
            }

            Debug.LogWarning("Using found main camera as VR camera!");
        }
        
        if (photonView.IsMine)
        {
            transform.Find("HeadSphere").Find("HMD").localScale = new Vector3(0.0f,0.0f,0.0f);
            transform.Find("Body").localScale = new Vector3(0.0f,0.0f,0.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = vrCamera.transform.position +  offset;
        transform.position = new Vector3(vrCamera.transform.position.x +  offset.x,
                                            1.5f,
                                            vrCamera.transform.position.z +  offset.z);
        transform.Find("HeadSphere").rotation = vrCamera.transform.rotation;
    }
}
