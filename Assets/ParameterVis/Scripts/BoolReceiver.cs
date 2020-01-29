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


    public class BoolReceiver : MonoBehaviour, IJsonStringReceivable, IJsonStringSendable
{

    public string Name = "ReceiveTest";
    public GameObject Cube; 
    private string m_inputJsonString = "";
    private string m_currentlyUsedJsonString = null;
    private bool changed = false;
    bool b;

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

    public bool hasChanged()
    {
        //if (changed)
        //{
        //    changed = false;
        //    return true;
        //}
        return true;
    }

    public string jsonString()
    {
        //VisBool old = VisBoolFromString();
        //old.b = b;

        Parameter<int> param = new Parameter<int>();
        param.value = 45;
        param.name = "test";
        param.name = "projekt::SphereRenderer";

        return param.json();
    }

    VisBool VisBoolFromString()
    {
        Debug.Log("BoolReceiver: VisBoolFromString" + m_inputJsonString);


        VisBool bb = new VisBool();
        Debug.Log("BoolReceiver: VisBool" + bb);
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
            b = VisBoolFromString().b;
            Debug.Log("Json String for VisBool: " + b);
            changed = true;
            if (b)
            {
                Cube.gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
            }
            else
            {
                Cube.gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            }
        }
    }

    public void setBool()
    {

    }
}
