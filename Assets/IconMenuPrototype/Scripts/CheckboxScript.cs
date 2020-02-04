using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using HTC.UnityPlugin.Vive;

public class CheckboxScript : MonoBehaviour, IPointerClickHandler
{
    private bool isChecked;
    private GameObject cross;
    public bool initValueTrue;

    // Start is called before the first frame update
    void Start()
    {
        cross = this.transform.GetChild(1).gameObject;
        isChecked = initValueTrue;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.IsViveButton(ControllerButton.Trigger))
        {
            cross.SetActive(isChecked);
            isChecked = !isChecked;
        }       
    }
}
