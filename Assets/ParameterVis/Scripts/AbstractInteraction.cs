using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractInteraction<T>: MonoBehaviour  
{
    private T value;

    T GetValue()
    {
        return value;
    }

    void SetValue(T newValue)
    {
        value = newValue;
    }


    void StartInteraction()
    {
        gameObject.SetActive(true);
    }

    void StartInteraction(T initValue)
    {
        value = initValue;
        StartInteraction();
    }

    void StopInteraction()
    {
        gameObject.SetActive(false);
    }
}
