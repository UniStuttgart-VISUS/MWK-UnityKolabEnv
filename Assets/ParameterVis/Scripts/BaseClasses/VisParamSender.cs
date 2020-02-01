using interop;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisParamSender<T> : MonoBehaviour, IJsonStringSendable
{
    public string senderName = "ParameterSender";

    private Parameter<T> param;
    private bool changed = false;

    public VisParamSender(string name, bool initBool)
    {
        this.senderName = name;
        this.changed = initBool;
    }

    public bool hasChanged()
    {
       return changed;
    }

    public string jsonString()
    {
        changed = false;
        return param.json();
    }

    public string nameString()
    {
        return senderName;
    }

    //public void setParameter(T value)
    //{
    //    param.value = value;
    //    changed = true;
    //}

    public void Send(Parameter<T> param)
    {
        this.param = param;
        changed = true;
    }
}
