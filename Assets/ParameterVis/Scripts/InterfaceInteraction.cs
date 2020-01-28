using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface InterfaceInteraction<T>
{
    T GetValue();

    void SetValue(T newValue);

    void StartInteraction();

    void StartInteraction(T initValue);

    void StopInteraction();
}
