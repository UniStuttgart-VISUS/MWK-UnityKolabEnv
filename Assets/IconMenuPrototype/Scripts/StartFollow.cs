using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Xml.Schema;
using HTC.UnityPlugin.Vive;
using UnityEditor;

public class StartFollow : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public FollowUser follow_User;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointersClick(PointerEventData eventData)
    {
        //GameObject quad = transform.Find("Quad").gameObject;
        transform.GetComponent<MeshRenderer>().material.color = new Color(50, 50, 50);
        if (eventData.IsViveButton(ControllerButton.Trigger))
        {


            //quad.GetComponent<MeshRenderer>().material.color = new Color(50, 50, 50);

            // let the quad follow the user
            follow_User.InitFollow();

            Debug.Log("follow Start following");
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("follow Swatch ClickLeft");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.IsViveButton(ControllerButton.Trigger))
        {
            Debug.Log("Stoped following icon");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<MeshRenderer>().material.color = Color.black;
        Debug.Log("follow Swatch ClickLeft");
    }
}