using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using HTC.UnityPlugin.Vive;
using interop;

public class VisParamCubeMenu : VisParamMenu, IPointerClickHandler
{

    public UnityBoolInteraction boolinteraction;
    public BoolSender boolSender;



    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("[VisInteraction]: Menu clickt");
        if (eventData.IsViveButton(ControllerButton.Trigger))
        {
            foreach (object paramObject in parameterList)
            {

                if ( paramObject is Parameter<bool>) {
                    Debug.Log("[VisInteraction]: bool param started");
                    Parameter<bool> p = (Parameter<bool>)parameterList[0];
                    Debug.Log("[VisInteraction]: bool param: " + p.name + ", " + (p.param ? "true" : "false"));
                    Debug.Log("[VisInteraction]: bool sender: " + boolSender.senderName);
                    boolinteraction.StartInteraction(p, boolSender);
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
