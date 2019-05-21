using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
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

    
    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            //Player is local
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
            transform.Find("NameLabel").GetComponent<TextMesh>().text = photonView.Owner.NickName;
        }
        else
        {
            //For own player, only update name label
            transform.Find("NameLabel").GetComponent<TextMesh>().text = PhotonNetwork.LocalPlayer.NickName;
        }
        
        //Handle collision warning
        if (photonView.IsMine)
        {
            //Write own real position and reference lighthouses
            Hashtable worldPosProps = new Hashtable();
            worldPosProps.Add("realPos",InputTracking.GetLocalPosition(XRNode.CenterEye));
            worldPosProps.Add("referenceBases",JsonConvert.SerializeObject(EnvConstants.CollisionSN));
            PhotonNetwork.LocalPlayer.SetCustomProperties(worldPosProps);
            
            //Get other players real position and search for collisions
            collisionGO.SetActive(false);
            foreach (Player player in PhotonNetwork.PlayerListOthers)
            {
                Debug.Log(player.CustomProperties.ToString());
                List<string> refBases =
                    JsonConvert.DeserializeObject<List<string>>((string)player.CustomProperties["referenceBases"]);
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
        }
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
