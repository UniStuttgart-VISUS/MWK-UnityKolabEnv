using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using HTC.UnityPlugin.Vive;

public class ToolTipManager : MonoBehaviour
{
    CanvasGroup cGroup;
    Text uiText;
    float currentAlpha = 1.0f;
    string currentMessage = "Test Message";
    bool notificationsEnabled = true;
    float messageTimeout = 3.0f;
    DateTime lastShow;

    // Start is called before the first frame update
    void Start()
    {
        cGroup = transform.Find("TTCanvas").GetComponent<CanvasGroup>();
        uiText = transform.Find("TTCanvas").Find("messagePanel").Find("Text").GetComponent<Text>();

        //Register self as tooltip handler
        EnvConstants.instance.toolTipHandler = transform.gameObject;

        //Initial unshow
        //ClearMessage(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (ViveInput.GetPressDownEx(HandRole.LeftHand, ControllerButton.Grip))
        {
            notificationsEnabled = !notificationsEnabled;
            if(!notificationsEnabled)
            {
                currentAlpha = 0.0f;
            }
        }
        ClearMessageFromTimeout();
        cGroup.alpha = Mathf.Lerp(cGroup.alpha, currentAlpha, Time.deltaTime * 2);        
    }

    public void ShowMessage(string message)
    {
        if (notificationsEnabled)
        {
            Debug.Log("Show tooltip message: " + message);
            currentMessage = message;
            uiText.text = currentMessage;
            currentAlpha = 1.0f;
            lastShow = DateTime.Now;
        }
    }

    public void ClearMessage(bool immediately = false)
    {
        currentAlpha = 0.0f;
        if(immediately)
        {
            cGroup.alpha = 0.0f;
        }
    }

    public void ClearMessageFromTimeout()
    {
        if((DateTime.Now - lastShow).TotalSeconds > messageTimeout && currentAlpha == 1.0f)
        {
            Debug.Log("Clear from timeout");
            currentAlpha = 0.0f;
        }
    }
}
