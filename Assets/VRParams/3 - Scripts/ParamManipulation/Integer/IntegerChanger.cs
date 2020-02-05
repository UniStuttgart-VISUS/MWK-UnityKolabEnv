using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HTC.UnityPlugin.Vive;
using interop;

public class IntegerChanger : UnityIntInteraction
{
    public Text integerValueText;
    public Text stepSizeText;
    public Transform leftHand;
    public Transform rightHand;
    public Transform integerValueDisplay;
    public Transform stepSizeDisplay;

    /// <summary>
    /// set a yOffset for the distance between the displayed text and
    /// given controller. An offset of 0 will result in the display
    /// overlapping the controller.
    /// </summary>
    public float yOffset;


    public HandRole left;
    public HandRole right;

    private float selectedFloat;
    private float step;
    private int selectedInt;

    GameObject emptyObject1;
    GameObject emptyObject2;


    // Start is called before the first frame update
    new void Start()
    {
        // create a new gameobjects, so that the scale and rotation of the displays
        // are maintained!
        emptyObject1 = new GameObject();
        emptyObject2 = new GameObject();
        emptyObject1.SetActive(false);
        emptyObject2.SetActive(false);

        integerValueDisplay.SetParent(emptyObject1.transform);
        stepSizeDisplay.SetParent(emptyObject2.transform);
        Vector3 yVecOffset = new Vector3(0, yOffset, 0);
        integerValueDisplay.transform.position = leftHand.position + yVecOffset;
        stepSizeDisplay.transform.position = rightHand.position + yVecOffset;

        emptyObject1.transform.SetParent(leftHand);
        emptyObject2.transform.SetParent(rightHand);

        step = 1;
        selectedFloat = 0;
        UpdateStepText();
        UpdateIntText();

        base.Start();
    }

    override public void StartInteraction(Parameter<int> param, VisParamSender<int> sender)
    {
        emptyObject1.SetActive(true);
        emptyObject2.SetActive(true);

        base.StartInteraction(param, sender);
    }

    public override void StopInteraction()
    {
        emptyObject1.SetActive(false);
        emptyObject2.SetActive(false);

        base.StopInteraction();
    }

    // Update is called once per frame
    void Update()
    {
        ChangeStep();
        ChangeIntValue();
        integerValueDisplay.rotation = Quaternion.Euler(0, leftHand.eulerAngles.y, 0);
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
            float[] stepSizes = new float[] { 0.00001f, 0.0001f, 0.001f, 0.01f, 0.1f, 1, 10 };

            float section = angle / 360;
            int index = (int)System.Math.Round(stepSizes.Length * section, 0);
            step = stepSizes[index];

            UpdateStepText();
        }
    }

    /// <summary>
    /// Changes the current selected integer value
    /// </summary>
    public void ChangeIntValue()
    {
        if (ViveInput.GetPress(left, ControllerButton.Trigger))
        {
            float rotation = GetLHRotation();
            float angle = (rotation + 1) * 180;


            if (angle <= 150)
            {
                selectedFloat += step;

            }
            else if (angle >= 210)
            {
                selectedFloat -= step;
            }
            selectedInt = (int)selectedFloat;
            if (selectedInt != selectedValue.param)
            {
                selectedValue.param = selectedInt;
                senderManager.Send(selectedValue);
            }
            UpdateIntText();
        }
    }

    /// <summary>
    /// Updates the step size text, which is shown above the controller
    /// </summary>
    public void UpdateStepText()
    {
        stepSizeText.text = step.ToString();
    }

    /// <summary>
    /// Updates the int text, which is shown above the controller
    /// </summary>
    public void UpdateIntText()
    {
        integerValueText.text = selectedInt.ToString();
    }
}
