using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour, InterfaceInteraction<List<string>>
{
    private List<string> enumValues;
    public VisParamMenu menu;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<string> GetValue()
    {
        throw new System.NotImplementedException();
    }

    public void SetValue(List<string> newValue)
    {
        throw new System.NotImplementedException();
    }

    void InterfaceInteraction<List<string>>.StartInteraction()
    {
        throw new System.NotImplementedException();
    }

    public void StartInteraction(List<string> initValue)
    {
        enumValues = initValue;
        throw new System.NotImplementedException();
    }

    void InterfaceInteraction<List<string>>.StopInteraction()
    {
        throw new System.NotImplementedException();
    }
}
