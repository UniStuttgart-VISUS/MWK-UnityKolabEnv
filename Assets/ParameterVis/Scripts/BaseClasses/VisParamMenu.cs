﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using interop;



public class VisParamMenu : MonoBehaviour
{
    //public List<VarParameter> parameterList2;
    protected List<object> parameterList = new List<object>();
    private List<object> parameterNameList = new List<object>();
    //protected SortedDictionary<string, object> parameterList = new SortedDictionary<string, object>();
    private VisParamSenderManager senderManager;
    private VisVarSender varSender;

    public void AddParameter(string name, object p)
    {
        //Debug.Log("[VisInteraction]: add parameter");
        if (!parameterNameList.Contains(name))
        {
            Debug.Log("[VisInteraction]: new parameter " + name);
            parameterList.Add(p);
            parameterNameList.Add(name);
        }else
        {
            int index = parameterNameList.IndexOf(name);
            parameterList[index] = p;
        }
        //else
        //{
        //    //Debug.Log("[VisInteraction]: parameter already in list");
        //    int index = parameterList.IndexOf(p);
        //    parameterList[index] = p;
        //}
    }

    public void UpdateParameter()
    {

    }

    //public void AddParameter(VarParameter p)
    //{
    //    Debug.Log("[VisVarInteraction]: add parameter");
    //    if (!parameterList2.Contains(p))
    //    {
    //        Debug.Log("[VisVarInteraction]: new parameter");
    //        parameterList2.Add(p);
    //    } else
    //    {
    //        Debug.Log("[VisVarInteraction]: parameter already in list");
    //        int index = parameterList2.IndexOf(p);
    //        parameterList2[index] = p;
    //    }
    //}
}
