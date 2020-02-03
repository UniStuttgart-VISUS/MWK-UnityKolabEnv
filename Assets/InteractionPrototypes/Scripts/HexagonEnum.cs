using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using HTC.UnityPlugin.Vive;
using UnityEngine.UI;

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
    public string[] values = {"0","1","2","3","4","5","6","7","8","9","10"};
    private int backElementIndx;
    private int frontElementIndx;
    private int backValueIndx;
    private Text[] texts; 
    private bool curLock;
    private int i;

    // Start is called before the first frame update
    void Start()
    {
        if (totalsides > values.Length)
        {
            throw new System.ArgumentException("Its necessary that values has at least as many entries as total sides on the GameObject.");
        }


        // get the text fields in rotation direction
        texts = GetComponentsInChildren<Text>();

        backElementIndx = upSides - 1;
        backValueIndx = upSides - 1;
        frontElementIndx = 0;

        // set the texts of the sides which are up
        for (int index = 0; index < upSides; index++)
        {
            texts[index].text = values[index];
        }

        // set the texts of the sides which are down
        for (int index = 1; index <= downsides; index++)
        {
            texts[texts.Length - index].text = values[values.Length - index];
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
            if (backValueIndx >= values.Length) // prefent index out of bounds
            {
                backValueIndx = 0;
            }
            texts[backElementIndx].text = values[backValueIndx];


             
            rotation = -45;
            i = -1;
            
        }
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
