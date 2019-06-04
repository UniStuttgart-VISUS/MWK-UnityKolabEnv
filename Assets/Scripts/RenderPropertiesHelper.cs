using System.Collections;
using System.Collections.Generic;
using NetMQ;
using NetMQ.Sockets;
using UnityEngine;
using UnityEngine.XR;
using Valve.Newtonsoft.Json;
using Valve.VR;

public class RenderPropertiesHelper : MonoBehaviour
{
    //Rendering params
    public Vector2 hmdResolution;
    public Vector2 renderBaseResolution;
    public Vector2Int motionAdaptedResolution;
    public Vector2 initialPositionBoth;
    public Vector2 adaptedPositionLeft;
    public Vector2 adaptedPositionRight;
    public VRTextureBounds_t[] textureBounds;
    public float recommendedFOV;
    public float recommendedAspect;
    public string trackingSystem;

    public bool setMaterialPropsOnStart = true;
    public GameObject LeftEyeGO;
    public GameObject RightEyeGO;
    public RenderTexture LeftEyeRT;
    public RenderTexture RightEyeRT;
    
    private PublisherSocket _pubSocket;
    public GameObject netManager;
    
    //Internal helpers
    private Vector3 _lastEyePosition = new Vector3(0,0,0);
    public float velocity3D;
    
