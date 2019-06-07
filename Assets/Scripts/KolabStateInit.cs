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

    public GameObject[] desktopModeObjs;
    public GameObject[] vrModeObjs;

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

        if (EnvConstants.DesktopMode == true)
        {
            foreach (GameObject go in vrModeObjs)
            {
                go.SetActive(false);
                Debug.Log("Deactivating " + go.name);
            }

            foreach (GameObject go in desktopModeObjs)
            {
                go.SetActive(true);
                Debug.Log("Activating " + go.name);
            }
        }

        //Check if other instances are local within the same tracking space (Base Station ID)
        if (!EnvConstants.DesktopMode)
        {
            using (StreamReader r = new StreamReader("C:\\Program Files (x86)\\Steam\\config\\lighthouse\\lighthousedb.json"))
            {
                string json = r.ReadToEnd();
                dynamic conf = JsonConvert.DeserializeObject(json);
                foreach (var item in conf["base_stations"])
                {
                    Debug.Log("Found base ID " + item["config"]["serialNumber"]);
                    if (!EnvConstants.CollisionSN.Exists(i => i == item["config"]["serialNumber"].ToString()))
                        EnvConstants.CollisionSN.Add(item["config"]["serialNumber"].ToString());
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //do nothing in here!
    }
}
