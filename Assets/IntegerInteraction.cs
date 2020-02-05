using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using HTC.UnityPlugin.Vive;
using System;
using interop;

public class IntegerInteraction : UnityIntInteraction, IPointerClickHandler
{
    private List<HexagonEnum> digits = new List<HexagonEnum>();
    // Start is called before the first frame update
    new void Start()
    {
        //digits = GetComponentsInChildren<GameObject<HexagonEnum>>();
        foreach (Transform child in this.transform)
        {
            if (child.GetComponent<HexagonEnum>())
            {
                digits.Add(child.GetComponent<HexagonEnum>());
            }
        }

        base.Start();
    }

    public override void StartInteraction(Parameter<int> initValue, VisParamSender<int> sender)
    {
        base.StartInteraction(initValue, sender);

        // Set the front Text of each digit to get the number stored in initValue
        int value = selectedValue.param;
        float tensBasis = 10000f;
        int tmp;
        Parameter<List<string>> tmpParam = new Parameter<List<string>>();

        Debug.Log("[IntegerInteraction]: digits: " + digits.Count);
        Debug.Log("[IntegerInteraction]: number: " + value);
        int number = value; // = and int
        Debug.Log("[IntegerInteraction]: number: " + value);
        LinkedList<int> stack = new LinkedList<int>();
        while (number > 0)
        {
            stack.AddLast(number % 10);
            number = number / 10;
        }
        Debug.Log("[IntegerInteraction]: number: " + value);


        foreach (HexagonEnum digit in digits)
        {
            if (stack.Count > 0)
            {
                tmp = stack.First.Value;
                stack.RemoveFirst();
            }else
            {
                tmp = 0;
            }
           
            Debug.Log("[IntegerInteraction]: digit " + tensBasis/10000 + " with value " + tmp);
            tmpParam.param = new List<string>(new string[] { tmp + "", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" });
            digit.StartInteraction(tmpParam, null);
            tensBasis /= 10;
        }
    }

    private void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.IsViveButton(ControllerButton.Trigger))
        {
            int number = 0;
            int tensBasis = 10000;

            // Get the number out of the digits
            foreach (HexagonEnum digit in digits)
            {
                number += Int32.Parse(digit.getFrontText().text) * number;
                tensBasis /= 10;
            }
            selectedValue.param = number;
            Debug.Log("[IntegerInteraction]: number = " + number);
            senderManager.Send(selectedValue);
        }
    }

}
