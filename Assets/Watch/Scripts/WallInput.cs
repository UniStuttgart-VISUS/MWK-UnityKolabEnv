using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WallInput : MonoBehaviour
{
    public float Value;
    public Text myText;
    public Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetValue(int val)
    {
        myText.text = "Value: " + System.Convert.ToString(val);
    }

    public void IncrementValue()
    {
        Value++;
        SetValue((int) Value);
    }

    public void UpdateValue()
    {
        slider = GameObject.FindObjectOfType(typeof(Slider)) as Slider;
        Value = slider.value;
        SetValue((int)Value);
    }
}
