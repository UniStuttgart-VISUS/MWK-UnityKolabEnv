using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayMenu : MonoBehaviour
{
    public CheckboxScript checkbox;
    public OverlayMenu menu;
    public PopUpMenuIcon Icon;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public CheckboxScript GetInteraction()
    {
        return checkbox;
    }

    public OverlayMenu GetMenu()
    {
        return menu;
    }

    public PopUpMenuIcon GetPopUpMenuIcon()
    {
        return Icon;
    }

    public void Appear()
    {

        gameObject.SetActive(true);
        Debug.Log("OverlayMenu appeared");
    }

    // menu disappears
    public void Disappear()
    {
        gameObject.SetActive(false);
        Debug.Log("OverlayMenu disappeared");
    }
}
