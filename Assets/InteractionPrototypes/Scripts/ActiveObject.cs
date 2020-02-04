using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using HTC.UnityPlugin.Vive;

public class ActiveObject : MonoBehaviour, IPointerClickHandler
{
    private bool isActive;

    // Start is called before the first frame update
    void Start()
    {
        isActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.IsViveButton(ControllerButton.Trigger))
        {
            if (isActive)
            {
                transform.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
            } else
            {
                transform.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            }

            isActive = !isActive;
        }
    }
}
