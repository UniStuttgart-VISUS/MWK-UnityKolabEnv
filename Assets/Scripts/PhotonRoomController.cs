using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using HTC.UnityPlugin.Vive;
using Photon.Realtime;

public class PhotonRoomController : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public Transform spawnPoint;
    
    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.InRoom)
        {
            Debug.Log("Not in a room, Photon networking in offline mode");
            PhotonNetwork.OfflineMode = true;
        }

        // Spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, Quaternion.identity, 0);
    }

    // Update is called once per frame
    void Update()
    {
//        if (ViveInput.GetPressDownEx(HandRole.RightHand, ControllerButton.Grip))
//        {
//            PhotonNetwork.LeaveRoom();
//        }
    }
    
    public override void OnLeftRoom()
    {
        //We have left the Room, return back to the GameLobby
        UnityEngine.SceneManagement.SceneManager.LoadScene("Workspaces");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        EnvConstants.instance.showTooltip(newPlayer.NickName+" entered room", ToolTipLevel.INFO);
    }
    
    public override void OnPlayerLeftRoom(Player gonePlayer)
    {
        EnvConstants.instance.showTooltip(gonePlayer.NickName+" left room",ToolTipLevel.INFO);
    }
}
