using HTC.UnityPlugin.Vive;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoolInteraction : MonoBehaviour, IPointerClickHandler
{

    public bool value = true;

    public bool changed = false;

    public BoolSender sender;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.IsViveButton(ControllerButton.Trigger))
        {
            value = !value;
            changed = true;
            sender.value = value;

            Debug.Log("[BoolInteraction] value = " + value);
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
