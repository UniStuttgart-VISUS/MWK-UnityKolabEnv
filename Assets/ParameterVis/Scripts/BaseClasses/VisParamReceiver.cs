using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using interop;

public class VisParamReceiver<T>: MonoBehaviour, IJsonStringReceivable
{

    public string receiverName = "VisParamReceiver";
    public VisParamMenu menu;
    //private List<string> m_inputJsonStringList;
    private List<string> parameterList;
    private string m_inputJsonString;

    public VisParamReceiver(string name, VisParamMenu menu)
    {
        parameterList = new List<string>();
        receiverName = name;
        this.menu = menu;
    }

    public VisParamReceiver(string name)
    {
        parameterList = new List<string>();
        receiverName = name;
    }

    public string nameString()
    {
        return receiverName;
    }

    public void setJsonString(string json)
    {
        Parameter<T> p = ParameterFromString(json);

        string paramID = p.name + p.modulFullName;

        menu.AddParameter(paramID, p);
        
    }

    //public List<Parameter<T>> GetReceivedParameters()
    //{
    //    List<Parameter<T>> newList = new List<Parameter<T>>(m_inputJsonStringList.Count);

    //    foreach (string str in m_inputJsonStringList)
    //    {
    //        newList.Add(ParameterFromString(str));
    //    }
    //    return newList;
    //}

    Parameter<T> ParameterFromString(string json)
    {
        //Debug.Log("BoolReceiver: VisBoolFromString");
        Parameter<T> param = new Parameter<T>();
        param.fromJson(json);
        
        //m_inputJsonString = null;

        return param;
    }
}
