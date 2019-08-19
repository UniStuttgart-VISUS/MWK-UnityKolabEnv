﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotInteraction : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            print("space key was pressed");
            ScreenCapture.CaptureScreenshot(Application.persistentDataPath+"/screenshot_"+DateTime.Now.ToString("yyyyMMddHHmmssfff")+".png",2);
        }
    }
}
