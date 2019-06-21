using UnityEngine;
using HTC.UnityPlugin.Vive;
using Photon.Pun;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class RotateZoomPan : MonoBehaviourPun, IPunObservable
{
    public bool isGrabbing;
    public string mode = "";
    public Vector3 initialPosL;

    public bool useInternalPan = true;
    public Vector3 initialPosR;
     
    private Vector3 latestPos;
    private Quaternion latestRot;
    private float latestZoom;
    private Vector3 latestPan;
    
    private Vector3 initialPos;
    private Quaternion initialRot;
    private float initialZoom;
    private Vector3 initialPan;

    private Collider m_Collider;
    private Transform origin;

    public GameObject internalPan;

    public Text statusMsg;
    
    void Start()
    {
        //Cache dataset collider ref
        m_Collider = GetComponent<Collider>();
        origin = GameObject.Find("_VROrigin").transform;
        mode = "init";
    }

    // Update is called once per frame
    void Update()
    {
        //Sanity check
        if(!ViveInput.Active) Debug.LogWarning("ViveInput inactive");
        
        //Status overlay
        statusMsg.text = mode;

        //Collider update
        m_Collider = GetComponent<Collider>();
        
        //LOCAL Interaction  - differentiate for pan, zoom, rotate
        if (m_Collider.bounds.Contains(VivePose.GetPoseEx(HandRole.RightHand, origin).pos) &&
            m_Collider.bounds.Contains(VivePose.GetPoseEx(HandRole.LeftHand, origin).pos) &&
            ViveInput.GetPressDownEx(HandRole.RightHand, ControllerButton.Trigger) &&
            ViveInput.GetPressDownEx(HandRole.LeftHand, ControllerButton.Trigger) && 
            (!isGrabbing || (isGrabbing && mode != "zoom")))
        {
            startZoom();
        } else if (m_Collider.bounds.Contains(VivePose.GetPoseEx(HandRole.RightHand, origin).pos) &&
                   ViveInput.GetPressDownEx(HandRole.RightHand, ControllerButton.Trigger) && !isGrabbing)
        {
            startRotate();
        } else if (m_Collider.bounds.Contains(VivePose.GetPoseEx(HandRole.LeftHand, origin).pos) &&
                   ViveInput.GetPressDownEx(HandRole.LeftHand, ControllerButton.Trigger) && !isGrabbing)
        {
            startPan();
        } else if (m_Collider.bounds.Contains(VivePose.GetPoseEx(HandRole.RightHand, origin).pos) &&
                   ViveInput.GetPressDownEx(HandRole.RightHand, ControllerButton.Trigger) && isGrabbing && mode == "pan")
        {
            //Upgrade pan to zoom
            startZoom();
        } else if (m_Collider.bounds.Contains(VivePose.GetPoseEx(HandRole.LeftHand, origin).pos) &&
                   ViveInput.GetPressDownEx(HandRole.LeftHand, ControllerButton.Trigger) && isGrabbing && mode == "rotate")
        {
            //Upgrade rotate to zoom
            startZoom();
        }
        
        //Grab already occured, only update dependent on mode
        else if (isGrabbing)
        {
            if (mode == "zoom")
            {
                float zoomRaw = (VivePose.GetPoseEx(HandRole.LeftHand, origin).pos - VivePose.GetPoseEx(HandRole.RightHand, origin).pos).magnitude - (initialPosL - initialPosR).magnitude;
                float scaleVal = initialZoom + zoomRaw;
                transform.localScale = new Vector3(scaleVal, scaleVal, scaleVal);
                latestZoom = scaleVal;
            } 
            else if (mode == "rotate")
            {
                Vector3 currentPos = VivePose.GetPoseEx(HandRole.RightHand, origin).pos;
                Vector3 eulerRot = new Vector3();
                eulerRot.z = -((initialPosR.z - currentPos.z) * 100) % 360;
                eulerRot.y = ((initialPosR.x - currentPos.x) * 100) % 360;
                eulerRot.x = -((initialPosR.y - currentPos.y) * 100) % 360;
                Quaternion additionalRotation = Quaternion.Euler(eulerRot);
                transform.rotation = additionalRotation * initialRot;
                latestRot = transform.rotation;
            } 
            else if(mode == "pan")
            {
                latestPan = initialPosL - VivePose.GetPoseEx(HandRole.LeftHand, origin).pos;
                if (!useInternalPan) transform.position = initialPos - latestPan;
                else internalPan.transform.localPosition = initialPos - latestPan;
                latestPan = transform.position;
            }
        }
        
        //Grab release
        if (isGrabbing && (ViveInput.GetPressUpEx(HandRole.RightHand, ControllerButton.Trigger) || (ViveInput.GetPressUpEx(HandRole.LeftHand, ControllerButton.Trigger))))
        {
            Debug.Log("End "+mode+" grab");
            this.GetComponent<PhotonView>().TransferOwnership(0);
            isGrabbing = false;
            mode = "none";
        }
        
        //REMOTE Interaction - only use values from network if not grabbed locally
        if (!isGrabbing && !this.GetComponent<PhotonView>().IsMine)
        {
            //Add incoming network translation / rotation (not really accounting for collisions or concurrency right now)
            //transform.position = Vector3.Lerp(transform.position, latestPos, Time.deltaTime * 2);
            this.transform.rotation = Quaternion.Lerp(transform.rotation, latestRot, Time.deltaTime * 2);
            this.transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(latestZoom, latestZoom, latestZoom), Time.deltaTime * 2);
            this.transform.position = Vector3.Lerp(transform.position, latestPan, Time.deltaTime * 2);
            this.internalPan.transform.localPosition = Vector3.Lerp(transform.position, latestPan, Time.deltaTime * 2);
        }
    }

    private void startPan()
    {
        //Pan mode
        Debug.Log("Pan start");
        isGrabbing = true;
        mode = "pan";
        initialPosR = VivePose.GetPoseEx(HandRole.RightHand, origin).pos;
        initialPosL = VivePose.GetPoseEx(HandRole.LeftHand, origin).pos;
        if (!useInternalPan) initialPos = transform.position;
        else initialPos = internalPan.transform.localPosition;
        
        //Take PUN ownership if necessary
        if (!this.photonView.IsMine)
        {
            this.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
        }
    }

    private void startRotate()
    {
        //Rotate mode
        Debug.Log("Rotate start");
        isGrabbing = true;
        mode = "rotate";
        initialPosR = VivePose.GetPoseEx(HandRole.RightHand, origin).pos;
        initialPosL = VivePose.GetPoseEx(HandRole.LeftHand, origin).pos;
        initialRot = transform.rotation;
        
        //Take PUN ownership if necessary
        if (!this.photonView.IsMine)
        {
            this.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
        }
    }

    private void startZoom()
    {
        //Zoom mode
        Debug.Log("Zoom start");
        isGrabbing = true;
        mode = "zoom";
        initialPosR = VivePose.GetPoseEx(HandRole.RightHand, origin).pos;
        initialPosL = VivePose.GetPoseEx(HandRole.LeftHand, origin).pos;
        initialZoom = transform.localScale.x;
            
        //Take PUN ownership if necessary
        if (!this.photonView.IsMine)
        {
            this.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //We own this dataset: send the others our data
            stream.SendNext(transform.rotation);
            stream.SendNext(transform.localScale.x);
            if(!useInternalPan) stream.SendNext(transform.position);
            else stream.SendNext(internalPan.transform.localPosition);
        }
        else
        {
            //Network dataset change, receive data
            latestRot = (Quaternion)stream.ReceiveNext();
            latestZoom = (float) stream.ReceiveNext();
            latestPan = (Vector3) stream.ReceiveNext();
        }
    }
}
