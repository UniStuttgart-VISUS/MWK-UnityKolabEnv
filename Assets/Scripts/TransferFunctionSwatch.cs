using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using HTC.UnityPlugin.Vive;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;

public class TransferFunctionSwatch : MonoBehaviour, IPointerEnterHandler
    , IPointerExitHandler
    , IPointerClickHandler
    , IDragHandler
    , IPointerDownHandler 
    , IPointerUpHandler {

    public GameObject cpPrefab;
    private ColorPickerTriangle cpTriangle;
    private Material mat;
    private GameObject cpObject;
    private bool choosing = false;
    public Color selectedColor = Color.red;
    private HashSet<PointerEventData> hovers = new HashSet<PointerEventData>();
    private Vector3 dragStartPoint;
    private bool isDragging = false;
    
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
        Debug.Log("COLOR:Start");
    }

    // Update is called once per frame
    void Update()
    {
        if (choosing)
        {
            setColor(cpTriangle.TheColor);
        }
    }

    void OnMouseDown()
    {
        if (!choosing)
        {
            startColorChoosing();
        }
        else
        {
            stopColorChoosing();
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hovers.Add(eventData) && hovers.Count == 1)
        {
            //Debug.Log("Swatch Hover");
            //transform.localScale = new Vector3(0.3f,0.3f,0.3f);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (hovers.Remove(eventData) && hovers.Count == 0)
        {
            //Debug.Log("Swatch UnHover");// turn to normal state
            //transform.localScale = new Vector3(0.1f,0.1f,0.1f);
        }
        isDragging = false;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log(eventData.selectedObject);
        if (eventData.IsViveButton(ControllerButton.Trigger) || true)
        {
            //Debug.Log("Swatch Click");
            if (!choosing)
            {
                startColorChoosing();
            }
            else
            {
                stopColorChoosing();
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            //Debug.Log("Swatch ClickLeft");
        }
    }
    
    public void setColor(Color input)
    {
        if (mat == null)
        {
            //Debug.Log("COLOR:Mat was null");
            mat = GetComponent<MeshRenderer>().material;
        }
        mat.color = input;
        selectedColor = input;
    }
    
    void startColorChoosing()
    {
        cpObject = (GameObject)Instantiate(cpPrefab, transform.position + Vector3.up * 1.4f, Quaternion.identity);
        //cpObject.transform.localScale = Vector3.one * 1.1f;
        //cpObject.transform.LookAt(transform.parent);
        cpTriangle = cpObject.GetComponent<ColorPickerTriangle>();
        cpTriangle.SetNewColor(mat.color);
        cpObject.transform.parent = transform;
        choosing = true;
    }

    void stopColorChoosing()
    {
        Destroy(cpObject);
        choosing = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging )
        {
            Vector3 dragDist = dragStartPoint -
                               transform.parent.InverseTransformPoint(eventData.pointerCurrentRaycast.worldPosition);
            Debug.Log(dragDist.magnitude);
            if (dragDist.magnitude > 0.02f)
            {
                Vector3 tp = transform.localPosition;
                tp = new Vector3(tp.x,Mathf.Clamp(tp.y-dragDist.y/10, -3.0f, 3.0f),tp.z);
                transform.localPosition = tp;
                //Debug.Log("POS CHANGE "+tp+ " in was "+dragDist);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        dragStartPoint = transform.parent.InverseTransformPoint(eventData.pointerCurrentRaycast.worldPosition);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }
}
