using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NetMQ;
using NetMQ.Sockets;

using interop;

public class BoolSender : MonoBehaviour, IJsonStringSendable
{
    public string Name = "SendTest";
    public bool value = false;
    public BoolInteraction cube;

    public bool hasChanged()
    {
        //if (changed)
        //{
        //    changed = false;
        //    return true;
        //}
        return true;
    }

    public string nameString()
    {
        return this.Name;
    }

    public string jsonString()
    {
        VisBool b = new VisBool();
        b.b = value;
        b.name = "test";

        Parameter<int> param = new Parameter<int>();
        param.value = 45;
        param.name = "test";
        param.name = "projekt::SphereRenderer";

        Debug.Log("[BoolInteraction] value " + value + " sendable");
        return b.json();
    }

}
