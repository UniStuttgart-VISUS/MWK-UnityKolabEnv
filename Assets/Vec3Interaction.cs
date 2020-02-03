using HTC.UnityPlugin.Vive;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Vec3Interaction : UnityVector3Interaction, IPointerClickHandler
{
    // Start is called before the first frame update
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.IsViveButton(ControllerButton.Trigger))
        {
            selectedValue.param = new Vector3(1.0f, 1.0f, 1.0f);
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            senderManager.Send(selectedValue);

        }
    }
}
