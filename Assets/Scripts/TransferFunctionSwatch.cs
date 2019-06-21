using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using HTC.UnityPlugin.Vive;
using Photon.Voice.Unity;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;

public class TransferFunctionSwatch : MonoBehaviour 
    , IPointerEnterHandler
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
    private bool wasMoved = false;
    private bool removeStarted = false;
    
    // Start is called before the first frame update
    void Start()
    {
        mat = transform.Find("eis").Find("Sphere001").gameObject.GetComponent<MeshRenderer>().material;
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

    void OnMouseUp()
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

        if (transform.localPosition.z < -0.22f && removeStarted) {
            //Remove final
            Destroy(gameObject);
        }
        else
        {
            Vector3 tp = transform.localPosition;
            tp = new Vector3(tp.x, tp.y, 0.04f);
            transform.localPosition = tp;
            transform.Find("eis").Find("Cone001").GetComponent<Renderer>().material.color = new Color(0.3f, 0.3f, 0.3f, 1.0f);
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.IsViveButton(ControllerButton.Trigger))
        {
            if (!wasMoved)
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
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("Swatch ClickLeft");
        }
    }
    
    public void setColor(Color input)
    {
        if (mat == null)
        {
            mat = transform.Find("eis").Find("Sphere001").gameObject.GetComponent<MeshRenderer>().material;
        }
        mat.color = input;
        selectedColor = input;
    }
    
    void startColorChoosing()
    {
        cpObject = Instantiate(cpPrefab, transform.position + Vector3.up * 1.4f, Quaternion.identity);
        cpObject.transform.localScale = Vector3.one * 0.4f;
        cpObject.transform.LookAt(Camera.main.transform);
        cpTriangle = cpObject.GetComponent<ColorPickerTriangle>();
        cpTriangle.SetNewColor(mat.color);
        cpObject.transform.parent = transform;
        Debug.Log(cpObject);
        choosing = true;
    }

    void stopColorChoosing()
    {
        Destroy(cpObject);
        choosing = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            Vector3 dragDist = dragStartPoint -
                               transform.parent.InverseTransformPoint(eventData.pointerCurrentRaycast.worldPosition);
            if (Mathf.Abs(dragDist.y) > 0.02f && !removeStarted)
            {
                wasMoved = true;
                Vector3 tp = transform.localPosition;
                tp = new Vector3(tp.x,Mathf.Clamp(dragStartPoint.y-dragDist.y, -3.0f, 0.0f),tp.z);
                transform.localPosition = tp;
            } else if (dragDist.z > 0.04f || removeStarted) {
                wasMoved = true;
                removeStarted = true;
                Vector3 tp = transform.localPosition;
                tp = new Vector3(tp.x,tp.y,Mathf.Clamp(dragStartPoint.z-dragDist.z/5, -0.4f, 0.04f));
                transform.localPosition = tp;
                transform.Find("eis").Find("Cone001").GetComponent<Renderer>().material.color = new Color(0.3f,0.3f,0.3f,1.0f - dragDist.z*2);
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
        if (transform.localPosition.z < -0.22f && removeStarted)
        {
            //Remove final
            Destroy(gameObject);
        }
        else
        {
            Vector3 tp = transform.localPosition;
            tp = new Vector3(tp.x, tp.y, 0.04f);
            transform.localPosition = tp;
            transform.Find("eis").Find("Cone001").GetComponent<Renderer>().material.color = new Color(0.3f, 0.3f, 0.3f, 1.0f);
        }
    }
}
