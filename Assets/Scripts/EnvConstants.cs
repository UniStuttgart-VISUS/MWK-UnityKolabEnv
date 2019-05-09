using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnvConstants : MonoBehaviour
{
    
    private static EnvConstants e_Instance = null;
    public Dictionary<string,string> cmdArgs = new Dictionary<string, string>();

    private void Start()
    {
        //Read command line arguments if present, overwrite constants via reflection (slow but only done once)
        string[] args = System.Environment.GetCommandLineArgs ();
        Debug.Log ("Received args: "+args);
        string input = "";
        for (int i = 0; i < args.Length; i++) {
            if (!args[i].StartsWith("-")) continue;
            try
            {
                PropertyInfo pi = typeof(EnvConstants).GetProperty(args[i].Substring(1),BindingFlags.IgnoreCase |  BindingFlags.Public | BindingFlags.Instance |  BindingFlags.Static);
                pi.SetValue(EnvConstants.instance, Convert.ChangeType(args[i+1], pi.PropertyType), null);
                Debug.Log("Set "+pi.Name+" from args to value "+args[i+1]);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Encountered argument with no matching property: "+args[i]);
            }
        }
    }

    public static EnvConstants instance
    {
        get
        {
            if (e_Instance == null)
            {
                // FindObjectOfType() returns the first AManager object in the scene.
                e_Instance = FindObjectOfType(typeof(EnvConstants)) as EnvConstants;
            }
 
            if (e_Instance == null)
            {
                var obj = new GameObject("EmergencyGlobalsManager");
                Debug.LogWarning("Created default globals store, please add a EnvConstant Script to some GameObject");
                e_Instance = obj.AddComponent<EnvConstants>();
            }
 
            return e_Instance;
        }
    }

    //Here comes our actual env values (both prop and field)
    
    public static bool UseInviwoPositioning
    {
        get { return instance._useInviwoPositioning; }
        set { instance._useInviwoPositioning = value; }
    }
    
    public static bool RttVisualization
    {
        get { return instance._rttVisualization; }
        set { instance._rttVisualization = value; }
    }
    
    public static bool CreateRoomOnLoad
    {
        get { return instance._createRoomOnLoad; }
        set { instance._createRoomOnLoad = value; }
    }
    
    public static bool AutoJoinFirstRoomOnLoad
    {
        get { return instance._autoJoinFirstRoomOnLoad; }
        set { instance._autoJoinFirstRoomOnLoad = value; }
    }

    public static bool ExternalRendererMode
    {
        get { return instance._externalRendererMode; }
        set { instance._externalRendererMode = value; }
    }

    public static string ProjectPath
    {
        get { return instance._projectPath; }
        set { instance._projectPath = value; }
    }

    public static string WorkspacesPath
    {
        get { return instance._workspacesPath; }
        set { instance._workspacesPath = value; }
    }

    public static string InviwoPath
    {
        get { return instance._inviwoPath; }
        set { instance._inviwoPath = value; }
    }

    public static string MegamolPath
    {
        get { return instance._megamolPath; }
        set { instance._megamolPath = value; }
    }

    public bool _useInviwoPositioning = true;
    public bool _rttVisualization = false;
    public bool _createRoomOnLoad = true;
    public bool _autoJoinFirstRoomOnLoad = false;
    public bool _externalRendererMode = false;
    public string _projectPath = "undefined";
    public string _workspacesPath = "C:\\Users\\flo\\Documents\\KolabWorking\\inviwo\\";
    public string _inviwoPath = "C:\\Users\\flo\\Documents\\KolabWorking\\inviwo\\build\\bin\\Debug\\inviwo.exe";
    public string _megamolPath = "C:\\Users\\flo\\Documents\\KolabWorking\\inviwo\\build\\bin\\Debug\\inviwo.exe";
}