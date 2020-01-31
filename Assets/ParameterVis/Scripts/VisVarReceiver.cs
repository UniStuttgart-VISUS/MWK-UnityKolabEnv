using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using interop;

public class VisVarReceiver : MonoBehaviour, IJsonStringReceivable
{
    public string receiverName = "VisParamReceiver";
    private VisParamMenu menu;
    private List<string> m_inputJsonStringList;
    private string m_inputJsonString;

    public VisVarReceiver(string name, VisParamMenu menu)
    {
        receiverName = name;
        this.menu = menu;
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

    public List<VarParameter> GetReceivedParameters()
    {
        List<VarParameter> newList = new List<VarParameter>(m_inputJsonStringList.Count);

        foreach (string str in m_inputJsonStringList)
        {
            newList.Add(ParameterFromString(str));
        }
        return newList;
    }

    VarParameter ParameterFromString(string json)
    {
        Debug.Log("[VisVarInteraction]: VisBoolFromString");
        VarParameter param = new VarParameter();
        param.fromJson(json);
        Debug.Log("[VisVarInteraction]: FromString " + param.value);

        //m_inputJsonString = null;

        return param;
    }
}
