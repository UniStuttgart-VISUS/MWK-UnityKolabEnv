using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.Vive;
using Photon.Pun;
using UnityEngine;

public class AnimTrigger : MonoBehaviourPun
{
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ViveInput.GetPressDownEx(HandRole.RightHand, ControllerButton.DPadUp))
        {
            //anim.Play("ExplodeAnimation");
            this.photonView.RPC("PlayRemote", RpcTarget.AllBuffered, "ExplodeAnimation");
        }
        
        if (ViveInput.GetPressDownEx(HandRole.RightHand, ControllerButton.DPadDown))
        {
            //anim.Play("Unexplode");
            this.photonView.RPC("PlayRemote", RpcTarget.AllBuffered, "Unexplode");
        }
    }

    
    [PunRPC]
    void PlayRemote(string name)
    {
        anim.Play(name);
    }
}
