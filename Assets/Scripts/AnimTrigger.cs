using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.Vive;
using UnityEngine;

public class TestAnimtrigger : MonoBehaviour
{
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ViveInput.GetPressDownEx(HandRole.RightHand, ControllerButton.DPadUp))
        {
            Debug.Log("Spinning");
            anim.Play("ExplodeAnimation");
        }
        
        if (ViveInput.GetPressDownEx(HandRole.RightHand, ControllerButton.DPadDown))
        {
            Debug.Log("Spinning");
            anim.Play("Unexplode");
        }
    }
}
