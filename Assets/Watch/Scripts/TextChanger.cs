using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HTC.UnityPlugin.Vive;

public class TextChanger : MonoBehaviour
{
    public Text myText;
    float i = 0;
    public WallInput wall;
    string currentSelection;
    public HandRole hand;

    public string[] enumArray = new string[6];
    public ScrollRect rect;
    
        // Start is called before the first frame update
    void Start()
    {
        wall = GameObject.FindObjectOfType(typeof(WallInput)) as WallInput;
        enumArray[0] = "hallo0";
        enumArray[1] = "hallo1";
        enumArray[2] = "hallo2";
        enumArray[3] = "hallo3";
        enumArray[4] = "hallo4";
        enumArray[5] = "hallo5";
    }

    // Update is called once per frame
    void Update()
    {
       ChangeText();
    }

    public void ChangeText()
    {
         ChangeEnum();
        //ChangeInt();
    }

    private void ChangeInt()
    {
        float rotation = GetRotation();
        float angle = (rotation + 1) * 180;
        Debug.Log(angle);
        int direction = 0;

        if ((angle <= 150.0f && angle > 60.0f) && (i < 99.0f))
        {
            i -= 0.3f * rotation;
            direction = 1;
        }
        else if ((angle >= 210.0f && angle < 300.0f) && (i > -99.0f))
        {
            i += -0.3f * rotation;
            direction = -1;

        }
        else if ((angle >= 300.0f) && (i > -99.0f))
        {
            i -= 1;
            direction = -2;
        }
        else if ((angle <= 60.0f) && (i < 99.0f))
        {
            i += 1;
            direction = 2; ;
        }
        string intString = System.Convert.ToString((int)i);

        if (direction == 1)
        {
            myText.text = intString + " >";
        } else if (direction == 2)
        {
            myText.text = intString + " >>";
        }
        else if (direction == -1)
        {
            myText.text = "< " + intString;
        }
        else if (direction == -2)
        {
            myText.text = "<< " + intString;
        }
        else
        {
            myText.text = intString;
        }
        
        wall.SetValue((int)i);
    }

    private void ChangeEnum()
    {
        float rotation = GetRotation();
        float angle = (rotation + 1) * 180;
        Debug.Log(angle);

        if ((angle <= 150.0f && angle > 60.0f) && (i < 5))
        {
            i -= 0.3f * rotation;
        }
        else if ((angle >= 210.0f && angle < 300.0f) && (i > 0))
        {
            i += -0.3f * rotation;
        }
        else if ((angle >= 300.0f) && (i > 0))
        {
            i -= 1;
        }
        else if ((angle <= 60.0f) && (i < 5))
        {
            i += 1;
        }
        int index = (int)i;
        string intString = enumArray[index];
        myText.text = intString;
        wall.SetValue((int)i);
    }

    public float GetRotation()
    {

        Quaternion newRotation = Quaternion.identity; // Create new quaternion with no rotation
        newRotation.z = transform.rotation.z; // Get only the X rotation from the controllers quaternion

        // Output the x rotation. It should be a value between -1 and 1
        // Debug.Log("newRotation.z: " + newRotation.z);
        // Convert to a value between 0 and 360
        //float xEuler = (newRotation.z + 1) * 180;
        float xEuler = newRotation.z;
        // Output the x rotation as a Euler angle
        //Debug.Log("xEuler: " + xEuler);

        return xEuler;

    }

    public void PrintTouch() {
        Debug.Log(ViveInput.GetPadAxis(hand).x);
    }
}
