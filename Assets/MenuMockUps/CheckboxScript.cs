using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using HTC.UnityPlugin.Vive;

public class CheckboxScript : MonoBehaviour, IPointerClickHandler
{
    private bool isChecked;
    private GameObject cross;
    public bool initValueTrue;
    public OverlayMenu OverlayMenu;

    // Start is called before the first frame update
    void Start()
    {
        cross = this.transform.GetChild(1).gameObject;
        isChecked = initValueTrue;
        gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.IsViveButton(ControllerButton.Trigger))
        {
            cross.SetActive(isChecked);
            isChecked = !isChecked;
        }
    }

    public OverlayMenu GetMenu()
    {
        return OverlayMenu;
    }

    public void Appear()
    {
        gameObject.SetActive(true);
    }

    public void Disappear()
    {
        gameObject.SetActive(false);
    }
}
