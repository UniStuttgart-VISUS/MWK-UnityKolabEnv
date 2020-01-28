using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisParamMenu : MonoBehaviour
{
    public InterfaceInteraction<List<string>> enumInteraction;
    public InterfaceInteraction<int> intInteraction;
    public InterfaceInteraction<float> floatInteraction;
    public InterfaceInteraction<bool> boolInteraction;
    public InterfaceInteraction<Vector3> vectorInteraction;

    private List<Parameter> parameterList;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
