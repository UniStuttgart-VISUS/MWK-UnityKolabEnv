using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;

public class FileLoaderHelper : MonoBehaviour
{
    public List<string> availableFiles = new List<string>();
    public List<string> handledFiles = new List<string>();
    
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
            availableFiles.Add(file.FullName);

        foreach(var renderer in EnvConstants.externalRenderers)
            handledFiles.AddRange(renderer.filterOwnWorkspaceFiles(availableFiles));

        //Create objects per file
        int i = 0;
        foreach(var hf in handledFiles)
        {
            var file = hf;
            Texture tex = EnvConstants.externalRenderers.Find(renderer => renderer.isOwnedFiletype(file)).loadWorkspacePreview(file);

            // Create obj
            var square = GameObject.CreatePrimitive(PrimitiveType.Plane);
            square.transform.parent = this.transform;
            square.transform.localPosition = new Vector3(0.0f,-Mathf.Floor(i/5)*0.65f-0.5f,-(i%5*0.8f)-0.5f);
            square.transform.localScale = new Vector3(0.05f,0.05f,0.05f);
            square.GetComponent<Renderer>().material = new Material(Shader.Find("Unlit/Texture"));
            square.GetComponent<Renderer>().material.mainTexture = tex;
            square.transform.RotateAround(square.transform.position, square.transform.forward, 90f);
            square.AddComponent<Button>().name = file;
            square.GetComponent<Button>().onClick.AddListener(OnClick);
            i++;
        }
        Debug.Log("file setup done: " + handledFiles.Count + " files");
    }

    private void OnClick()
    {
        string fullFilename = EventSystem.current.currentSelectedGameObject.name;
        string relativeFilePath = fullFilename.Replace(EnvConstants.WorkspacesPath, "");

        //this.startRenderingProcess(relativeFilePath);

        PhotonView photonView = PhotonView.Get(this);
        PhotonNetwork.OpCleanRpcBuffer(photonView); // remove RPCs for datasets loaded before
        photonView.RPC("startRenderingProcess", RpcTarget.AllBufferedViaServer, relativeFilePath);
    }

    [PunRPC]
    public void startRenderingProcess(string relativeWorkspaceFilePath)
    {
        string myWorkspaceDir = EnvConstants.WorkspacesPath.TrimEnd(new char[] { '\\' }); // remove trailing backslash from workspace path
        string filename = myWorkspaceDir + relativeWorkspaceFilePath;

        // TODO: instantiate Dataset GameObject here, before the renderer is started?

        ExternalApplicationController.Instance.closeAllRendererInstances();

        var renderProcess = EnvConstants.externalRenderers.Find(renderer => renderer.isOwnedFiletype(filename)).startRendering(filename);
        ExternalApplicationController.Instance.addRendererInstance(renderProcess);

        GameObject.Find("MintDataset").GetComponent<BoundingBoxCornersJsonReceiver>().reset();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
