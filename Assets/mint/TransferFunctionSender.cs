using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using interop;

// Returns transform of the GameObject as Json string in interop format.
public class TransferFunctionSender : MonoBehaviour, IJsonStringSendable {

    public string Name = "TransferFunction";
    public List<TfPoint> pointsToSend = new List<TfPoint>();

    private void Start()
    {
	    //Start
    }

    public string nameString() {
        return this.Name;
	}

	public string jsonString() {
        TransferFunction mc = new TransferFunction();
        mc.type = 0;
        mc.maskMax = 1.0f;
        mc.maskMin = 0.0f;        
        
        mc.points = new List<TfPoint>();

        if (pointsToSend.Count > 0) mc.points = pointsToSend;
        else
        {

	        TfPoint tp = new TfPoint();
	        tp.pos = 0.22f;
	        tp.rgba = new Vector4(0.0f, 1.0f, 0.0f, 0.002f);
	        mc.points.Add(tp);

	        TfPoint tp2 = new TfPoint();
	        tp2.pos = 1.0f;
	        tp2.rgba = new Vector4(1.0f, 0.0f, 0.0f, 0.64f);
	        mc.points.Add(tp2);
        }

        string json = mc.json();
        return json;
	}
}
