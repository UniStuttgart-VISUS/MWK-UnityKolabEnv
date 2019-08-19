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
    private Transform typePanels;
    float currentAlpha = 1.0f;
    string currentMessage = "Test Message";
    bool notificationsEnabled = true;
    private ToolTipLevel currentLevel = ToolTipLevel.HELP;
    
    float messageTimeout = 3.0f;
    DateTime lastShow;

    // Start is called before the first frame update
    void Start()
    {
        cGroup = transform.Find("TTCanvas").GetComponent<CanvasGroup>();
        uiText = transform.Find("TTCanvas").Find("messagePanel").Find("Text").GetComponent<Text>();
        typePanels = transform.Find("TTCanvas").Find("typePanels");

        //Register self as tooltip handler
        EnvConstants.instance.toolTipHandler = transform.gameObject;

        //Initial unshow
        ClearMessage(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (ViveInput.GetPressDownEx(HandRole.LeftHand, ControllerButton.Grip))
        {
            //Switch help off
            if (currentLevel == ToolTipLevel.HELP) currentLevel = ToolTipLevel.INFO;
            else currentLevel = ToolTipLevel.HELP;
        }
        ClearMessageFromTimeout();
        float fadeMultiplier;
        if (cGroup.alpha > currentAlpha) fadeMultiplier = 2.0f;
        else fadeMultiplier = 8.0f;
        cGroup.alpha = Mathf.Lerp(cGroup.alpha, currentAlpha, Time.deltaTime * fadeMultiplier);        
    }

    public void ShowMessage(string message, ToolTipLevel level)
    {
        if (notificationsEnabled && level >= currentLevel)
        {
            Debug.Log("Show tooltip message: " + message);
            currentMessage = message;
            currentLevel = level;
            uiText.text = currentMessage;
            SetTypeIcon(level);
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

    private void SetTypeIcon(ToolTipLevel level)
    {
        foreach (Transform typePanel in typePanels)
        {
            GameObject go = typePanel.gameObject;
            if (go.name.EndsWith(level.ToString()))
            {
                go.SetActive(true);
            }
            else
            {
                go.SetActive(false);
            }
        }
    }
}

public enum ToolTipLevel
{
    HELP = 1,
    INFO = 2,
    WARN = 4,
    ERROR = 8
}
