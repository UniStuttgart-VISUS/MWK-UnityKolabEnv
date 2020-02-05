using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using HTC.UnityPlugin.Vive;

public class CheckboxScript : UnityBoolInteraction, IPointerClickHandler
{
    private bool isChecked;
    private GameObject cross;
    public bool initValueTrue;

    // Start is called before the first frame update
    new void Start()
    {
        cross = this.transform.GetChild(1).gameObject;
        isChecked = initValueTrue;
        base.Start();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.IsViveButton(ControllerButton.Trigger))
        {
            cross.SetActive(isChecked);
            isChecked = !isChecked;
            selectedValue.param = isChecked;
            senderManager.Send(selectedValue);
        }       
    }
}
