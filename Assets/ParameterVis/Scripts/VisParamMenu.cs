using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using interop;

public class VisParamMenu : MonoBehaviour
{
    public List<VarParameter> parameterList;
    public VisParamSenderManager senderManager;
    public VisVarSender varSender;
    
    //public void AddParameter(object p)
    //{
    //    parameterList.Add(p);
    //}

    public void AddParameter(VarParameter p)
    {
        Debug.Log("[VisVarInteraction]: add parameter");
        if (!parameterList.Contains(p))
        {
            Debug.Log("[VisVarInteraction]: new parameter");
            parameterList.Add(p);
        } else
        {
            Debug.Log("[VisVarInteraction]: parameter already in list");
            int index = parameterList.IndexOf(p);
            parameterList[index] = p;
        }
    }
}
