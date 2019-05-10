using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.Vive;
using NetMQ;
using NetMQ.Sockets;
using Photon.Pun;
using UnityEngine;
using Valve.Newtonsoft.Json;

[RequireComponent(typeof(PhotonView))]
public class CuttingPlane : MonoBehaviour, IPunObservable
{
    public bool isCutting;
    private PublisherSocket _pubSocket;
    public GameObject netManager;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Pub socket
        if (_pubSocket == null && netManager != null)
        {
            _pubSocket = netManager.GetComponent<NetMQManager>()._pubSocket;
        }
        else if(_pubSocket == null)
        {
            return;
        }
        
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
            /*if (!this.photonView.IsMine)
            {
                this.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
            }*/
            
            //send to inviwo
            var jsonSendPos = "{\"value\":"+JsonUtility.ToJson(currentPos)+ " }";
            var jsonSendNormal = "{\"value\":"+JsonUtility.ToJson(currentVec)+ " }";
            _pubSocket.SendMoreFrame("clipPos").SendFrame(jsonSendPos);
            _pubSocket.SendMoreFrame("clipNormal").SendFrame(jsonSendNormal);
            //Debug.Log(jsonSend);
            
        } else if (ViveInput.GetPressUpEx(HandRole.LeftHand, ControllerButton.Grip) && isCutting) {
            //end grab
            Debug.Log("End cut");
            this.GetComponent<PhotonView>().TransferOwnership(0);
            isCutting = false;
        }
        else if(isCutting)
        {
            Debug.Log("Update cut");
            Vector3 currentPos = VivePose.GetPoseEx(HandRole.LeftHand).pos;
            Vector3 currentVec = VivePose.GetPoseEx(HandRole.LeftHand).rot * VivePose.GetPoseEx(HandRole.LeftHand).up;
            
            //send to inviwo
            var jsonSendPos = "{\"value\":"+JsonUtility.ToJson(currentPos)+ " }";
            var jsonSendNormal = "{\"value\":"+JsonUtility.ToJson(currentVec)+ " }";
            _pubSocket.SendMoreFrame("clipPos").SendFrame(jsonSendPos);
            _pubSocket.SendMoreFrame("clipNormal").SendFrame(jsonSendNormal);
            //Debug.Log(jsonSend);
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //nothing yet
    }
}
