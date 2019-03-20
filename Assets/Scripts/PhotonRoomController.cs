using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using HTC.UnityPlugin.Vive;

public class PhotonRoomController : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public Transform spawnPoint;
    
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.CurrentRoom == null)
        {
            Debug.Log("Not in the room, returning back to Lobby");
            UnityEngine.SceneManagement.SceneManager.LoadScene("Workspaces");
            return;
        }

        //We're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, Quaternion.identity, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (ViveInput.GetPressDownEx(HandRole.RightHand, ControllerButton.Grip))
        {
            PhotonNetwork.LeaveRoom();
        }
    }
    
    public override void OnLeftRoom()
    {
        //We have left the Room, return back to the GameLobby
        UnityEngine.SceneManagement.SceneManager.LoadScene("Workspaces");
    }
}
