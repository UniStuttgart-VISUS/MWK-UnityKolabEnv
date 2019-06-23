using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverTooltipSender : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string message = "";

    // Start is called before the first frame update
    void Start()
    {
        if (message == "")
        {
            message = "Please set message for hover on " + transform.gameObject.name;
        } 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Pointer events
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Send tooltip message: " + message);
        EnvConstants.instance.showTooltip(message);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Clear tooltip message: " + message);
        EnvConstants.instance.clearTooltip();
    }
}
