using HTC.UnityPlugin.Vive;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IntInteraction : UnityIntInteraction, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.IsViveButton(ControllerButton.Trigger))
        {
            selectedValue.param = 10;
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            senderManager.Send(selectedValue);

        }
    }
}
