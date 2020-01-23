using HTC.UnityPlugin.Vive;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopUpMenuIcon : MonoBehaviour, IPointerClickHandler
{
    public OverlayMenu Menu;
    private bool isActive = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Menu.isActiveAndEnabled)
        {
            gameObject.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Menu.Appear();
    }

    public void Appear()
    {
         gameObject.SetActive(true);
    }
}