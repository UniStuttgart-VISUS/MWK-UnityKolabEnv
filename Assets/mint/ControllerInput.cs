using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Photon.Pun;

public class ControllerInput : MonoBehaviour {

    protected GameObject collidingObject = null;
    protected GameObject objectInHand = null;

    private GameObject proxy;

    public SteamVR_Input_Sources handType;
    public SteamVR_Action_Boolean grabActiveAction;

    bool logging = false;

    void log(string s){
        if(logging)
            print(s);
    }

	// Use this for initialization
	void Start () {
        proxy = new GameObject("GrabProxyObject");
	}
	
	// Update is called once per frame
	void Update () {
        proxy.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);

        if (grabActiveAction.GetLastStateDown(handType) && collidingObject)
        {
            GrabObject();
            Debug.Log("picked up object");
        }

        if (grabActiveAction.GetLastStateUp(handType) && objectInHand)
        {
            ReleaseObject();
            Debug.Log("picked up object");
        }
	}

    public void OnTriggerEnter(Collider other)
    {
        log("OnTriggerEnter");
        SetCollidingObject(other);
    }

    public void OnTriggerStay(Collider other)
    {
        log("OnTriggerStay");
        SetCollidingObject(other);
    }

    public void OnTriggerExit(Collider other)
    {
        log("OnTriggerExit");
        RemoveCollidingObject(other);
    }

    public void SetCollidingObject(Collider other)
    {
        if (collidingObject || !other.GetComponent<Rigidbody>())
            return;

        collidingObject = other.gameObject;
    }

    public void RemoveCollidingObject(Collider other)
    {
        if (!collidingObject)
            return;

        collidingObject = null;
    }

    public void GrabObject()
    {
        if (!collidingObject)
            return;
        
        if(!collidingObject.GetComponent<PhotonView>().IsMine)
            collidingObject.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);

        objectInHand = collidingObject;        
        var io = objectInHand.GetComponent<InteractiveObject>();
        if(io)
            io.Pickup(proxy);
    }

    public void ReleaseObject()
    {
        if (!objectInHand)
            return;

        objectInHand.GetComponent<PhotonView>().TransferOwnership(0);

        var io = objectInHand.GetComponent<InteractiveObject>();
        if(io)
            io.Release(proxy);
    }
}
