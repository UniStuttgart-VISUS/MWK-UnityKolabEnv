using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using interop;

public class VisParamReceiver<T>: MonoBehaviour, IJsonStringReceivable
{

    public string receiverName = "VisParamReceiver";
    public VisParamMenu menu;
    private List<string> m_inputJsonStringList;
    private string m_inputJsonString;

    public VisParamReceiver(string name, VisParamMenu menu)
    {
        receiverName = name;
        this.menu = menu;
    }

    public VisParamReceiver(string name)
    {
        receiverName = name;
    }

    public string nameString()
    {
        return receiverName;
    }

    public void setJsonString(string json)
    {
        //m_inputJsonString = json;
        //if (!m_inputJsonStringList.Contains(m_inputJsonString))
        //{
        //    m_inputJsonStringList.Add(m_inputJsonString);
        //}
        menu.AddParameter(ParameterFromString(json));
        
    }

    public List<Parameter<T>> GetReceivedParameters()
    {
        List<Parameter<T>> newList = new List<Parameter<T>>(m_inputJsonStringList.Count);

        foreach (string str in m_inputJsonStringList)
        {
            newList.Add(ParameterFromString(str));
        }
        return newList;
    }

    Parameter<T> ParameterFromString(string json)
    {
        Debug.Log("BoolReceiver: VisBoolFromString");
        Parameter<T> param = new Parameter<T>();
        param.fromJson(json);
        
        //m_inputJsonString = null;

        return param;
    }
}
