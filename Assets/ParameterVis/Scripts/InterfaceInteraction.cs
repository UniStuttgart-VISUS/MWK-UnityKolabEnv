using HTC.UnityPlugin.Vive;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface InterfaceInteraction<T>: IPointerClickHandler
{
    T GetSelectedValue();

    void SetSelectedValue(T newValue);

    bool HasChanged();

    void StartInteraction();

    void StartInteraction(T initValue);

    void StopInteraction();
}
