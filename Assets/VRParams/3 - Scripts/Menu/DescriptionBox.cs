using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.JSON;

public class DescriptionBox : MonoBehaviour
{

    public Text description;

    private ParameterType currentParameter;

    public Text minimizedText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitDescription(ParameterType param)
    {
        string descStr = "";
        string minimizedStr = "";
        this.currentParameter = param;
        switch (param.type)
        {
            case Type.ENUM:
                descStr = "Selected Parameter: " + ((EnumParameter)param).name + "\n" + "CurrentValue: " + ((EnumParameter)param).currentValue;
                minimizedStr = ((EnumParameter)param).name + ": " + ((EnumParameter)param).currentValue;
                break;
            case Type.BOOLEAN:
                descStr = "Selected Parameter: " + ((BooleanParameter)param).name + "\n" + "CurrentValue: " + ((BooleanParameter)param).currentValue;
                minimizedStr = ((BooleanParameter)param).name + ": " + ((BooleanParameter)param).currentValue;
                break;
            case Type.FLOAT:
                descStr = "Selected Parameter: " + ((FloatParameter)param).name + "\n" + "CurrentValue: " + ((FloatParameter)param).currentValue;
                minimizedStr = ((FloatParameter)param).name + ": " + ((FloatParameter)param).currentValue;
                break;
            case Type.INTEGER:
                descStr = "Selected Parameter: " + ((IntegerParameter)param).name + "\n" + "CurrentValue: " + ((IntegerParameter)param).currentValue;
                minimizedStr = ((IntegerParameter)param).name + ": " + ((IntegerParameter)param).currentValue;
                break;
        }
        description.text = descStr;
        minimizedText.text = minimizedStr;
    }
}
