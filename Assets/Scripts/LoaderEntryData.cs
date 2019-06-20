using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoaderEntryData : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Text rendererText = null;
    public Text pathText = null;
    public Text filenameText = null;
    public RawImage itemImage = null;
    public string filepath = null;
    public FileInfo fileinfo = null;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localPosition = new Vector3(transform.localPosition.x,transform.localPosition.y,transform.localPosition.z-4.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localPosition = new Vector3(transform.localPosition.x,transform.localPosition.y,transform.localPosition.z+4.2f);
    }
}
