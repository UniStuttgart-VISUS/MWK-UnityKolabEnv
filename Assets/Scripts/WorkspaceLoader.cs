using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WorkspaceLoader : MonoBehaviourPunCallbacks
{
    public string workspaceName = "Main";
    public string appVersion = "0.01";
    public string defaultRoomName = "TEST";
    
    private List<RoomInfo> ownWorkspaces = new List<RoomInfo>();
    private bool isJoining = false;

    public Canvas leftUI;
    public Canvas centerUI;
    public Canvas rightUI;

    private Button createRoomButton;
    private Button refreshButton;
    private Text connStatusText;
    private Text userNameText;

    public GameObject buttonPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        //Hack: get refs for more flexible structure w/o prefabs
        createRoomButton = GameObject.Find("CreateRoomButton").GetComponent<UnityEngine.UI.Button>();
        refreshButton = GameObject.Find("RefreshButton").GetComponent<UnityEngine.UI.Button>();
        
        createRoomButton.onClick.AddListener(createClick);
        refreshButton.onClick.AddListener(refreshClick);
        
        connStatusText = GameObject.Find("ConnStatusText").GetComponent<UnityEngine.UI.Text>();
        userNameText = GameObject.Find("UserNameText").GetComponent<UnityEngine.UI.Text>();
        
        //Photon Startup
        PhotonNetwork.AutomaticallySyncScene = true;
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = appVersion;
            PhotonNetwork.ConnectUsingSettings();
        }

        connStatusText.text = "Disconnected";
        //From config files
        userNameText.text = EnvConstants.Nickname;
        PhotonNetwork.NickName = EnvConstants.Nickname;
    }

    // Update is called once per frame
    void Update()
    {
        if (isJoining) connStatusText.text = "Joining...";
        DateTime currentTime = System.DateTime.Now;        
    }
    
    /*
     * UI Parts / Looby 
     */
    void createClick()
    {
        defaultRoomName = EnvConstants.Session;
        Debug.Log("Create room with name "+defaultRoomName);
        if (defaultRoomName != "")
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsOpen = true;
            roomOptions.IsVisible = true;
            roomOptions.MaxPlayers = (byte)7; //Set any number

            PhotonNetwork.JoinOrCreateRoom(defaultRoomName, roomOptions, TypedLobby.Default);
           
        }
    }
    
    void refreshClick()
    {
        Debug.Log("Refresh");
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinLobby(TypedLobby.Default);
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    void joinClick(string name)
    {
        Debug.Log("Joining "+name);
        PhotonNetwork.JoinRoom(name);
    }
    
    /*
     * Networking Callback Parts
     */
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("OnFailedToConnectToPhoton. StatusCode: " + cause.ToString() + " ServerAddress: " + PhotonNetwork.ServerAddress);
        connStatusText.text = "Disconnected";
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        connStatusText.text = "Connected";
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        
        if (EnvConstants.CreateRoomOnLoad && !EnvConstants.FromLoader)
        {
            createClick();
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ownWorkspaces = roomList;
        Debug.Log("RoomListUpdate - Rooms: "+ownWorkspaces.Count.ToString());
        foreach (Button b in centerUI.transform.GetComponentsInChildren<Button>())
        {
            Destroy(b.transform.gameObject);
        }
        foreach (RoomInfo info in ownWorkspaces)
        {
            GameObject newButton = Instantiate(buttonPrefab) as GameObject;
            newButton.transform.SetParent(centerUI.transform, false);
            newButton.transform.Find("RoomText").GetComponent<Text>().text = info.Name;
            newButton.transform.Find("CountsText").GetComponent<Text>().text = info.PlayerCount + "/" + info.MaxPlayers;
            newButton.GetComponent<Button>().onClick.AddListener(delegate{joinClick(info.Name);});
        }
        if (ownWorkspaces.Any(i => i.Name == EnvConstants.Session) && EnvConstants.FromLoader) {
            //We have a production session w/ session name, and an existing session
            PhotonNetwork.JoinRoom(EnvConstants.Session);
            Debug.Log("Joining room for requested session: " + EnvConstants.Session);
        } else if (!ownWorkspaces.Any(i => i.Name == EnvConstants.Session) && EnvConstants.FromLoader) {
            //We have a production session w/ session name, but no existing session, so we create one
            Debug.Log("No room for requested session, creating one: " + EnvConstants.Session);
            RoomOptions roomOptions = new RoomOptions
            {
                IsOpen = true,
                IsVisible = true,
                MaxPlayers = (byte)10
            };
            PhotonNetwork.JoinOrCreateRoom(EnvConstants.Session, roomOptions, TypedLobby.Default);
        }
        else if (ownWorkspaces.Count > 0 && EnvConstants.AutoJoinFirstRoomOnLoad && !EnvConstants.FromLoader) {
            joinClick(ownWorkspaces[0].Name);
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("OnCreateRoomFailed got called. This can happen if the room exists (even if not visible). Try another room name.");
        isJoining = false;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRoomFailed got called. This can happen if the room is not existing or full or closed.");
        isJoining = false;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed got called. This can happen if the room is not existing or full or closed.");
        isJoining = false;
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom");
        PhotonNetwork.NickName = EnvConstants.Nickname;
        PhotonNetwork.LoadLevel(workspaceName);
    }

    public override void OnJoinedRoom()
    {
        connStatusText.text = "Joined";
        Debug.Log("OnJoinedRoom: "+PhotonNetwork.CurrentRoom.Name);
    }
}
