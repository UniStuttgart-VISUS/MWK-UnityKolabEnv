using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using interop;

public class AbstractInteraction<T>: MonoBehaviour 
{

    protected Parameter<T> selectedValue;
    protected T value;

    // send the selected value to this VisParamMenu after a change
    public VisParamSender<T> senderManager;
    //public VisVarSender varSender;

    public Parameter<T> GetSelectedValue()
    {
        return selectedValue;
    }

    public void SetSelectedValue(Parameter<T> newValue)
    {
        selectedValue = newValue;
    }

    public void Start()
    {
        gameObject.SetActive(false);
    }

    //public void send()
    //{
    //    senderManager.send(selectedValue);
    //}

    private void StartInteraction()
    {
        gameObject.SetActive(true);
        Debug.Log("[VisInteraction]: bool param visible");
    }

    public void StartInteraction(Parameter<T> initValue, VisParamSender<T> sender)
    {
        Debug.Log("[VisInteraction]: start interaction");
        selectedValue = initValue;
        this.senderManager = sender;
        this.value = initValue.param;
        StartInteraction();
        Debug.Log("[VisInteraction]: bool param successful started");
    }

    public void StopInteraction()
    {
        gameObject.SetActive(false);
    }
}

public class UnityBoolInteraction: AbstractInteraction<bool>
{

}
