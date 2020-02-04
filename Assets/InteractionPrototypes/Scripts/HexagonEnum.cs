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
    private bool curLock;
    private int i;

    override public void StartInteraction(Parameter<List<string>> initValue, VisParamSender<List<string>> sender)
    {
        Debug.Log("[VisInteraction]: start interaction");
        selectedValue = initValue;
        this.senderManager = sender;
        this.value = initValue.param;

        values = initValue.param;
        values.RemoveAt(0);
        gameObject.SetActive(true);
        setUpValuesList();

        while (!values[frontElementIndx].Equals(selectedValue.param[0]))
        {
            rotate();
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
        if (totalsides > values.Count)
        {
            int diff = totalsides - values.Count;

            for (int i = 0; i < diff; i++ )
            {
                if (i >= values.Count)
                {
                    i = 0;
                }
                values.Add(values[i]);
            }
        }


        // get the text fields in rotation direction
        texts = GetComponentsInChildren<Text>();

        backElementIndx = upSides - 1;
        backValueIndx = upSides - 1;
        frontElementIndx = 0;
        base.selectedValue.param = new List<string>(1);
        Debug.Log("[HexagonEnumScript]: frontElementIndx = " + frontElementIndx);
        base.selectedValue.param[0] = getFrontText().text;

        // set the texts of the sides which are up
        for (int index = 0; index < upSides; index++)
        {
            texts[index].text = values[index];
        }

        // set the texts of the sides which are down
        for (int index = 1; index <= downsides; index++)
        {
            texts[texts.Length - index].text = values[values.Count - index];
        }
        curLock = false;
        rotation = 0;
        i = -1;
    }

    // Update is called once per frame
    void Update()
    {
        float currentRot = this.transform.rotation.eulerAngles.x;
        if (i >= rotation)
        {
            curLock = true;
            this.transform.Rotate(new Vector3(i*0.045f, 0, 0));
            //Debug.Log("i = " + i + ", " + rotation);
            i--;
        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.IsViveButton(ControllerButton.Trigger))
        {
            rotate();

        }
    }

    private void rotate()
    {
        float currentRot = this.transform.rotation.eulerAngles.x;
        //this.transform.Rotate(new Vector3(currentRot - (currentRot % 45), 0, 0));
        currentRot = (currentRot % -45);
        Debug.Log("Currenr Rotation: " + currentRot);
        if (currentRot == 0.0)
        {
            ;
        }

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

        backValueIndx++;
        if (backValueIndx >= values.Count) // prefent index out of bounds
        {
            backValueIndx = 0;
        }
        texts[backElementIndx].text = values[backValueIndx];

        base.selectedValue.param[0] = texts[frontElementIndx].text;
        base.senderManager.Send(base.selectedValue);

        rotation = -45;
        i = -1;
    }

    public Text getFrontText()
    {
        return texts[frontElementIndx];
    }

    IEnumerator ExampleCoroutine(int i)
    {
        //Print the time of when the function is first called.
        //Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        
        yield return new WaitForSeconds(1000);

        //After we have waited 5 seconds print the time again.
        //Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }
}
