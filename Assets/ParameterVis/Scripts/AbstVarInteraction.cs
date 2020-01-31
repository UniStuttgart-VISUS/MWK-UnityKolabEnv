using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using interop;

public class AbstVarInteraction<T> : MonoBehaviour
{
    protected VarParameter selectedValue;
    protected T value;

    // send the selected value to this VisParamMenu after a change
    public VisVarSender varSender;

    public VarParameter GetSelectedValue()
    {
        return selectedValue;
    }

    public void SetSelectedValue(VarParameter newValue)
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

    public void StartInteraction(VarParameter initValue, VisVarSender varSender)
    {
        selectedValue = initValue;

        if (initValue.value is T)
        {
            value = (T) initValue.value;
        }
        this.varSender = varSender;
        StartInteraction();
    }

    public void StopInteraction()
    {
        gameObject.SetActive(false);
    }
}
