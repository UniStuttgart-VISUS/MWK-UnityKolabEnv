using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using HTC.UnityPlugin.Vive;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;

public class TransferFunctionMask : MonoBehaviour, IPointerEnterHandler
    , IPointerExitHandler
    , IPointerClickHandler
    , IDragHandler
    , IPointerDownHandler 
    , IPointerUpHandler {

    private Vector3 dragStartPoint;
    private bool isDragging = false;
    private bool wasMoved = false;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseUp()
    {
        
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        isDragging = false;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            Vector3 dragDist = dragStartPoint -
                               transform.parent.InverseTransformPoint(eventData.pointerCurrentRaycast.worldPosition);
            Debug.Log(dragDist.magnitude);
            if (Mathf.Abs(dragDist.y) > 0.02f)
            {
                wasMoved = true;
                Vector3 tp = transform.localPosition;
                tp = new Vector3(tp.x,Mathf.Clamp(dragStartPoint.y-dragDist.y, -3.0f, 0.0f),tp.z);
                transform.localPosition = tp;
                //Debug.Log("POS CHANGE "+tp+ " in was "+dragDist);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        wasMoved = false;
        isDragging = true;
        dragStartPoint = transform.parent.InverseTransformPoint(eventData.pointerCurrentRaycast.worldPosition);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }
}
