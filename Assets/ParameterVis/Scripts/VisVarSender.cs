using interop;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisVarSender : MonoBehaviour, IJsonStringSendable
{
    public string senderName = "ParameterSender";

    private VarParameter param;
    private bool changed = false;

    public VisVarSender(string name, bool initBool)
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
        
        if (param.value is bool)
        {
            Parameter<bool> parameter = new Parameter<bool>();
            parameter.name = param.name;
            //parameter.modulFullName = param.modulFullName;
            parameter.param = (bool)param.value;
            return parameter.json();
        }
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

    public void Send(VarParameter param)
    {
        this.param = param;
        changed = true;
    }


}
