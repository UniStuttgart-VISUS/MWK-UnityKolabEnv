using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowVectorPosition : MonoBehaviour
{
    public Vector3D Vector3D;
    public Text text; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = Vector3D.GetPosition();

        text.text = " X: " + System.Convert.ToString(position.x) + "\n Y: " + System.Convert.ToString(position.y) + "\n Z: " + System.Convert.ToString(position.z);
    }
}
