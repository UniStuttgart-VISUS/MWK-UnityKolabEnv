using interop;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisParamSender : MonoBehaviour, IJsonStringSendable
{
    public string name = "ParameterSender";

    private Parameter<int> param;
    private bool changed = false;

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
        return name;
    }

    public void setParameter(int value)
    {
        param.value = value;
        changed = true;
    }

    public VisParamSender(Parameter<int> param)
    {
        this.param = param;
    }

    

}
