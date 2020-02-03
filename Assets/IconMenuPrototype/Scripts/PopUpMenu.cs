using HTC.UnityPlugin.Vive;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopUpMenu : MonoBehaviour
{
    public GameObject Camera; // the canvas of the icon, which starts this menu
    private bool isActive = false;
    public Canvas canvas;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(isActive);
    }

    // menu will appear in front of the user
    public void Appear()
    {
        // set the position of the gameobject in front of the player
        transform.position = canvas.transform.position;

        transform.rotation = new Quaternion(transform.rotation.x, canvas.transform.rotation.y, transform.rotation.z, transform.rotation.w);

        isActive = true;
        gameObject.SetActive(isActive);
        Debug.Log("Menu appeared");
    }

    // menu disappears
    public void Disappear()
    {
        gameObject.SetActive(false);
        Debug.Log("Menu disappeared");
    }

    // Update is called once per frame
    void Update()
    {
        // set the position of the gameobject in front of the player
        //Vector3 position = Camera.transform.position;
        //Vector3 rotation = Camera.transform.rotation.eulerAngles;
        //Debug.Log(rotation.x + ", " + rotation.y + ", " + rotation.z);
        //Debug.Log("position camera: " + position);
        //Debug.Log("position changes: " + Mathf.Cos(rotation.y) + ", " + Mathf.Sin(rotation.y));
        //gameObject.transform.position = position + new Vector3(1 + Mathf.Cos(rotation.y/10), 0, 2 + Mathf.Sin(rotation.y/10) );

        //Debug.Log("position menu: " + gameObject.transform.position.x + ", " + gameObject.transform.position.y + ", " + gameObject.transform.position.z);


        ////transform.rotation.eulerAngles.Set(rotation.x, rotation.y + 45, rotation.z);
        //transform.rotation.SetLookRotation(new Vector3(rotation.x, rotation.y + 45, rotation.z));

        //Debug.Log("rotation menu: " + gameObject.transform.rotation.x + ", " + gameObject.transform.rotation.y + ", " + gameObject.transform.rotation.z);

        //isActive = true;
        //gameObject.SetActive(isActive);
        //Debug.Log("Menu appeared");

        //transform.position = canvas.transform.position;

        //transform.rotation = new Quaternion(transform.rotation.x, canvas.transform.rotation.y, transform.rotation.z, transform.rotation.w);

    }
}
