using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HTC.UnityPlugin.Vive;

public class FloatChanger : MonoBehaviour
{
    public Text floatValueText;
    public Text stepSizeText;
    public Transform leftHand;
    public Transform rightHand;
    public Transform floatValueDisplay;
    public Transform stepSizeDisplay;
    public float yOffset;
    public HandRole left;
    public HandRole right;

    private float selectedFloat;
    private float step;

    // Start is called before the first frame update
    void Start()
    {
        var emptyObject1 = new GameObject();
        var emptyObject2 = new GameObject();
        floatValueDisplay.SetParent(emptyObject1.transform);
        stepSizeDisplay.SetParent(emptyObject2.transform);
        Vector3 yVecOffset = new Vector3(0, yOffset, 0);
        floatValueDisplay.transform.position = leftHand.position + yVecOffset;
        stepSizeDisplay.transform.position = rightHand.position + yVecOffset;

        emptyObject1.transform.SetParent(leftHand);
        emptyObject2.transform.SetParent(rightHand);

        step = 1;
        selectedFloat = 0;
        UpdateStepText();
        UpdateFloatText();


    }

    // Update is called once per frame
    void Update()
    {
        ChangeStep();
        ChangeFloatValue();
        floatValueDisplay.rotation = Quaternion.Euler(0, leftHand.eulerAngles.y, 0);
        stepSizeDisplay.rotation = Quaternion.Euler(0, rightHand.eulerAngles.y, 0);
    }

    public float GetLHRotation()
    {
        return VivePose.GetPose(left).rot.z;
    }

    public float GetRHRotation()
    {
        return VivePose.GetPose(right).rot.z;
    }

    public void ChangeStep()
    {
        if (ViveInput.GetPress(right, ControllerButton.Trigger))
        {
            float rotation = GetRHRotation();
            float angle = (rotation + 1) * 180;
            float[] stepSizes = new float[]{0.00001f, 0.0001f, 0.001f, 0.01f, 0.1f, 1, 10};

            float section = angle / 360;
            int index = (int)System.Math.Round(stepSizes.Length * section, 0);
            step = stepSizes[index];

            UpdateStepText();
        }
    }

    public void ChangeFloatValue()
    {
        if (ViveInput.GetPress(left, ControllerButton.Trigger))
        {
            float rotation = GetLHRotation();
            float angle = (rotation + 1) * 180;


            if (angle <= 150)
            {
                selectedFloat += step;
                
            } else if (angle >= 210) {
                selectedFloat -= step;
            }
            Debug.Log(angle + " " + selectedFloat);
            UpdateFloatText();
        }
    }

    public void UpdateStepText()
    {
        stepSizeText.text = step.ToString();
    }

    public void UpdateFloatText()
    {
        floatValueText.text = selectedFloat.ToString();
    }

    public void SetFloatValueText(float value)
    {
        floatValueText.text = value.ToString();
    }

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }
}
