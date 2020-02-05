using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HTC.UnityPlugin.Vive;

public class DynamicViewMenu : VisParamMenu
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

    public ListDisplay listDisplay;
    public DescriptionBox description;

    // Communication

    public UnityBoolInteraction boolinteraction;
    public BoolSender boolSender;

    public UnityIntInteraction intInteraction;
    public IntSender intSender;

    public UnityEnumInteraction enumInteraction;
    public EnumSender enumSender;

    public UnityFloatInteraction floatInteraction;
    public FloatSender floatSender;

    public UnityVector3Interaction vec3Interaction;
    public Vec3Sender vec3Sender;

    private int prevSize;

    // Start is called before the first frame update
    void Start()
    {

        // might have to restart if new parameter get added!!
        listDisplay.InitDisplay(parameterList);
        listDisplay.SetInteractions(boolinteraction, intInteraction, floatInteraction, enumInteraction, vec3Interaction);
        listDisplay.SetSenders(boolSender, intSender, floatSender, enumSender, vec3Sender);

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

        prevSize = parameterList.Count;
        
    }

    // Update is called once per frame
    void Update()
    {

        if (parameterList.Count > prevSize)
        {
            listDisplay.InitDisplay(parameterList);
        }
        prevSize = parameterList.Count;

        if (boolinteraction.gameObject.active)
        {
            description.InitDescription(boolinteraction.GetSelectedValue().name, boolinteraction.GetSelectedValue().param.ToString());
        }
        else if (intInteraction.gameObject.active)
        {
            description.InitDescription(intInteraction.GetSelectedValue().name, intInteraction.GetSelectedValue().param.ToString());
        }
        else if (floatInteraction.gameObject.active)
        {
            description.InitDescription(floatInteraction.GetSelectedValue().name, floatInteraction.GetSelectedValue().param.ToString());
        }
        else if (enumInteraction.gameObject.active)
        {
            description.InitDescription(enumInteraction.GetSelectedValue().name, string.Join(", ", enumInteraction.GetSelectedValue().param.ToArray()));//enumInteraction.GetSelectedValue().param[0]);
        }
        else if (vec3Interaction.gameObject.active)
        {
            description.InitDescription(vec3Interaction.GetSelectedValue().name, vec3Interaction.GetSelectedValue().param.ToString());
        }
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
