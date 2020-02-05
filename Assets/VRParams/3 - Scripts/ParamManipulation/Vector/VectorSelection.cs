using HTC.UnityPlugin.Vive;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VectorSelection : UnityVector3Interaction
{
    public HandRole rightHand;
    public HandRole leftHand;

    public Transform arrow;
    public Transform cameraRig;
    private Vector3 currentDirection;
    private Quaternion currentRotation;



    // Update is called once per frame
    void Update()
    {
        Debug.Log(GetHandVector());
    }

    public Vector3 GetHandVector()
    {
        Vector3 leftPos = VivePose.GetPose(leftHand, cameraRig).pos;
        Vector3 rightPos = VivePose.GetPose(rightHand, cameraRig).pos;
        Vector3[] pos = new Vector3[] { leftPos, rightPos };
        Vector3 direction = VivePose.GetPose(rightHand).pos - VivePose.GetPose(leftHand).pos;
        Vector3 middle = leftPos + 0.5f * direction;
        float length = direction.magnitude;
        float scale = length * 0.5f;
        arrow.localScale = new Vector3(scale, scale, scale);

        if (ViveInput.GetPress(rightHand, ControllerButton.Trigger) && ViveInput.GetPress(leftHand, ControllerButton.Trigger))
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            rotation *= Quaternion.Euler(Vector3.up * -90);
            arrow.SetPositionAndRotation(middle, rotation);

            currentDirection = direction;
            currentRotation = rotation;

            return direction;
        }
        else
        {
            arrow.SetPositionAndRotation(middle, currentRotation);

            // send value to megamol
            if (currentDirection != selectedValue.param)
            {
                selectedValue.param = currentDirection;
                senderManager.Send(selectedValue);
            }

            return currentDirection;
        }

    }
}
