using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using interop;

public class VisParamReceiver : MonoBehaviour, IJsonStringReceivable
{

    public string name = "VisParamReceiver";
    private List<string> m_inputJsonStringList;
    private string m_inputJsonString;

    public string nameString()
    {
        return name;
    }

    public void setJsonString(string json)
    {
        m_inputJsonStringList.Add(json);
    }

    Parameter<int> ParameterFromString()
    {
        Debug.Log("BoolReceiver: VisBoolFromString");
        Parameter<int> bb = new Parameter<int>();
        bb.fromJson(m_inputJsonString);

        //bb.b = convert.toUnity(bb.b);
        //bb.name = convert.toUnity(bb.max);
        //bb.length = convert.toUnity(bb.max);

        //m_currentlyUsedJsonString = m_inputJsonString;
        m_inputJsonString = null;

        return bb;
    }
}
