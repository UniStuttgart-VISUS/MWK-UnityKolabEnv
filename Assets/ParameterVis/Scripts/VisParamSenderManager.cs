using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using interop;

public class VisParamSenderManager : MonoBehaviour, IJsonStringSendable
{
    // the names of all VisParamSender should be declared here and public
    public string boolSenderName = "boolSenderName";

    private VisParamSender<bool> boolSender;  

    // Start is called before the first frame update
    void Start()
    {
        // initialize all VisParamSender of different types here
        boolSender = new VisParamSender<bool>(boolSenderName, false);
    }

    /*
     * For each supported parameter type should exist an overloaded send() methode for this type
     */


    public void Send(Parameter<bool> param)
    {
        //boolSender.send(param);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string nameString()
    {
        throw new System.NotImplementedException();
    }

    public string jsonString()
    {
        throw new System.NotImplementedException();
    }

    public bool hasChanged()
    {
        throw new System.NotImplementedException();
    }
}
