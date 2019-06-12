using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(Rigidbody))]
public class InteractiveObject : MonoBehaviour {

    public Camera hmdCamera;

    public SteamVR_Input_Sources handType;
    public SteamVR_Action_Boolean rotationActiveAction;
    public SteamVR_Action_Vector2 rotationDirectionAction;

    public SteamVR_Action_Boolean scalingActiveAction;
    public SteamVR_Action_Vector2 scalingValueAction;

    // grabbing object
    public SteamVR_Action_Boolean grabActiveAction;
    protected Rigidbody rigidBody;
    protected bool originalKinematicState;
    protected Transform originalParent;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        originalParent = transform.parent;
        originalKinematicState = rigidBody.isKinematic;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (GetRotationActiveDown())
        {
            var rot = GetRotationDirection();
            rot.x = Mathf.Abs(rot.x) < 0.5 ? 0.0f : rot.x;
            rot.y = Mathf.Abs(rot.y) < 0.5 ? 0.0f : rot.y;
            //this.transform.Rotate(new Vector3(rot.y, -rot.x, 0.0f), Space.World);

            Vector3 rotAxisX = hmdCamera.transform.up;
            Vector3 rotAxisY = hmdCamera.transform.right;
            this.transform.RotateAround(this.transform.position, rotAxisX, -rot.x);
            this.transform.RotateAround(this.transform.position, rotAxisY, rot.y);

            return;
        }

        if(GetScalingActiveDown())
        {
            var scale = GetScalingValue();
            if(scale != 0.0f)
                this.transform.localScale = this.transform.localScale * ((scale < 0.0f) ? (0.99f) : (1.01f));
            return;
        }

        if(GetGrabActiveDown())
        {
            return;
        }
	}

    public bool GetRotationActiveDown()
    {
        return rotationActiveAction.GetState(handType);
        //return rotationActiveAction.GetStateDown(handType);
    }

    public Vector2 GetRotationDirection()
    {
        return rotationDirectionAction.GetAxis(handType);
        //return rotationDirectionAction.GetAxisDelta(handType);
    }

    public bool GetScalingActiveDown()
    {
        return scalingActiveAction.GetState(handType);
    }

    public float GetScalingValue()
    {
        return scalingValueAction.GetAxis(handType).y;
    }

    public bool GetGrabActiveDown()
    {
        return grabActiveAction.GetState(handType);
    }

    public void Pickup(GameObject holder)
    {
        rigidBody.isKinematic = true;

        transform.SetParent(holder.transform);
    }

    public void Release(GameObject holder)
    {
        if(transform.parent == holder.transform)
        {
            rigidBody.isKinematic = originalKinematicState;

            if(originalParent != holder.transform)
            {
                transform.SetParent(originalParent);
            }
            else
            {
                transform.SetParent(null);
            }
        }
    }
}
