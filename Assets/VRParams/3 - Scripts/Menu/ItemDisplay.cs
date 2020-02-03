using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.JSON;
using UnityEngine.Events;
using interop;

public class ItemDisplay : MonoBehaviour
{
    public Text parameterName;

    public ParameterType currentParameter;

    public Toggle toggle;

    public DescriptionBox descriptionBox;

    public UnityBoolInteraction boolinteraction;
    public BoolSender boolSender;

    public UnityIntInteraction intInteraction;
    public IntSender intSender;

    public UnityEnumInteraction enumInteraction;
    public EnumSender enumSender;

    public UnityFloatInteraction floatInteraction;
    public FloatSender floatSender;

    public UnityVector3Interaction vec3Interaction;
    public Vec3Sender vec3Sender;

    public enum Types {BOOL, INT, FLOAT, ENUM, VEC3};

    public Parameter<bool> boolParam;
    public Parameter<int> intParam;
    public Parameter<float> floatParam;
    public Parameter<List<string>> enumParam;
    public Parameter<Vector3> vec3Param;

    public object currentParam;

    public Types currentType;

    // Start is called before the first frame update
    void Start()
    {
       // if (currentParameter != null) SetParameter(currentParameter);

        if (currentParam != null) SetObjParameter(currentParam);

        toggle.onValueChanged.AddListener(ToggleListener());

        descriptionBox = GameObject.Find("DescriptionBox").GetComponent<DescriptionBox>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetSenders(BoolSender boolSender, IntSender intSender, FloatSender floatSender, EnumSender enumSender, Vec3Sender vec3Sender)
    {
        this.boolSender = boolSender;
        this.intSender = intSender;
        this.floatSender = floatSender;
        this.enumSender = enumSender;
        this.vec3Sender = vec3Sender;
    }

    public void SetInteractions(UnityBoolInteraction bI, UnityIntInteraction iI, UnityFloatInteraction fI, UnityEnumInteraction eI, UnityVector3Interaction vI)
    {
        this.boolinteraction = bI;
        this.intInteraction = iI;
        this.floatInteraction = fI;
        this.enumInteraction = eI;
        this.vec3Interaction = vI;
    }
    /// <summary>
    /// Set the parameter for this item, which will be visible in the selector list
    /// </summary>
    /// <param name="parameter"></param>
    public void SetParameter(ParameterType parameter)
    {
        this.currentParameter = parameter;

        if (parameterName != null)
            parameterName.text = currentParameter.name;
    }

    public void SetObjParameter(object parameter)
    {
        string currentParamName = "";
        if (parameter is Parameter<bool>)
        {
            this.boolParam = (Parameter<bool>)parameter;
            currentParamName = boolParam.name;
            currentType = Types.BOOL;
        }
        else if (parameter is Parameter<int>)
        {
            this.intParam = (Parameter<int>)parameter;
            currentParamName = intParam.name;

            currentType = Types.INT;
        }
        else if (parameter is Parameter<float>)
        {
            this.floatParam = (Parameter<float>)parameter;
            currentParamName = floatParam.name;

            currentType = Types.FLOAT;
        }
        else if (parameter is Parameter<List<string>>)
        {
            this.enumParam = (Parameter<List<string>>)parameter;
            currentParamName = enumParam.name;

            currentType = Types.ENUM;
        }
        else if (parameter is Parameter<Vector3>)
        {
            this.vec3Param = (Parameter<Vector3>)parameter;
            currentParamName = vec3Param.name;

            currentType = Types.VEC3;
        }

        if (parameterName != null)
            parameterName.text = currentParamName;
    }

    /// <summary>
    /// This function creates a listener for the current given toggle item in the configurator list.
    /// If selected, the selected parameter will be active for manipulation.
    /// </summary>
    /// <returns>Returns the unity action</returns>
    private UnityAction<bool> ToggleListener()
    {
        return (value) =>
        {
            if (value)
            {
                //descriptionBox.InitDescription(currentParameter);

                switch (currentType)
                {
                    case Types.BOOL:
                        boolinteraction.StartInteraction(boolParam, boolSender);
                        break;
                    case Types.INT:
                        intInteraction.StartInteraction(intParam, intSender);
                        break;
                    case Types.FLOAT:
                        floatInteraction.StartInteraction(floatParam, floatSender);
                        break;
                    case Types.ENUM:
                        enumInteraction.StartInteraction(enumParam, enumSender);
                        break;
                    case Types.VEC3:
                        vec3Interaction.StartInteraction(vec3Param, vec3Sender);
                        break;
                }
            }
            else
            {
                switch (currentType)
                {
                    case Types.BOOL:
                        boolinteraction.StopInteraction();
                        break;
                    case Types.INT:
                        intInteraction.StopInteraction();
                        break;
                    case Types.FLOAT:
                        floatInteraction.StopInteraction();
                        break;
                    case Types.ENUM:
                        enumInteraction.StopInteraction();
                        break;
                    case Types.VEC3:
                        vec3Interaction.StopInteraction();
                        break;
                }
            }
        };
    }
}

