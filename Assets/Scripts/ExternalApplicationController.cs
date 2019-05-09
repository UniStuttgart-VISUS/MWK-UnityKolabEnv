using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class ExternalApplicationController : Singleton<ExternalApplicationController>
{
    private Process inviwoProcess;
    private Process megamolProcess;
    
    // Start is called before the first frame update
    void Start()
    {
        //Check if applications exist
        
    }

    // Update is called once per frame
    void Update()
    {
        //Keep track of running processes
        if (inviwoProcess != null && !inviwoProcess.Responding)
        {
            //do what?
        }
    }

    private void OnApplicationQuit()
    {
        if(inviwoProcess != null) inviwoProcess.Kill();
        if(megamolProcess != null) megamolProcess.Kill();
    }

    private void OnDestroy()
    {
        if(inviwoProcess != null) inviwoProcess.Kill();
        if(megamolProcess != null) megamolProcess.Kill();
    }

    public void StartInviwoInstance(string workspaceFile)
    {
        //Check if others are running and kill
        if(inviwoProcess != null && !inviwoProcess.HasExited) inviwoProcess.Kill();
        
        //Start instance
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.LoadUserProfile = true;
        startInfo.FileName = EnvConstants.InviwoPath;
        startInfo.Arguments = "-n -c -w "+workspaceFile;
        inviwoProcess = Process.Start(startInfo);
    }

    public void StartMegamolInstance()
    {
        //Check if others are running and kill
        if(megamolProcess != null && !megamolProcess.HasExited) megamolProcess.Kill();
        
        //Start instance
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.LoadUserProfile = true;
        startInfo.FileName = EnvConstants.MegamolPath;
        //startInfo.Arguments = "Extra Arguments to Pass to the Program";
        megamolProcess = Process.Start(startInfo);
    }
}
