using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using interop;
using UnityEngine.UIElements;
using UnityEngine.WSA;
using Debug = UnityEngine.Debug;

public interface IRenderingProcess
{
    string rendererName();

    bool isOwnedFiletype(string filename);

    List<FileInfo> filterOwnWorkspaceFiles(List<FileInfo> filenames);

    Texture loadWorkspacePreview(string filename);
    TransferFunction loadTransferFunction(string filename);
    Process startRendering(string filename);
}

public class InviwoRenderingProcess : IRenderingProcess
{
    public string rendererName()
    {
        return "Inviwo";
    }

    public bool isOwnedFiletype(string filename)
    {
        return filename.EndsWith(".inv");
    }

    public List<FileInfo> filterOwnWorkspaceFiles(List<FileInfo> filenames)
    {
        return filenames.FindAll(f => (isOwnedFiletype(f.FullName)));
    }

    public Texture loadWorkspacePreview(string filename)
    {
        var fileContents = File.ReadAllText(filename);

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load (new StringReader(fileContents));
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

    public TransferFunction loadTransferFunction(string filename)
    {
        var fileContents = File.ReadAllText(filename);
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load (new StringReader(fileContents));
        string xmlPathPattern = ".//TransferFunction";
        XmlNode tfNode  = xmlDoc.SelectSingleNode(xmlPathPattern);
        TransferFunction tf = new TransferFunction();
        tf.points = new List<TfPoint>();
               
        if (tfNode != null)
        {
            try
            {
                //Create tf
                tf.type = Int32.Parse(tfNode.SelectSingleNode("type/@content").Value);
                tf.maskMax = float.Parse(tfNode.SelectSingleNode("maskMax/@content").Value);
                tf.maskMin = float.Parse(tfNode.SelectSingleNode("maskMin/@content").Value);
                XmlNodeList points = tfNode.SelectSingleNode("Points").ChildNodes;
                foreach (XmlNode p in points)
                {
                    TfPoint tfp = new TfPoint();
                    tfp.pos = float.Parse(p.SelectSingleNode("pos/@content").Value, CultureInfo.InvariantCulture);
                    tfp.rgba.x = float.Parse(p.SelectSingleNode("rgba/@x").Value, CultureInfo.InvariantCulture);
                    tfp.rgba.y = float.Parse(p.SelectSingleNode("rgba/@y").Value, CultureInfo.InvariantCulture);
                    tfp.rgba.z = float.Parse(p.SelectSingleNode("rgba/@z").Value, CultureInfo.InvariantCulture);
                    tfp.rgba.w = float.Parse(p.SelectSingleNode("rgba/@w").Value, CultureInfo.InvariantCulture);
                    tf.points.Add(tfp);
                }
            }
            catch (Exception)
            {
                Debug.LogWarning("TF parsing failed! Dump: "+tfNode.InnerText);
            }
        }
        return tf;
    }

    public Process startRendering(string filename)
    {
        //Start instance
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.LoadUserProfile = true;
        startInfo.FileName = EnvConstants.InviwoPath;
        startInfo.Arguments = "-n -c -w "+filename;
        return Process.Start(startInfo); // TODO: return only startinfo and somebody else starts renderer?
    }
}

public class MegaMolRenderingProcess : IRenderingProcess
{
    public string rendererName()
    {
        return "MegaMol";
    }

    public bool isOwnedFiletype(string filename)
    {
        return filename.EndsWith(".mmprj");
    }

    public List<FileInfo> filterOwnWorkspaceFiles(List<FileInfo> filenames)
    {
        return filenames.FindAll(f => (isOwnedFiletype(f.FullName)));
    }


    public Texture loadWorkspacePreview(string filename)
    {
        return null;
    }

    public TransferFunction loadTransferFunction(string filename)
    {
        Debug.LogWarning("No loading for Megamol tfs");
        TransferFunction tf = new TransferFunction();
        tf.points = new List<TfPoint>();
        return tf;
    }

    public Process startRendering(string filename)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.LoadUserProfile = true;
        startInfo.FileName = EnvConstants.MegamolPath;
        startInfo.Arguments = "-p \"" + filename + "\" -i Project_1 inst"; // mmconsole.exe started with arguments for project file generated by MegaMolConfigurator
        return Process.Start(startInfo);
    }
}

public class MintRenderingProcess : IRenderingProcess
{
    public string rendererName()
    {
        return "Mint Rendering";
    }

    public bool isOwnedFiletype(string filename)
    {
        return filename.EndsWith(".exe");
    }

    public List<FileInfo> filterOwnWorkspaceFiles(List<FileInfo> filenames)
    {
        return filenames.FindAll(f => (isOwnedFiletype(f.FullName)));
    }

    public Texture loadWorkspacePreview(string filename)
    {
        return null;
    }

    public TransferFunction loadTransferFunction(string filename)
    {
        Debug.LogWarning("No loading for Megamol tfs");
        return new TransferFunction();
    }

    public Process startRendering(string filename)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.LoadUserProfile = true;
        startInfo.FileName = filename;
        startInfo.Arguments = "";
        return Process.Start(startInfo);
    }
}

public class ExternalApplicationController : Singleton<ExternalApplicationController>
{
    private List<Process> m_runningRenderingProcesses = new List<Process>();
    
    // Start is called before the first frame update
    void Start()
    {
        //Check if applications exist
    }

    // Update is called once per frame
    void Update()
    {
        //Keep track of running processes
        foreach(var p in m_runningRenderingProcesses)
            if (p != null && !p.Responding)
            {
                //do what?
            }
    }

    private void OnApplicationQuit()
    {
        this.OnDestroy();
    }

    private void closeProcess(Process p)
    {
        if(p.HasExited)
            return;

        p.Kill();
    }

    private void OnDestroy()
    {
        m_runningRenderingProcesses.ForEach(closeProcess);
        m_runningRenderingProcesses.Clear();
    }

    public void addRendererInstance(Process proc)
    {
        m_runningRenderingProcesses.Add(proc);
    }

    // TODO: how to close one of the running processes?

    public void closeAllRendererInstances()
    {
        this.OnDestroy();
        // TODO: reset dataset object such that newly created renderers are treated correctly? (i.e. Bbox receiving script waits for new bbox bounds)
    }
}
