using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FileLoaderHelper : MonoBehaviour
{
    public List<string> availableFiles = new List<string>();
    
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
        var fileInfo = info.GetFiles("*.inv");
        foreach (var file in fileInfo)
        {
            availableFiles.Add(file.FullName);
        }
        
        //Create objects per file
        for (int i = 0; i < availableFiles.Count; i++)
        {
            // Try read thumbnail from file
            Texture2D tex = parseXmlFileReadThumb(File.ReadAllText(availableFiles[i]));
            
            // Create obj
            var square = GameObject.CreatePrimitive(PrimitiveType.Plane);
            square.transform.parent = this.transform;
            square.transform.localPosition = new Vector3(-0.2f,Mathf.Floor(i/5)*0.65f,-1.5f+(i%5*0.8f));
            square.transform.localScale = new Vector3(0.05f,0.05f,0.05f);
            square.GetComponent<Renderer>().material = new Material(Shader.Find("Unlit/Texture"));
            square.GetComponent<Renderer>().material.mainTexture = tex;
            square.transform.RotateAround(square.transform.position, square.transform.forward, 90f);
            square.AddComponent<Button>().name = availableFiles[i];
            square.GetComponent<Button>().onClick.AddListener(OnClick);
        }
    }

    private void OnClick()
    {
        Debug.Log(EventSystem.current.currentSelectedGameObject.name);
        ExternalApplicationController.Instance.StartInviwoInstance(EventSystem.current.currentSelectedGameObject.name);
    }

    Texture2D parseXmlFileReadThumb(string xmlData){
        string totVal = "";
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load (new StringReader(xmlData));
        string xmlPathPattern = "//CanvasImage/base64/@content";
        XmlNode canvasNode  = xmlDoc.SelectSingleNode(xmlPathPattern);

        Texture2D tex = new Texture2D(512, 512);
        if (canvasNode != null)
        {
            //Create texture
            byte[] b64_bytes = System.Convert.FromBase64String(canvasNode.Value);
            tex.LoadImage(b64_bytes);
        }

        return tex;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
