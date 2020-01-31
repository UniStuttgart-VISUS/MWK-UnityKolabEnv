using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using interop;

public abstract class AbstractInteraction<T>: MonoBehaviour 
{
    protected Parameter<T> selectedValue;

    // send the selected value to this VisParamMenu after a change
    public VisParamSenderManager senderManager;
    public VisVarSender varSender;

    public Parameter<T> GetSelectedValue()
    {
        return selectedValue;
    }

    public void SetSelectedValue(Parameter<T> newValue)
    {
        selectedValue = newValue;
    }

    //public void send()
    //{
    //    senderManager.send(selectedValue);
    //}

    private void StartInteraction()
    {
        gameObject.SetActive(true);
    }

    public void StartInteraction(Parameter<T> initValue, VisParamSenderManager senderManager, VisVarSender varSender)
    {
        selectedValue = initValue;
        this.varSender = varSender;
        this.senderManager = senderManager;
        StartInteraction();
    }

    public void StopInteraction()
    {
        gameObject.SetActive(false);
    }
}
