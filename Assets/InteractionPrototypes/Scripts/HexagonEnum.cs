using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using HTC.UnityPlugin.Vive;
using UnityEngine.UI;
using interop;

/**
 * this Script manages the displayed values on the OctagonEnum Prefab. The displayable values are stored in values. 
 * Its necessary that values has at least as many entries as total sides on the Octagonenum. 
 */ 
public class HexagonEnum : UnityEnumInteraction, IPointerClickHandler
{
    //specific variables depending on the geometric form (octagon)
    private int upSides = 5;
    private int downsides = 3;
    private int totalsides = 8;

    private int rotation;
    private List<string> values = new List<string>(new string[]{"0","1","2","3","4","5","6","7","8","9"});
    private int backElementIndx;
    private int frontElementIndx;
    private int backValueIndx;
    private Text[] texts; 
    private bool isRotating = false;
    private int i;

    override public void StartInteraction(Parameter<List<string>> initValue, VisParamSender<List<string>> sender)
    {
        Debug.Log("[VisInteraction]: start interaction");
        selectedValue = initValue;
        this.senderManager = sender;
        this.value = initValue.param;

        values = new List<string>(initValue.param.ToArray());
        values.RemoveAt(0);
        gameObject.SetActive(true);
        setUpValuesList();
        
        while (!texts[frontElementIndx].text.Equals(selectedValue.param[0]))
        {

            if (!isRotating)
            {
                rotate();
            }
            Debug.Log("[Integerinteraction]: selectedValue = " + selectedValue.param[0] + ", frontElement = " + texts[frontElementIndx].text + ", " + !values[frontElementIndx].Equals(selectedValue.param[0]));
        }

        Debug.Log("[VisEnumInteraction]: Enum param successful started");
    }



    // Start is called before the first frame update
    new void Start()
    {
        gameObject.SetActive(false);
        setUpValuesList();
    }

    private void setUpValuesList()
    {
        //if (totalsides > values.Count)
        //{
        //    int diff = totalsides - values.Count;

        //    for (int i = 0; i < diff; i++ )
        //    {
        //        if (i >= values.Count)
        //        {
        //            i = 0;
        //        }
        //        values.Add(values[i]);
        //    }
        //}

        // get the text fields in rotation direction
        texts = GetComponentsInChildren<Text>();

        backElementIndx = upSides - 1;
        backValueIndx = (upSides - 1) % values.Count;
        frontElementIndx = 0;
        Debug.Log("[HexagonEnumScript]: frontElementIndx = " + frontElementIndx + ", texts: " + texts.Length);

        // set the texts of the sides which are up
        int valuesIdx = 0;

        for (int index = 0; index < upSides; index++)
        {
            texts[index].text = values[valuesIdx];

            valuesIdx++;
            if (valuesIdx >= values.Count)
            {
                valuesIdx = 0;
            }
        }

        // set the texts of the sides which are down
        valuesIdx = 1;
        for (int index = 1; index <= downsides; index++)
        {
            texts[texts.Length - index].text = values[values.Count - valuesIdx];

            valuesIdx++;
            if (valuesIdx >= values.Count)
            {
                valuesIdx = 0;
            }
        }
        isRotating = false;
        rotation = 0;
        i = -1;
    }

    private void rotate()
    {
        
        /*
         *change text of the back element too increase the number of displayable values to infinity
         */
        backElementIndx++;
        if (backElementIndx >= totalsides) // prefent index out of bounds
        {
            backElementIndx = 0;
        }

        frontElementIndx++;
        if (frontElementIndx >= totalsides) // prefent index out of bounds
        {
            frontElementIndx = 0;
        }

        backValueIndx = (backValueIndx + 1) % values.Count;

        texts[backElementIndx].text = values[backValueIndx];

        selectedValue.param[0] = texts[frontElementIndx].text;
        Debug.Log("[hexagonEnum]: front Text: " + selectedValue.param[0] + ", " + selectedValue.param.Count);
        Debug.Log("[hexagonEnum]: displayed Value: " + string.Join(", ", GetSelectedValue().param.ToArray()));
        Debug.Log("[hexagonEnum]: Values: " + string.Join(", ", values.ToArray()));

        if (senderManager != null)
        {
            senderManager.Send(base.selectedValue);
        }

        rotation = -45;
        i = -1;
    }

    public Text getFrontText()
    {
        return texts[frontElementIndx];
    }

    public bool SetValue(string value)
    {
        if (values.Contains(value))
        {
            while (!getFrontText().text.Equals(value))
            {
                rotate();
            }
            return true;
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (i >= rotation)
        {
            isRotating = true;
            this.transform.Rotate(new Vector3(i * 0.045f, 0, 0));
            //Debug.Log("i = " + i + ", " + rotation);
            i--;
        }
        else
        {
            isRotating = false;
        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.IsViveButton(ControllerButton.Trigger))
        {

            if (!isRotating)
            {
                rotate();
            }


        }
    }
}
