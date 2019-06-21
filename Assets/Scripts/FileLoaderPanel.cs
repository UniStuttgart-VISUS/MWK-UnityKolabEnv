using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;

public class FileLoaderPanel : MonoBehaviour
{
    public List<FileInfo> availableFiles = new List<FileInfo>();
    public List<FileInfo> handledFiles = new List<FileInfo>();

    public GameObject entryPrefab;
    public GameObject contentTarget;
    public GameObject detailPanel;
    public RawImage detailPanelImage;
    public Text detailPanelFilename;
    public Text detailPanelPath;
    public Button detailPanelOK;
    public Button detailPanelOKSaved;
    public Text detailPanelIsCurrent;
    public GameObject TFEditorRef;
    public GameObject LoadingIndicatorRef;
    
    // Start is called before the first frame update
    void Start()
    {
        //Scan working dir
        var info = new DirectoryInfo(EnvConstants.WorkspacesPath);
        if (!info.Exists)
        {
            Debug.LogWarning("Workspaces path not found, skipping enumeration");
            return;
        }

        var fileInfo = info.GetFiles();
        foreach (var file in fileInfo)
            availableFiles.Add(file);

        foreach(var renderer in EnvConstants.externalRenderers)
            handledFiles.AddRange(renderer.filterOwnWorkspaceFiles(availableFiles));

        //Add internal rendering file on top
        //List items
        GameObject intEntry = (GameObject) Instantiate(entryPrefab, contentTarget.transform);
        intEntry.GetComponent<Button>().onClick.AddListener(ElementClick);
        LoaderEntryData intdata = intEntry.GetComponent<LoaderEntryData>();
        intdata.filenameText.text = "gears.fbx";
        intdata.rendererText.text = "Internal";
        intdata.pathText.text = "";
        
        //Create objects per file
        int i = 0;
        foreach(var hf in handledFiles)
        {
            var file = hf;
            Texture tex = EnvConstants.externalRenderers.Find(renderer => renderer.isOwnedFiletype(file.Extension)).loadWorkspacePreview(file.FullName);
          
            //List items
            GameObject newEntry = (GameObject) Instantiate(entryPrefab, contentTarget.transform);
            newEntry.GetComponent<Button>().onClick.AddListener(ElementClick);
            LoaderEntryData data = newEntry.GetComponent<LoaderEntryData>();
            data.filenameText.text = file.Name;
            data.rendererText.text = EnvConstants.externalRenderers.Find(renderer => renderer.isOwnedFiletype(file.Extension))
                .rendererName();
            data.pathText.text = file.FullName;
            data.itemImage.texture = tex;
            data.filepath = file.FullName;
            data.fileinfo = file;
        }
        
        //Add button events for OK buttons
        detailPanelOK.onClick.AddListener(DetailOKClick);
        detailPanelOKSaved.onClick.AddListener(DetailOKSavedClick);
        
        Debug.Log("file setup done: " + handledFiles.Count + " files");
    }

    private void DetailOKClick()
    {
        Debug.Log("Trigger open " + detailPanelPath.text);
        string fullFilename = detailPanelPath.text;
        string relativeFilePath = fullFilename.Replace(EnvConstants.WorkspacesPath, "");

        PhotonView photonView = PhotonView.Get(this);
        PhotonNetwork.OpCleanRpcBuffer(photonView); // remove RPCs for datasets loaded before
        photonView.RPC("startRenderingProcess", RpcTarget.AllBufferedViaServer, relativeFilePath);
        
        TFEditorRef.GetComponent<TransferFunctionEditor>().startRenderingProcess(relativeFilePath);
    }

    private void DetailOKSavedClick()
    {
        
    }

    private void ElementClick()
    {
        Debug.Log("Click");
        LoaderEntryData ld = EventSystem.current.currentSelectedGameObject.GetComponent<LoaderEntryData>();
        
        //HACK: Handle internal case
        if (ld.rendererText.text == "Internal")
        {
            detailPanelImage.texture = new Texture2D(200,200);
            detailPanelPath.text = ld.filenameText.text;
            detailPanelFilename.text = ld.filenameText.text;
        }
        else
        {
            detailPanelImage.texture = EnvConstants.externalRenderers
                .Find(renderer => renderer.isOwnedFiletype(ld.filepath)).loadWorkspacePreview(ld.filepath);
            detailPanelPath.text = ld.fileinfo.FullName;
            detailPanelFilename.text = ld.fileinfo.Name;
        }
        detailPanelOK.interactable = true;
    }

    [PunRPC]
    public void startRenderingProcess(string relativeWorkspaceFilePath)
    {
        if (relativeWorkspaceFilePath.EndsWith("fbx"))
        {
            //Switch to internal
            EnvConstants.ExternalRendererMode = false;
            GameObject.Find("_KolabStateManager").GetComponent<KolabStateInit>().SwitchRendererMode();
        }
        else
        {
            //Switch to external
            //Notify all of loading
            GameObject[] gos;
            gos = GameObject.FindGameObjectsWithTag("FileLoaderAffected"); 
 
            for(var i = 0; i < gos.Length; i++){
                gos[i].SendMessage("FileLoadingStatus", "triggered");
            }
            
            //Reconfigure
            EnvConstants.ExternalRendererMode = true;
            GameObject.Find("_KolabStateManager").GetComponent<KolabStateInit>().SwitchRendererMode();
            
            string myWorkspaceDir = EnvConstants.WorkspacesPath;///.TrimEnd(new char[] { '\\' }); // remove trailing backslash from workspace path
            string filename = myWorkspaceDir + relativeWorkspaceFilePath;

            // TODO: instantiate Dataset GameObject here, before the renderer is started?

            ExternalApplicationController.Instance.closeAllRendererInstances();

            var renderProcess = EnvConstants.externalRenderers.Find(renderer => renderer.isOwnedFiletype(filename)).startRendering(filename);
            ExternalApplicationController.Instance.addRendererInstance(renderProcess);

            GameObject.Find("MintDataset").GetComponent<BoundingBoxCornersJsonReceiver>().reset();
        }
    }
    
    public void FileLoadingStatus(string status)
    {
        if (status == "triggered")
        {
            LoadingIndicatorRef.transform.position = new Vector3(0f,1.5f,0f);
        } else if(status == "finished")
        {
            LoadingIndicatorRef.transform.position = new Vector3(0f,15f,0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
