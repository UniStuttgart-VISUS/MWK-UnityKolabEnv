using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.Vive;
using interop;
using NetMQ;
using NetMQ.Sockets;
using Photon.Pun;
using UnityEngine;
using Valve.Newtonsoft.Json;

[RequireComponent(typeof(PhotonView))]
public class CuttingPlaneNormalSender : MonoBehaviourPun, IPunObservable, IJsonStringSendable {

    public bool isCutting = false;
    public string Name = "CuttingPlaneNormal";
    private Vector4 lastSentValue = new Vector4();
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Input
        if(!ViveInput.Active) Debug.LogWarning("ViveInput inactive");
            
        if (ViveInput.GetPressDownEx(HandRole.LeftHand, ControllerButton.Grip) && !isCutting)
        {
            Debug.Log("Start cut");
            //start grab            
            isCutting = true;
            Vector3 currentPos = VivePose.GetPoseEx(HandRole.LeftHand).pos;
            Vector3 currentVec = VivePose.GetPoseEx(HandRole.LeftHand).rot * VivePose.GetPoseEx(HandRole.LeftHand).up;
            //photon take ownership if necessary
            if (!this.photonView.IsMine)
            {
                this.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
            }
            
        } else if (ViveInput.GetPressUpEx(HandRole.LeftHand, ControllerButton.Grip) && isCutting) {
            //end grab
            Debug.Log("End cut");
            this.GetComponent<PhotonView>().TransferOwnership(0);
            isCutting = false;
        }
        else if(isCutting)
        {
            Vector3 currentPos = VivePose.GetPoseEx(HandRole.LeftHand).pos;
            Vector3 currentVec = VivePose.GetPoseEx(HandRole.LeftHand).rot * VivePose.GetPoseEx(HandRole.LeftHand).up;
        }

        //only use values from network if not grabbed locally
        if (!isCutting && !this.GetComponent<PhotonView>().IsMine)
        {
            Debug.Log("Update cut remote");
            //Add incoming network translation / rotation (not really accounting for collisions or concurrency right now)
            //transform.position = Vector3.Lerp(transform.position, latestPos, Time.deltaTime * 2);
            //this.transform.rotation = Quaternion.Lerp(transform.rotation, latestRot, Time.deltaTime * 2);
        }
    }
    
    public string nameString() {
        return this.Name;
    }

    public string jsonString()
    {
        Vector4 currentVec = transform.rotation * Vector3.down;
        lastSentValue = currentVec;
        return "{\"value\":"+JsonUtility.ToJson(currentVec)+ " }";
    }

    public bool hasChanged()
    {
        return lastSentValue != (Vector4)(transform.rotation * Vector3.down);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //nothing yet
    }
}
