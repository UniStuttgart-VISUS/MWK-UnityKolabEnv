using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using ExitGames.Client.Photon;
using Microsoft.Win32;
using UnityEngine;
using Valve.Newtonsoft.Json;

[DataContract]
public class EnvConstants : MonoBehaviour
{
    
    private static EnvConstants e_Instance = null;
    public Dictionary<string,string> cmdArgs = new Dictionary<string, string>();

    private void Start()
    {
        //Read from serialized JSON if available
        if (!DeserializeFromJSON(Application.persistentDataPath + "settings.json"))
        {
            if (!DeserializeFromJSON("default_settings.json"))
            {
                Debug.LogWarning("Found no per-user settings and no default_settings.json - reverting to failsafe defaults");   
            }
        }
        
        //Read command line arguments if present, overwrite read settings via reflection (slow but only done once)
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
                if (args[i].Substring(1).ToLower() == "fromloader")
                {
                    FromLoader = true;
                    Debug.Log("Started from Loader, production mode");
                }
                else Debug.LogWarning("Encountered argument with no matching property: " + args[i]);
            }
        }
        
        //Dump everything for debugging
        Debug.Log("Env init finished, values now are:");
        Debug.Log(instance);
    }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnApplicationQuit()
    {
        SerializeToJSON();
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var prop in typeof(EnvConstants).GetFields(BindingFlags.NonPublic | 
                                                            BindingFlags.Instance))
        {
            if (prop.Name != "instance")
            {
                var val = prop.GetValue(instance);
                var valStr = val == null ? "" : val.ToString();
                sb.AppendLine(prop.Name + ": " + valStr);
            }
        }

        sb.AppendLine("Listed " + typeof(EnvConstants)
                          .GetFields(BindingFlags.NonPublic | 
                                     BindingFlags.Instance)
                          .Length + " properties");

        return sb.ToString();
    }

    public static EnvConstants instance
    {
        get
        {
            if (e_Instance == null)
            {
                // FindObjectOfType() returns the first AManager object in the scene.
                e_Instance = GameObject.Find("_GLOBALS").GetComponent(typeof(EnvConstants)) as EnvConstants;
            }
 
            if (e_Instance == null)
            {
                var obj = new GameObject("EmergencyGlobalsManager");
                Debug.LogWarning("Created default globals store, please add a EnvConstant Script to some GameObject");
                e_Instance = obj.AddComponent<EnvConstants>();
            }
 
            return e_Instance;
        }
        private set => e_Instance = value;
    }

    public static void SerializeToJSON()
    {
        //Serialize
        string serialized = JsonUtility.ToJson(instance, true);
        //Write to disk
        try
        {
            File.WriteAllText(Application.persistentDataPath + "settings.json", serialized);
            Debug.Log("Wrote settings to default store");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to write serialized settings: "+e.Message);
        }
    }

    public static bool DeserializeFromJSON(string path)
    {
        try
        {
            using (var sr = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read)))
            {
                JsonUtility.FromJsonOverwrite(sr.ReadToEnd(),instance);
                Debug.Log("Successfully read settings from "+path);
                return true;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to read serialized settings: "+e.Message);
            return false;
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
    
    public static List<string> CollisionSN
    {
        get { return instance._collisionSN; }
        set { instance._collisionSN = value; }
    }

    public static string Nickname
    {
        get { return instance._nickname; }
        set { instance._nickname = value; }
    }

    public static string Institution
    {
        get { return instance._institution; }
        set { instance._institution = value; }
    }

    public static string Session
    {
        get { return instance._session; }
        set { instance._session = value; }
    }

    public static string Username
    {
        get { return instance._username; }
        set { instance._username = value; }
    }

    public static string Password
    {
        get { return instance._password; }
        set { instance._password = value; }
    }

    public static bool DesktopMode
    {
        get { return instance._desktopMode; }
        set { instance._desktopMode = value; }
    }

    public static bool FromLoader
    {
        get; set;
    } = false;

    
    [SerializeField]
    private string _nickname = "";
    [SerializeField]
    private string _institution= "";
    [SerializeField]
    private string _session = "";
    [SerializeField]
    private bool _useInviwoPositioning = true;
    [SerializeField]
    private bool _rttVisualization = false;
    [SerializeField]
    private bool _createRoomOnLoad = false;
    [SerializeField]
    private bool _autoJoinFirstRoomOnLoad = false;
    [SerializeField]
    private bool _externalRendererMode = false;
    [SerializeField]
    private bool _desktopMode = true;
    [SerializeField]
    private string _projectPath = "undefined";
    [SerializeField]
    private string _username = "undefined";
    [SerializeField]
    private string _password = "undefined";
    [SerializeField]
    private string _workspacesPath = "C:\\Users\\flo\\Documents\\KolabWorking\\inviwo\\";
    [SerializeField]
    private string _inviwoPath = "C:\\Users\\flo\\Documents\\KolabWorking\\inviwo\\build\\bin\\Debug\\inviwo.exe";
    [SerializeField]
    private string _megamolPath = "C:\\Users\\flo\\Documents\\KolabWorking\\inviwo\\build\\bin\\Debug\\inviwo.exe";
    [SerializeField]
    private List<string> _collisionSN = new List<string>();
}