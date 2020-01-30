using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using interop;

public abstract class AbstractInteraction<T>: MonoBehaviour 
{
    protected Parameter<T> selectedValue;

    // send the selected value to this VisParamMenu after a change
    public VisParamSenderManager senderManager;

    public Parameter<T> GetSelectedValue()
    {
        return selectedValue;
    }

    public void SetSelectedValue(Parameter<T> newValue)
    {
        selectedValue = newValue;
    }

    private void StartInteraction()
    {
        gameObject.SetActive(true);
    }

    public void StartInteraction(Parameter<T> initValue)
    {
        selectedValue = initValue;
        StartInteraction();
    }

    public void StopInteraction()
    {
        gameObject.SetActive(false);
    }
}
