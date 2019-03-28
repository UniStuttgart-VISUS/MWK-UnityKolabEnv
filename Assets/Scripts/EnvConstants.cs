using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvConstants : MonoBehaviour
{

    private static EnvConstants e_Instance = null; 
 
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
                var obj = new GameObject("AManager");
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
 
    public bool _useInviwoPositioning = true;
    public bool _rttVisualization = false;
    public bool _createRoomOnLoad = false;
    public bool _autoJoinFirstRoomOnLoad = true;
    public bool _externalRendererMode = false;
}