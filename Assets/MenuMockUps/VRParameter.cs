using System.Collections;
using HTC.UnityPlugin.Vive;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class VRParameter : MonoBehaviour, IPointerClickHandler
{
    public OverlayMenu overlayMenu;
    private CheckboxScript checkbox;
    private PopUpMenuIcon Icon;
    bool isActive;
    // Start is called before the first frame update
    void Start()
    {
        checkbox = overlayMenu.GetInteraction();
        Icon = overlayMenu.GetPopUpMenuIcon();
        isActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        overlayMenu.Disappear();
        Icon.Appear();
        checkbox.Appear();
    }
}
