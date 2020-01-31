using HTC.UnityPlugin.Vive;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoolInteraction : AbstVarInteraction<bool>, IPointerClickHandler
{
        public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.IsViveButton(ControllerButton.Trigger))
        {
            value = !value;
            selectedValue.value = value;
            varSender.Send(selectedValue);

            Debug.Log("[BoolInteraction] value = " + selectedValue);
            if (value)
            {
                gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
                
            }
            else
            {
                gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            }
        }
    }
}
