using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NetMQ;
using NetMQ.Sockets;

using interop;

public class Vec4Receiver2 : MonoBehaviour
{
    private bool has = false;

    public void Update()
    {
        if(!has)
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


    public class BoolReceiver : MonoBehaviour, IJsonStringReceivable
{

    public string Name = "ReceiveTest";
    public GameObject Cube; 
    private string m_inputJsonString = null;
    private string m_currentlyUsedJsonString = null;

    public string nameString()
    {
        Debug.Log("BoolReceiver: nameString " + this.Name);
        return this.Name;
    }

    public void setJsonString(string json)
    {
        Debug.Log("BoolReceiver: set json " + json);
        m_inputJsonString = json;
    }

    VisBool VisBoolFromString()
    {
        Debug.Log("BoolReceiver: VisBoolFromString");
        VisBool bb = new VisBool();
        bb.fromJson(m_inputJsonString);

        //bb.b = convert.toUnity(bb.b);
        //bb.name = convert.toUnity(bb.max);
        //bb.length = convert.toUnity(bb.max);

        m_currentlyUsedJsonString = m_inputJsonString;
        m_inputJsonString = null;

        return bb;
    }

    public void Start()
    {
        
    }

    public void Update()
    {
        if (m_inputJsonString != null)
        {
            bool b = VisBoolFromString().b;
            Debug.Log("Json String for VisBool: " + b);
            if (b)
            {
                Cube.gameObject.GetComponent<Material>().color = Color.black;
            }
            else
            {
                Cube.gameObject.GetComponent<Material>().color = Color.red;
            }
        }
    }
}
