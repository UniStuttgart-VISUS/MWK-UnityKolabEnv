using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HTC.UnityPlugin.Vive;
using interop;

public class EnumChanger : UnityEnumInteraction
{
    public Text selectedText;
    public Text nextText;
    public Text previousText;
    public Transform rightHand;

    /// <summary>
    /// set a yOffset for the distance between the displayed text and
    /// given controller. An offset of 0 will result in the display
    /// overlapping the controller.
    /// </summary>
    public float yOffset;

    public float step = 0.03f;
    private float selection = 0;

    public HandRole right;

    private List<string> enumList;
    private GameObject emptyObject1;


    // Start is called before the first frame update
    new void Start()
    {
        // create a new gameobjects, so that the scale and rotation of the displays
        // are maintained!
        emptyObject1 = new GameObject();

        emptyObject1.SetActive(false);

        transform.SetParent(emptyObject1.transform);
        Vector3 yVecOffset = new Vector3(0, yOffset, 0);
        transform.position = rightHand.position + yVecOffset;
        emptyObject1.transform.SetParent(rightHand);



        base.Start();
    }

    override public void StartInteraction(Parameter<List<string>> param, VisParamSender<List<string>> sender)
    {
        base.StartInteraction(param, sender);
        enumList = selectedValue.param;
        // remove selected value
        Debug.Log("[EnumChanger] slected value: " + selectedValue.param[0] + ", Count: " + selectedValue.param.Count);
        enumList.RemoveAt(0);

        emptyObject1.SetActive(true);
        
    }

    public override void StopInteraction()
    {
        emptyObject1.SetActive(false);

        base.StopInteraction();
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, rightHand.eulerAngles.y, 0);
        ChangeEnum();
    }


    public float GetRHRotation()
    {
        return VivePose.GetPose(right).rot.z;
    }

    public void ChangeEnum()
    {
        if (ViveInput.GetPress(right, ControllerButton.Trigger))
        {
            float rotation = -1*GetRHRotation();
            float angle = (rotation + 1) * 180;

            if (angle > 210 && selection < enumList.Count - 1)
            {
                selection += step;
            } else if (angle < 150 && selection > 0)
            {
                selection -= step;
            }

            

            UpdateText((int)selection, enumList);

            if (senderManager != null)
            {
                selectedValue.param[0] = enumList[(int)selection];
                senderManager.Send(selectedValue);
            }

        }
    }

    public void UpdateText(int selection, List<string> enumList)
    {
        selectedText.text = enumList[selection];
        if (selection == 0)
        {
            nextText.text = enumList[selection+1];
            previousText.text = "";
        } else if (selection == enumList.Count-1)
        {
            previousText.text = enumList[selection - 1];
            nextText.text = "";
        } else
        {
            nextText.text = enumList[selection + 1];
            previousText.text = enumList[selection - 1];
        }
    }


}
