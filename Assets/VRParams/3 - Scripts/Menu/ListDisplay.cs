using System.Collections;
using System.Collections.Generic;
using Assets.JSON;
using UnityEngine;
using UnityEngine.UI;

public class ListDisplay : MonoBehaviour
{
    public Transform targetTransform;
    public ItemDisplay itemDisplay;

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

    ToggleGroup group;

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

    // Start is called before the first frame update
    void Start()
    {
        group = targetTransform.GetComponent<ToggleGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitDisplay(Configuration configurations)
    {
        ToggleGroup group = targetTransform.GetComponent<ToggleGroup>();

        foreach (ParameterType conf in configurations.allParameters)
        {
            ItemDisplay display = (ItemDisplay)Instantiate(itemDisplay);
            display.transform.SetParent(targetTransform, false);
            display.toggle.group = group;
            display.SetParameter(conf);
        }
    }

    public void InitDisplay(List<object> parameterList)
    {
        Debug.Log("[ListDisplay]: targetCount before: " + targetTransform.childCount);
        while (targetTransform.childCount != 0)
        {
            foreach (Transform child in targetTransform)
            {
                child.parent = null;
                Destroy(child.gameObject);
            }
        }
        Debug.Log("[ListDisplay]: targetCount after: " + targetTransform.childCount);

        ToggleGroup group = targetTransform.GetComponent<ToggleGroup>();
        foreach (object param in parameterList)
        {
            ItemDisplay display = (ItemDisplay)Instantiate(itemDisplay);
            display.SetSenders(boolSender, intSender, floatSender, enumSender, vec3Sender);
            display.SetInteractions(boolinteraction, intInteraction, floatInteraction, enumInteraction, vec3Interaction);
            display.transform.SetParent(targetTransform, false);
            display.toggle.group = group;
            display.SetObjParameter(param);
        }
    }

    public void addParameterButton(object parameter)
    {
        ItemDisplay display = (ItemDisplay)Instantiate(itemDisplay);
        display.SetSenders(boolSender, intSender, floatSender, enumSender, vec3Sender);
        display.SetInteractions(boolinteraction, intInteraction, floatInteraction, enumInteraction, vec3Interaction);
        display.transform.SetParent(targetTransform, false);
        display.toggle.group = group;
        display.SetObjParameter(parameter);
    }
}
