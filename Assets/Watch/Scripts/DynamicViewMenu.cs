using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HTC.UnityPlugin.Vive;

public class DynamicViewMenu : MonoBehaviour
{

    private float speed = 1.2f;
    private Vector3 originalPos;
    private Vector3 targetPos;
    private Vector3 originalRot;
    private Vector3 targetRot;
    public HandRole rightHandRole;
    public HandRole leftHandRole;
    public GameObject minimizedMenu;
    public GameObject maximizedMenu;
    private Button minimizeButton;
    private Button maximizeButton;

    // Start is called before the first frame update
    void Start()
    {
        originalPos = transform.localPosition;
        targetPos = new Vector3(0, 0, 0.7f);

        originalRot = transform.localEulerAngles;
        targetRot = Vector3.zero;

        minimizeButton = maximizedMenu.GetComponentInChildren<Button>();
        maximizeButton = minimizedMenu.GetComponent<Button>();

        minimizedMenu.SetActive(true);
        maximizedMenu.SetActive(false);

        maximizeButton.onClick.AddListener(() => moveToFrontView());
        minimizeButton.onClick.AddListener(() => moveToSideView());
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void moveToFrontView()
    {
        if (transform.localPosition == originalPos)
        {
            Debug.Log("zum Target bewegen");
            StartCoroutine(MoveAtSpeedCoroutine(targetPos, targetRot, speed, true));
        }
    }

    private void moveToSideView()
    {
        if (transform.localPosition == targetPos)
        {
            Debug.Log("zum ursprung zurück");
            StartCoroutine(MoveAtSpeedCoroutine(originalPos, originalRot, speed, false));
        }
    }


    public IEnumerator MoveAtSpeedCoroutine(Vector3 end, Vector3 targetRot, float speed, bool moveToFrontView)
    {
        while (Vector3.Distance(this.transform.localPosition, end) > speed * Time.deltaTime || Vector2.Distance(this.transform.localEulerAngles, targetRot) > speed * Time.deltaTime)
        {
            this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, end, speed * Time.deltaTime);
            this.transform.localEulerAngles = Vector3.MoveTowards(this.transform.localEulerAngles, targetRot, speed* 100 * Time.deltaTime);
            yield return 0;
        }
        this.transform.localPosition = end;
        this.transform.localEulerAngles = targetRot;

        // enable and disable maximized and minimized menu according to moving state
        if (moveToFrontView)
        {
            minimizedMenu.SetActive(false);
            maximizedMenu.SetActive(true);
        } else
        {
            minimizedMenu.SetActive(true);
            maximizedMenu.SetActive(false);
        }
        
    }

    


}