    // Start is called before the first frame update
    void Start()
    {
        // Check for available HMD
        trackingSystem = OpenVR.IVRSystem_Version;

        if (!OpenVR.IsHmdPresent())
        {
            Debug.LogWarning("No HMD present, render properties init - desktop mode");
            EnvConstants.DesktopMode = true;
            return;
        }
        
        // Initially, determine rendering parameters, save them and display on startup once in debugLog
        uint w = 0, h = 0;
        OpenVR.System.GetRecommendedRenderTargetSize(ref w, ref h);
        var sceneWidth = (float)w;
        var sceneHeight = (float)h;
        hmdResolution = new Vector2(w,h);
        
        float l_left = 0.0f, l_right = 0.0f, l_top = 0.0f, l_bottom = 0.0f;
        OpenVR.System.GetProjectionRaw(EVREye.Eye_Left, ref l_left, ref l_right, ref l_top, ref l_bottom);
        
        
        float r_left = 0.0f, r_right = 0.0f, r_top = 0.0f, r_bottom = 0.0f;
        OpenVR.System.GetProjectionRaw(EVREye.Eye_Right, ref r_left, ref r_right, ref r_top, ref r_bottom);
        
        var tanHalfFov = new Vector2(
            Mathf.Max(-l_left, l_right, -r_left, r_right),
            Mathf.Max(-l_top, l_bottom, -r_top, r_bottom));

        // Get raw texture bounds
        textureBounds = new VRTextureBounds_t[2];

        textureBounds[0].uMin = 0.5f + 0.5f * l_left / tanHalfFov.x;
        textureBounds[0].uMax = 0.5f + 0.5f * l_right / tanHalfFov.x;
        textureBounds[0].vMin = 0.5f - 0.5f * l_bottom / tanHalfFov.y;
        textureBounds[0].vMax = 0.5f - 0.5f * l_top / tanHalfFov.y;

        textureBounds[1].uMin = 0.5f + 0.5f * r_left / tanHalfFov.x;
        textureBounds[1].uMax = 0.5f + 0.5f * r_right / tanHalfFov.x;
        textureBounds[1].vMin = 0.5f - 0.5f * r_bottom / tanHalfFov.y;
        textureBounds[1].vMax = 0.5f - 0.5f * r_top / tanHalfFov.y;
        
        // Grow the recommended size to account for the overlapping fov
        sceneWidth = sceneWidth / Mathf.Max(textureBounds[0].uMax - textureBounds[0].uMin, textureBounds[1].uMax - textureBounds[1].uMin);
        sceneHeight = sceneHeight / Mathf.Max(textureBounds[0].vMax - textureBounds[0].vMin, textureBounds[1].vMax - textureBounds[1].vMin);

        // Calculate initial base resolution
        renderBaseResolution = new Vector2(sceneWidth, sceneHeight);
        
        // Calculate aspect / fov
        recommendedAspect = tanHalfFov.x / tanHalfFov.y;
        recommendedFOV = 2.0f * Mathf.Atan(tanHalfFov.y) * Mathf.Rad2Deg; 
        
        // Per-Eye Texture Settings
        initialPositionBoth = new Vector2(renderBaseResolution.x/2, renderBaseResolution.y/2);
        
        // Calculate actual position for left eye w/ bounds
        adaptedPositionLeft = new Vector2(-(initialPositionBoth.x)+((1.0f - textureBounds[0].uMax) * sceneWidth),
                                          -(initialPositionBoth.y)-(textureBounds[0].vMin * sceneHeight));
        //Debug.LogWarning(textureBounds[0].uMin + " ---- " + textureBounds[0].uMax);

        // Calculate actual position for right eye w/ bounds
        adaptedPositionRight = new Vector2(-(initialPositionBoth.x)-(textureBounds[1].uMin * sceneWidth),
                                           -(initialPositionBoth.y)-(textureBounds[1].vMin * sceneHeight));
        //Debug.LogWarning(textureBounds[1].uMin + " ---- " + textureBounds[1].uMax);
        
        // Set material props
        if (setMaterialPropsOnStart)
        {
            //Left
            LeftEyeGO.GetComponent<Renderer>().material.SetFloat("_X",adaptedPositionLeft.x);
            LeftEyeGO.GetComponent<Renderer>().material.SetFloat("_Y",adaptedPositionLeft.y);
            LeftEyeGO.GetComponent<Renderer>().material.SetFloat("_Width",renderBaseResolution.x);
            LeftEyeGO.GetComponent<Renderer>().material.SetFloat("_Height",renderBaseResolution.y);
            //LeftEyeRT.width = (int) renderBaseResolution.x;
            //LeftEyeRT.height = (int) renderBaseResolution.y;
            
            //Right
            RightEyeGO.GetComponent<Renderer>().material.SetFloat("_X",adaptedPositionRight.x);
            RightEyeGO.GetComponent<Renderer>().material.SetFloat("_Y",adaptedPositionRight.y);
            RightEyeGO.GetComponent<Renderer>().material.SetFloat("_Width",renderBaseResolution.x);
            RightEyeGO.GetComponent<Renderer>().material.SetFloat("_Height",renderBaseResolution.y);
            //RightEyeRT.width = (int) renderBaseResolution.x;
            //RightEyeRT.height = (int) renderBaseResolution.y;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Pub socket
        if (_pubSocket == null && netManager != null)
        {
            _pubSocket = netManager.GetComponent<NetMQManager>()._pubSocket;
        }
        else if(_pubSocket == null)
        {
            return;
        }
        
        //Calculate current velocity (apply minimal smoothing)
        Vector3 currentEyePosition = InputTracking.GetLocalPosition(XRNode.LeftEye);
        velocity3D = (0.02f * velocity3D) + (0.98f * Vector3.Distance(currentEyePosition, _lastEyePosition));
        _lastEyePosition = currentEyePosition;

        if (velocity3D > 0.03)
        {
            motionAdaptedResolution = new Vector2Int((int)Mathf.Floor(renderBaseResolution.x/3), (int)Mathf.Floor(renderBaseResolution.y/3));
        } 
        else if (velocity3D > 0.01)
        {
            motionAdaptedResolution = new Vector2Int((int)Mathf.Floor(renderBaseResolution.x/2), (int)Mathf.Floor(renderBaseResolution.y/2));
        }
        else
        {
            motionAdaptedResolution = new Vector2Int((int)Mathf.Floor(renderBaseResolution.x/2), (int)Mathf.Floor(renderBaseResolution.y/2));
        }
        
        //Send resolution
        var jsonSend = "{\"value\":"+JsonConvert.SerializeObject(motionAdaptedResolution)+ " }";
        _pubSocket.SendMoreFrame("res").SendFrame(jsonSend);
        //Debug.Log(jsonSend);
        
    }
}
