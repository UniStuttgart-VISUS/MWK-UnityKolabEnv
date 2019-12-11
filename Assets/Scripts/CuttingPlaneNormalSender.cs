using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.Vive;
using interop;
using NetMQ;
using NetMQ.Sockets;
using Photon.Pun;
using UnityEngine;
using Valve.Newtonsoft.Json;

public class CuttingPlaneNormalSender : MonoBehaviour, IJsonStringSendable {

    public string Name = "CuttingPlaneNormal";
    private Vector3 lastSentValue = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 currentValue = new Vector3(1.0f, 0.0f, 0.0f);
    private CuttingPlaneStateSender stateSender;
    public GameObject objectToCut;

    // Start is called before the first frame update
    void Start()
    {
        stateSender = GetComponent<CuttingPlaneStateSender>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentVec = Quaternion.Inverse(objectToCut.transform.rotation) * (transform.rotation * Vector3.up);
        currentVec = convert.toOpenGL(currentVec);
        currentValue = currentVec;
    }
    
    public string nameString() {
        return this.Name;
    }

    public string jsonString()
    {
        lastSentValue = currentValue;
        return "{\"value\":"+JsonUtility.ToJson(currentValue)+ " }";
    }

    public bool hasChanged()
    {
        return lastSentValue != currentValue;
    }
}
