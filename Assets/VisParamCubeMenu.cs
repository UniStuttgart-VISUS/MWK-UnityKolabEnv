using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using HTC.UnityPlugin.Vive;
using interop;

public class VisParamCubeMenu : VisParamMenu, IPointerClickHandler
{

    public AbstVarInteraction<bool> boolinteraction;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.IsViveButton(ControllerButton.Trigger))
        {
            foreach (VarParameter paramObject in parameterList)
            {
                if ( paramObject.value is bool) {
                    boolinteraction.StartInteraction(parameterList[0], varSender);
                }
            }
            
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
