﻿using System;
using System.Globalization;
using System.Text;
using UnityEngine;
using NetMQ;
using NetMQ.Sockets;
using UnityEngine.XR;
using Valve.Newtonsoft.Json;
using Valve.VR;

public class SendAll : MonoBehaviour
{
    public Camera baseCamera;
    private PublisherSocket _pubSocket;
    public GameObject netManager;
    public GameObject vrOrigin;
    public GameObject additionalRotationFrom;
    public int count = 0;
    public float multiplier = 1.0f;
    public GameObject localRttMatGO;
    public Vector3 offsetY;
    private float fovFade = 90;
    private Vector2Int t = new Vector2Int(100,100);
    
    // Start is called before the first frame update
    void Start()
    {
        if(baseCamera == null) baseCamera = Camera.main;       
    }

    // Update is called once per frame
    void Update()
    {
        if (_pubSocket == null && netManager != null)
        {
            _pubSocket = netManager.GetComponent<NetMQManager>()._pubSocket;
        }
        else if(_pubSocket == null)
        {
            return;
        }       

        Vector3 vec1 = (InputTracking.GetLocalPosition(XRNode.LeftEye) - offsetY - vrOrigin.transform.position) * multiplier;
        Vector3 vec2 = (InputTracking.GetLocalPosition(XRNode.RightEye) - offsetY - vrOrigin.transform.position) * multiplier;

        if(EnvConstants.DesktopMode)
        {
            vec1 = vec2 = baseCamera.transform.position * multiplier;
        }
       
        Vector3 vecFwd = baseCamera.transform.forward;
        Vector3 vecUp = baseCamera.transform.up;

        Quaternion addRotation = Quaternion.Inverse(additionalRotationFrom.transform.rotation);
        //addRotation.y = -addRotation.y;
           
        if (!EnvConstants.UseInviwoPositioning)
        {
            vec1 = vec1 / vec1.magnitude;
            vec2 = vec2 / vec2.magnitude;
        }

        if (additionalRotationFrom != null)
        {
            vec1 = RotatePointAroundPivot(vec1, new Vector3(0f, 0f, 0f), addRotation);
            vec2 = RotatePointAroundPivot(vec2, new Vector3(0f, 0f, 0f), addRotation );
            vecFwd =  RotatePointAroundPivot(vecFwd, new Vector3(0f, 0f, 0f), addRotation);
            vecUp =  RotatePointAroundPivot(vecUp, new Vector3(0f, 0f, 0f), addRotation);
        }

        if (count % 2 == 0 || true)
        {
            //Send camera continuously
            string jsonSend = "{" +
                                  "\"camVecCamL\":" + JsonUtility.ToJson(vec1) + "," +
                                  "\"camVecCamR\":" + JsonUtility.ToJson(vec2) + "," +
                                  "\"camUpCamL\":" + JsonUtility.ToJson(vecUp) + "," +
                                  "\"camUpCamR\":" + JsonUtility.ToJson(vecUp) + "," +
                                  "\"camFwdCamL\":" + JsonUtility.ToJson(vecFwd) + "," +
                                  "\"camFwdCamR\":" + JsonUtility.ToJson(vecFwd) +
                              "}";
            _pubSocket.SendMoreFrame("camera").SendFrame(jsonSend);
            
            //Test/Sample: Send fov once, defined as interaction property in inviwo
//            jsonSend = "{\"value\":"+JsonConvert.SerializeObject(t)+ " }";
//            _pubSocket.SendMoreFrame("res").SendFrame(jsonSend);
//            Debug.Log(jsonSend);
        }

        if (localRttMatGO != null)
        {
            int xPos = (int)Math.Floor((count % 100) * 9.75) - 500;
            localRttMatGO.GetComponent<Renderer>().material.SetInt("_X",xPos);
        }

        count++;
    }
         
    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion angles) {
        return angles * (point - pivot) + pivot;
    }
}   
