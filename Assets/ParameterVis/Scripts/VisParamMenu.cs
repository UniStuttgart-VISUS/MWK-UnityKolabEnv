using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using interop;

public class VisParamMenu : MonoBehaviour
{
    public AbstractInteraction<List<string>> enumInteraction;
    public AbstractInteraction<int> intInteraction;
    public AbstractInteraction<float> floatInteraction;
    public AbstractInteraction<bool> boolInteraction;
    public AbstractInteraction<Vector3> vectorInteraction;

    private List<object> parameterList;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void AddParameter(object p)
    {
        parameterList.Add(p);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
