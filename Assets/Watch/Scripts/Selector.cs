using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Xml.Schema;
using HTC.UnityPlugin.Vive;
using UnityEditor;

public class Selector : MonoBehaviour, IPointerClickHandler
{
    public ConfigPanel panel;
    public GameObject child;
    public GameObject sphere;
    bool isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        panel.Hide();
        child = gameObject.transform.GetChild(0).gameObject;
        sphere = child.transform.GetChild(0).gameObject;
        sphere.GetComponent<MeshRenderer>().material.color = Color.red;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isActive)
        {
            panel.Show();
            sphere.GetComponent<MeshRenderer>().material.color = Color.green;
            isActive = true;
        }  else
        {
            panel.Hide();
            sphere.GetComponent<MeshRenderer>().material.color = Color.red;
            isActive = false;
        }
    }
}
