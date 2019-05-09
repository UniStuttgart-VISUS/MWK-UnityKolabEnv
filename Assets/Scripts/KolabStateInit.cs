using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            ExternalApplicationController.Instance.StartInviwoInstance("C:\\Users\\flo\\Documents\\KolabWorking\\inviwo\\stereo_spheres.inv");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //do nothing in here!
    }
}
