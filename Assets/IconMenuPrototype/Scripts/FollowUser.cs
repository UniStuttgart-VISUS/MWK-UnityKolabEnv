using HTC.UnityPlugin.Vive;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FollowUser : MonoBehaviour, IPointerClickHandler
{
    public GameObject camera;

    private bool shouldFollow = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void InitFollow()
    {
        transform.GetComponent<MeshRenderer>().material.color = Color.blue;
        shouldFollow = true;
        Debug.Log("init following");
    }

    void StopFollow()
    {
        shouldFollow = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldFollow)
        {
            Vector3 position = camera.transform.position;
            float rotation_x = camera.transform.rotation.x;
            float rotation_y = camera.transform.rotation.y;
            float rotation_z = camera.transform.rotation.z;

            transform.position = position + new Vector3(1 + rotation_y, rotation_x, 2 + rotation_z);
            transform.rotation.SetLookRotation(new Vector3(rotation_x, rotation_y + 45, rotation_z));
        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.IsViveButton(ControllerButton.Trigger))
        {
            StopFollow();
            Debug.Log("Stoped following");
        }
    }
}