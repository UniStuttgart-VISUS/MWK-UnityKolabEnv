using System.Collections;
using System.Collections.Generic;
using Assets.JSON;
using UnityEngine;
using UnityEngine.UI;

public class ListDisplay : MonoBehaviour
{
    public Transform targetTransform;
    public ItemDisplay itemDisplay;

    // Start is called before the first frame update
    void Start()
    {
        
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
}
