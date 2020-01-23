using HTC.UnityPlugin.Vive;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Parameter : MonoBehaviour, IPointerClickHandler
{
    public OwnTaskbarMenu Menu;
    public OwnTaskbarMenuItem item;
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
        Menu.add(item);
    }
}
