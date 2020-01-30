using HTC.UnityPlugin.Vive;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoolInteraction : AbstractInteraction<bool>
{
        public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.IsViveButton(ControllerButton.Trigger))
        {
            selectedValue.value = !selectedValue.value;
            senderManager.send(selectedValue);

            Debug.Log("[BoolInteraction] value = " + selectedValue);
            if (selectedValue.value)
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
