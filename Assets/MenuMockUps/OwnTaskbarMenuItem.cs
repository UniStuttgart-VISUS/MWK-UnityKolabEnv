using HTC.UnityPlugin.Vive;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OwnTaskbarMenuItem : MonoBehaviour, IPointerClickHandler
{
    public CheckboxScript Interaction;
    private bool isActive;
    public OwnTaskbarMenu taskbarMenu;
    // Start is called before the first frame update
    void Start()
    {
        isActive = false;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isActive)
        {
            taskbarMenu.stopInteraction(Interaction, this);
        }
        else
        {
            taskbarMenu.startInteraction(Interaction, this);
        }
    }
}
