using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.Vive;
using interop;
using NetMQ;
using NetMQ.Sockets;
using Photon.Pun;
using UnityEngine;
using Valve.Newtonsoft.Json;

public class CuttingPlaneStateSender : MonoBehaviour, IJsonStringSendable {

    public string Name = "CuttingPlaneState";
    public bool lastSentValue = false;
    public GameObject objectToCut;
    
    // Start is called before the first frame update
    void Start()
    {
        
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
        bool currentState = objectToCut.GetComponent<Collider>().bounds.Contains(this.transform.position);
        lastSentValue = currentState;
        Debug.Log(lastSentValue);
        return "{\"value\":\""+currentState.ToString().ToLower()+ "\" }";
    }

    public bool hasChanged()
    {
        bool currentState = objectToCut.GetComponent<Collider>().bounds.Contains(this.transform.position);
        return (lastSentValue != currentState || Time.frameCount%60 == 0);
    }
}
