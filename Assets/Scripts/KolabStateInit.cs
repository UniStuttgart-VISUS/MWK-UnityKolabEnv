using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NetMQ;
using UnityEngine;
using Valve.Newtonsoft.Json;

public class KolabStateInit : MonoBehaviour
{
    public GameObject[] internalRenderObjs;
    public GameObject[] externalRenderObjs;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        //Create a Kolab instance parametrized by the Environmental Constants
        if (EnvConstants.ExternalRendererMode == false)
        {
            foreach (GameObject go in externalRenderObjs)
            {
                go.SetActive(false);
                Debug.Log("Deactivating " + go.name);
            }

            foreach (GameObject go in internalRenderObjs)
            {
                go.SetActive(true);
                Debug.Log("Activating " + go.name);
            }
        }
        else
        {
            //ExternalApplicationController.Instance.StartInviwoInstance("C:\\Users\\flo\\Documents\\KolabWorking\\inviwo\\stereo_spheres.inv");
        }
        
        //Check if other instances are local within the same tracking space
        using (StreamReader r = new StreamReader("C:\\Program Files (x86)\\Steam\\config\\lighthouse\\lighthousedb.json"))
        {
            string json = r.ReadToEnd();
            dynamic conf = JsonConvert.DeserializeObject(json);
            foreach(var item in conf["base_stations"])
            {
                Debug.Log("Found base ID "+ item["config"]["serialNumber"]);
                EnvConstants.CollisionSN.Add(item["config"]["serialNumber"].ToString());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //do nothing in here!
    }
}
