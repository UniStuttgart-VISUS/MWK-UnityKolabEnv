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
    private Vector3 lastSentValue = new Vector3();
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
        
    }
    
    public string nameString() {
        return this.Name;
    }

    public string jsonString()
    {
        Vector3 currentVec = Quaternion.Inverse(objectToCut.transform.rotation) * (transform.rotation * Vector3.up);//Quaternion.Inverse(objectToCut.transform.rotation) * (transform.rotation * Vector3.down);
        currentVec.z = -currentVec.z;
        lastSentValue = currentVec;
        return "{\"value\":"+JsonUtility.ToJson(currentVec)+ " }";
    }

    public bool hasChanged()
    {
        return stateSender.lastSentValue;
    }
}
