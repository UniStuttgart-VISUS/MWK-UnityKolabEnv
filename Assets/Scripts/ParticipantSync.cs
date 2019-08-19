using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using HTC.UnityPlugin.Vive;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.XR;
using Valve.Newtonsoft.Json;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ParticipantSync : MonoBehaviourPun, IPunObservable
{
    //List of the scripts that should only be active for the local player (ex. PlayerController, MouseLook etc.)
    public MonoBehaviour[] localScripts;
    //List of the GameObjects that should only be active for the local player (ex. Camera, AudioListener etc.)
    public GameObject[] localObject;
    Vector3 latestPos;
    Quaternion latestRot;
    private bool firstData = true;
    
    public GameObject collisionGO;
    private Recorder ownRecorder;
    private GameObject nameLabelMat;
    private Color targetColor = new Color(0.0f,0.0f,0.3f,0.7f);
    private SpriteRenderer speakerRenderer;
    private GameObject speaker;
    TextMesh labelMesh;

    
    // Start is called before the first frame update
    void Start()
    {
        nameLabelMat = transform.Find("NameLabel").gameObject;
        speaker = transform.Find("Speaker").gameObject;
        labelMesh = nameLabelMat.GetComponent<TextMesh>();
        speakerRenderer = speaker.GetComponent<SpriteRenderer>();
        
        if (photonView.IsMine)
        {
            //Player is local
            ownRecorder = GetComponent<Recorder>();
            //ownRecorder.Init();
        }
        else
        {
            //Player is Remote, deactivate the scripts and object that should only be enabled for the local player
            for (int i = 0; i < localScripts.Length; i++)
            {
                localScripts[i].enabled = false;
            }
            for (int i = 0; i < localObject.Length; i++)
            {
                localObject[i].SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Handle position
        if(firstData)
            transform.Find("NameLabel").GetComponent<TextMesh>().text = photonView.Owner.NickName;
               
        if (!photonView.IsMine)
        {
            //Initial positioning without smoothing
            if (firstData)
            {
                firstData = false;
                transform.position = latestPos;
                transform.rotation = latestRot;
            }
            
            //Update remote player (smooth this, this looks good, at the cost of some accuracy)
            transform.position = Vector3.Lerp(transform.position, latestPos, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, latestRot, Time.deltaTime * 5);
        }
        
        //Handle collision warning
        if (photonView.IsMine)
        {
            //Write own real position and reference lighthouses
            Hashtable worldPosProps = new Hashtable();
            worldPosProps.Add("realPos",InputTracking.GetLocalPosition(XRNode.CenterEye));
            worldPosProps.Add("referenceBases",JsonConvert.SerializeObject(EnvConstants.CollisionSN));
            if(ownRecorder.IsCurrentlyTransmitting) worldPosProps.Add("audioLevel", ownRecorder.LevelMeter.CurrentPeakAmp);
            else worldPosProps.Add("audioLevel", 0f);
            PhotonNetwork.LocalPlayer.SetCustomProperties(worldPosProps);
            
            //Get other players real position and search for collisions
            collisionGO.SetActive(false);
            foreach (Player player in PhotonNetwork.PlayerListOthers)
            {
                //Debug.Log(player.CustomProperties.ToString());
                List<string> refBases = new List<string>();
                try
                {
                    refBases =
                        JsonConvert.DeserializeObject<List<string>>((string) player.CustomProperties["referenceBases"]);
                }
                catch (Exception e)
                {
                    refBases = new List<string>();
                }

                if (EnvConstants.CollisionSN.Intersect(refBases).Any())
                {
                    //We are in the same tracking space, calculate if collision can occur
                    Vector3 realPos = (Vector3)player.CustomProperties["realPos"];
                    var direction = (realPos - InputTracking.GetLocalPosition(XRNode.CenterEye));
                    if (direction.magnitude < 2.0f)
                    {
                        //Visualize
                        collisionGO.SetActive(true);
                        collisionGO.transform.position = realPos;
                        Renderer[] children;
                        children = collisionGO.GetComponentsInChildren<Renderer>();
                        foreach(Renderer r in children) {
                            r.material.color = new Color(1.0f,0.0f,0.0f, 1.0f-direction.magnitude);
                        }
                        
                        //Output warning
                        //Debug.LogWarning("Collision warning - player "+player.NickName+" has distance of "+direction.magnitude+ " (Bases matching: "+EnvConstants.CollisionSN.Intersect(refBases).Count());
                    }
                }
            }
            
            //Handle Push-To-Talk
            if (ViveInput.GetPressDownEx(HandRole.RightHand, ControllerButton.Grip))
            {
                ownRecorder.TransmitEnabled = true;
            }
            else if(ViveInput.GetPressUpEx(HandRole.RightHand, ControllerButton.Grip))
            {
                ownRecorder.TransmitEnabled = false;
            }
        }
        
        //Show audio activity
        float level = 0.0f;
        try
        {
            level = (float) photonView.Owner.CustomProperties["audioLevel"];
        }
        catch
        {
            
        }
        
        if (level > 0.3f)
        {
            targetColor = new Color(1.0f,0.0f,0.1f,0.7f);
            speakerRenderer.color = new Color(1f,1f,1f,1f);
        }
        else
        {
            targetColor = new Color(0.0f,0.0f,0.3f,0.7f);
            speakerRenderer.color = new Color(1f,1f,1f,0f);
        }

        labelMesh.color = Color.Lerp(labelMesh.color, targetColor, Time.deltaTime*3);
        
        //Orient name label
        Vector3 targetDirPos = new Vector3(Camera.main.transform.position.x, nameLabelMat.transform.position.y, Camera.main.transform.position.z);
        nameLabelMat.transform.LookAt(2 * nameLabelMat.transform.position - targetDirPos);
        speaker.transform.LookAt(2 * speaker.transform.position - targetDirPos);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            //Network player, receive data
            latestPos = (Vector3)stream.ReceiveNext();
            latestRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
