using HTC.UnityPlugin.Vive;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnumInteraction : UnityEnumInteraction, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.IsViveButton(ControllerButton.Trigger))
        {
            selectedValue.param[0] = "test";
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            senderManager.Send(selectedValue);

        }
    }
}
