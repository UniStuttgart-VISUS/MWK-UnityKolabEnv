using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.JSON;
using UnityEngine.Events;

public class ItemDisplay : MonoBehaviour
{
    public Text parameterName;

    public ParameterType currentParameter;

    public Toggle toggle;

    public DescriptionBox descriptionBox;

    

    // Start is called before the first frame update
    void Start()
    {
        if (currentParameter != null) SetParameter(currentParameter);

        toggle.onValueChanged.AddListener(ToggleListener());

        descriptionBox = GameObject.Find("DescriptionBox").GetComponent<DescriptionBox>();

    }

    // Update is called once per frame
    void Update()
    {
        
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
                descriptionBox.InitDescription(currentParameter);
            }
            else
            {
                Debug.Log(value);
            }
        };
    }
}
