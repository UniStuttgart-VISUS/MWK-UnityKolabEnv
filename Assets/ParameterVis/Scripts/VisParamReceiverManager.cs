using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using interop;

public class VisParamReceiverManager : MonoBehaviour
{
    // the names of all VisParamReceivers should be declared here and public
    public string boolReceiverName = "boolReceiverName";
    public VisParamMenu menu;

    private VisParamReceiver<bool> boolReceiver;

    // Start is called before the first frame update
    void Start()
    {
        // initialize all VisParamSender of different types here
        boolReceiver = new VisParamReceiver<bool>(boolReceiverName, menu);
    }

    /*
     * For each supported parameter type should exist an overloaded send() methode for this type
     */


   

    // Update is called once per frame
    void Update()
    {

    }
}
