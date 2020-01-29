using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using interop;

public class Vec4Receiver : MonoBehaviour
{
    private bool has = false;

    public void Update()
    {
        if (!has)
        {
            Object.FindObjectOfType<ZMQReceiver>().addReceiver(
                "Vec4Receiver",
                "Vec4Test",
                my_callback);
            has = true;
        }
    }

    public void my_callback(string input)
    {
        Debug.Log("Vec4Receiver input: " + input);
    }
}