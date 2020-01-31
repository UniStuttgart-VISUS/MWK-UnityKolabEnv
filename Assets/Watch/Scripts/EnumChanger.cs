using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HTC.UnityPlugin.Vive;

public class EnumChanger : MonoBehaviour
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

    private string[] enumList = new string[] { "eins", "zwei", "drei", "vier", "fünf", "sechs", "sieben" };

    // Start is called before the first frame update
    void Start()
    {
        // create a new gameobjects, so that the scale and rotation of the displays
        // are maintained!
        var emptyObject1 = new GameObject();
        transform.SetParent(emptyObject1.transform);
        Vector3 yVecOffset = new Vector3(0, yOffset, 0);
        transform.position = rightHand.position + yVecOffset;
        emptyObject1.transform.SetParent(rightHand);
        

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

            if (angle > 210 && selection < enumList.Length - 1)
            {
                selection += step;
            } else if (angle < 150 && selection > 0)
            {
                selection -= step;
            }


            UpdateText((int)selection);

        }
    }

    public void UpdateText(int selection)
    {
        selectedText.text = enumList[selection];
        if (selection == 0)
        {
            nextText.text = enumList[selection+1];
            previousText.text = "";
        } else if (selection == enumList.Length-1)
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
