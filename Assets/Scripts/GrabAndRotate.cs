using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HTC.UnityPlugin.Vive;

public class GrabAndRotate : MonoBehaviour
{
    public bool isGrabbing;
    public Vector3 initialPos;
    public Vector3 additionalRotation;
    private Vector3 initialRotation;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ViveInput.GetPressDownEx(HandRole.RightHand, ControllerButton.Trigger) && !isGrabbing)
        {
            //start grab
            isGrabbing = true;
            initialPos = VivePose.GetPoseEx(HandRole.RightHand).pos;
            additionalRotation = new Vector3();
            initialRotation = transform.rotation.eulerAngles;
        } else if (ViveInput.GetPressUpEx(HandRole.RightHand, ControllerButton.Trigger) && isGrabbing) {
            //end grab
            isGrabbing = false;
        }
        else if(isGrabbing)
        {
            Vector3 currentPos = VivePose.GetPoseEx(HandRole.RightHand).pos;
            additionalRotation.z = -((initialPos.x - currentPos.x) * 100) % 360;
            additionalRotation.y = -((initialPos.y - currentPos.y) * 100) % 360;
            additionalRotation.x = -((initialPos.z - currentPos.z) * 100) % 360;
            this.transform.rotation = Quaternion.Euler(additionalRotation) * Quaternion.Euler(initialRotation);
        }
    }
}
