using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HTC.UnityPlugin.Vive;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class GrabAndRotate : MonoBehaviourPun, IPunObservable
{
    public bool isGrabbing;
    public Vector3 initialPos;
    public Vector3 additionalRotation;
    private Vector3 initialRotation;
    
    Vector3 latestPos;
    Quaternion latestRot;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!ViveInput.Active) Debug.LogWarning("ViveInput inactive");
            
        if (ViveInput.GetPressDownEx(HandRole.RightHand, ControllerButton.Trigger) && !isGrabbing)
        {
            Debug.Log("Start grab");
            //start grab            
            isGrabbing = true;
            initialPos = VivePose.GetPoseEx(HandRole.RightHand).pos;
            additionalRotation = new Vector3();
            initialRotation = transform.rotation.eulerAngles;
            //photon take ownership if necessary
            if (!this.photonView.IsMine)
            {
                this.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
            }
        } else if (ViveInput.GetPressUpEx(HandRole.RightHand, ControllerButton.Trigger) && isGrabbing) {
            //end grab
            Debug.Log("End grab");
            this.GetComponent<PhotonView>().TransferOwnership(0);
            isGrabbing = false;
        }
        else if(isGrabbing)
        {
            Debug.Log("Update grab");
            Vector3 currentPos = VivePose.GetPoseEx(HandRole.RightHand).pos;
            additionalRotation.z = -((initialPos.x - currentPos.x) * 100) % 360;
            additionalRotation.y = -((initialPos.y - currentPos.y) * 100) % 360;
            additionalRotation.x = -((initialPos.z - currentPos.z) * 100) % 360;
            this.transform.rotation = Quaternion.Euler(additionalRotation) * Quaternion.Euler(initialRotation);
        }

        //only use values from network if not grabbed locally
        if (!isGrabbing && !this.GetComponent<PhotonView>().IsMine)
        {
            Debug.Log("Update grab remote");
            //Add incoming network translation / rotation (not really accounting for collisions or concurrency right now)
            //transform.position = Vector3.Lerp(transform.position, latestPos, Time.deltaTime * 2);
            this.transform.rotation = Quaternion.Lerp(transform.rotation, latestRot, Time.deltaTime * 2);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //We own this player: send the others our data
            Debug.Log("Send: "+transform.rotation);
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            //Network player, receive data
            Debug.Log("Receive: "+latestRot);
            latestPos = (Vector3)stream.ReceiveNext();
            latestRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
