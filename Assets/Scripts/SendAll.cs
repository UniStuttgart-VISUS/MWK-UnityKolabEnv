using System;
using System.Text;
using UnityEngine;
using NetMQ;
using NetMQ.Sockets;
using UnityEngine.XR;
using Valve.VR;

public class SendAll : MonoBehaviour
{
    public Camera baseCamera;
    private PublisherSocket _pubSocket;
    public GameObject netManager;
    public GameObject vrOrigin;
    public GameObject additionalRotationFrom;
    public int count = 0;
    public float multiplier = 1.0f;
    public GameObject localRttMatGO;
    public Vector3 offsetY;
    
    // Start is called before the first frame update
    void Start()
    {
        if(baseCamera == null) baseCamera = Camera.main;

        // Setup render values
        uint w = 0, h = 0;
        OpenVR.System.GetRecommendedRenderTargetSize(ref w, ref h);
        var sceneWidth = (float)w;
        var sceneHeight = (float)h;
        
        float l_left = 0.0f, l_right = 0.0f, l_top = 0.0f, l_bottom = 0.0f;
        OpenVR.System.GetProjectionRaw(EVREye.Eye_Left, ref l_left, ref l_right, ref l_top, ref l_bottom);
        
        
        float r_left = 0.0f, r_right = 0.0f, r_top = 0.0f, r_bottom = 0.0f;
        OpenVR.System.GetProjectionRaw(EVREye.Eye_Right, ref r_left, ref r_right, ref r_top, ref r_bottom);
        
        var tanHalfFov = new Vector2(
            Mathf.Max(-l_left, l_right, -r_left, r_right),
            Mathf.Max(-l_top, l_bottom, -r_top, r_bottom));

        var textureBounds = new VRTextureBounds_t[2];

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

        var aspect = tanHalfFov.x / tanHalfFov.y;
        var fieldOfView = 2.0f * Mathf.Atan(tanHalfFov.y) * Mathf.Rad2Deg;

        Debug.LogWarning("Tex size recommended:" + sceneWidth + " by " + sceneHeight + " px");
        Debug.LogWarning("Aspect: " +aspect);
        Debug.LogWarning("FoV: " +fieldOfView);

        for (int i = 0; i < 2; i++)
        {
            Debug.LogWarning("TexBounds "+i+" uMin: "+textureBounds[i].uMin.ToString("f4"));
            Debug.LogWarning("TexBounds "+i+" Left "+(sceneWidth-(sceneWidth*textureBounds[i].uMin))+" px");
            Debug.LogWarning("TexBounds "+i+" uMax: "+textureBounds[i].uMax.ToString("f4"));
            Debug.LogWarning("TexBounds "+i+" Right "+(sceneWidth-(sceneWidth*textureBounds[i].uMax))+" px");
            Debug.LogWarning("TexBounds "+i+" vMin: "+textureBounds[i].vMin.ToString("f4"));
            Debug.LogWarning("TexBounds "+i+" Top "+(sceneHeight-(sceneHeight*textureBounds[i].vMin))+" px");
            Debug.LogWarning("TexBounds "+i+" vMax: "+textureBounds[i].vMax.ToString("f4"));
            Debug.LogWarning("TexBounds "+i+" Bottom "+(sceneHeight-(sceneHeight*textureBounds[i].vMax))+" px");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_pubSocket == null && netManager != null)
        {
            _pubSocket = netManager.GetComponent<NetMQManager>()._pubSocket;
        }
        else if(_pubSocket == null)
        {
            return;
        }       

        Vector3 vec1 = (InputTracking.GetLocalPosition(XRNode.LeftEye) - offsetY - vrOrigin.transform.position) * multiplier;
        Vector3 vec2 = (InputTracking.GetLocalPosition(XRNode.RightEye) - offsetY - vrOrigin.transform.position) * multiplier;
       
        Vector3 vecFwd = baseCamera.transform.forward;
        Vector3 vecUp = baseCamera.transform.up;

        Vector3 addRotation = additionalRotationFrom.transform.eulerAngles;       
           
        if (!EnvConstants.UseInviwoPositioning)
        {
            vec1 = vec1 / vec1.magnitude;
            vec2 = vec2 / vec2.magnitude;
        }

        if (additionalRotationFrom != null)
        {
            vec1 = RotatePointAroundPivot(vec1, new Vector3(0f, 0f, 0f), addRotation);
            vec2 = RotatePointAroundPivot(vec2, new Vector3(0f, 0f, 0f), addRotation );
            vecFwd =  RotatePointAroundPivot(vecFwd, new Vector3(0f, 0f, 0f), addRotation);
            vecUp =  RotatePointAroundPivot(vecUp, new Vector3(0f, 0f, 0f), addRotation);
        }

        if (count % 2 == 0 || true)
        {
            string jsonSend = "{" +
                                  "\"seqNum\":" + count + "," +
                                  "\"camVecCamL\":" + JsonUtility.ToJson(vec1) + "," +
                                  "\"camVecCamR\":" + JsonUtility.ToJson(vec2) + "," +
                                  "\"camUpCamL\":" + JsonUtility.ToJson(vecUp) + "," +
                                  "\"camUpCamR\":" + JsonUtility.ToJson(vecUp) + "," +
                                  "\"camFwdCamL\":" + JsonUtility.ToJson(vecFwd) + "," +
                                  "\"camFwdCamR\":" + JsonUtility.ToJson(vecFwd) +
                              "}";
            _pubSocket.SendMoreFrame("camera").SendFrame(jsonSend);
        }

        if (localRttMatGO != null)
        {
            int xPos = (int)Math.Floor((count % 100) * 9.75) - 500;
            localRttMatGO.GetComponent<Renderer>().material.SetInt("_X",xPos);
        }

        count++;
    }
         
    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }
}   
