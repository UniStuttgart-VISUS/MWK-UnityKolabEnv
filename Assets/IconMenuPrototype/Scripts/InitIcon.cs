using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Xml.Schema;
using HTC.UnityPlugin.Vive;
using UnityEditor;

public class InitIcon : MonoBehaviour, IPointerClickHandler
{

    public GameObject icon;
    private bool active = false;

    // Start is called before the first frame update
    void Start()
    {
        icon.SetActive(active);
        Debug.Log("Set Active to ");
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //transform.GetComponent<MeshRenderer>().material.color = new Color(520, 520, 520);
        Debug.Log("Start following");
        if (eventData.IsViveButton(ControllerButton.Trigger))
        {


            //quad.GetComponent<MeshRenderer>().material.color = new Color(50, 50, 50);
            Debug.Log("Click");
            active = !active;
            icon.SetActive(active);
            Debug.Log("active");


            Debug.Log("Start init icon");
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("Swatch ClickLeft");
        }
    }
}