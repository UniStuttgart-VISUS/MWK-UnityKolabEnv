using System.CodeDom;
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ColorPickerTriangle : MonoBehaviour, IPointerEnterHandler
    , IPointerExitHandler
    , IPointerClickHandler
    , IPointerDownHandler
    , IPointerUpHandler
    , IDragHandler {

    public Color TheColor = Color.cyan;

    const float MainRadius = 5.8f;
    const float CRadius = 0.5f;
    const float CWidth = 0.1f;
    const float TRadius = 1.0f;

    public GameObject Triangle;
    public GameObject PointerColor;
    public GameObject PointerMain;
    public GameObject AlphaBar;
    public GameObject PointerAlpha;

    private Mesh TMesh;
    private Mesh AMesh;
    private Plane MyPlane;
    private Vector3[] RPoints;
    private Vector3 CurLocalPos;
    private Vector3 CurBary = Vector3.up;
    private Color CircleColor = Color.red;
    private Color AlphaCircleColor = Color.red;
    private bool DragCircle = false;
    private bool DragTriangle = false;

    private bool MousePressed = false;

    private string startGO = "";

	// Use this for initialization
	void Awake () {
        //Init
        float h, s, v;
        Color.RGBToHSV(TheColor, out h, out s, out v);
        MyPlane = new Plane(transform.TransformDirection(Vector3.forward), transform.position);
        RPoints = new Vector3[3];
        SetTrianglePoints();
        TMesh = Triangle.GetComponent<MeshFilter>().mesh;
        AMesh = AlphaBar.GetComponent<MeshFilter>().mesh;
        SetNewColor(TheColor);
        
        Color[] colors = new Color[AMesh.colors.Length];
        for (int i = 0; i < AMesh.colors.Length; i++)
        {
            float a = 1.0f / AMesh.colors.Length * i; 
            colors[i] = new Color(1.0f,0.0f,0.0f, a);
        }

        AMesh.colors = colors;
    }
	
	// Update is called once per frame
	void Update () {
        
    }

    private void StopDrag()
    {
        DragCircle = false;
        DragTriangle = false;
    }

    public void SetNewColor(Color NewColor)
    {
        TheColor = NewColor;
        float h, s, v;
        Color.RGBToHSV(TheColor, out h, out s, out v);
        CircleColor = Color.HSVToRGB(h, 1, 1);
        ChangeTriangleColor(CircleColor);
        ChangeAlphaColor(CircleColor);
        PointerMain.transform.localEulerAngles = Vector3.back * (h * 360f);
        PointerAlpha.transform.localEulerAngles = Vector3.back * NewColor.a;
        CurBary.y = 1f - v;
        CurBary.x = v * s;
        CurBary.z = 1f - CurBary.y - CurBary.x;
        CurLocalPos = RPoints[0] * CurBary.x + RPoints[1] * CurBary.y + RPoints[2] * CurBary.z;
        PointerColor.transform.localPosition = CurLocalPos;
    }

    private void CheckCirclePosition()
    {
        float a = Vector3.Angle(Vector3.left, CurLocalPos);
        if (CurLocalPos.y < 0)
            a = 360f - a;

        CircleColor = Color.HSVToRGB(a / 360, 1, 1); 
        ChangeTriangleColor(CircleColor);
        ChangeAlphaColor(CircleColor);
        PointerMain.transform.localEulerAngles = Vector3.back * a;
        SetColor();
    }
    
    private void CheckAlphaCirclePosition()
    {
        float a = Vector3.Angle(Vector3.left, CurLocalPos);
        if (CurLocalPos.y < 0)
            a = 360f - a;

        PointerAlpha.transform.localEulerAngles = Vector3.back * a;
        AlphaCircleColor = new Color(0f,0f,0f, a/360);
        SetColor();
    }

    private void CheckTrianglePosition()
    {
        Vector3 b = Barycentric(CurLocalPos, RPoints[0], RPoints[1], RPoints[2]);
        if (b.x >= 0f && b.y >= 0f && b.z >= 0f)
        {
            CurBary = b;
            PointerColor.transform.localPosition = CurLocalPos;
            DragTriangle = !DragCircle;
            SetColor();
        }
    }

    private void SetColor()
    {
        float h, v, s;
        Color.RGBToHSV(CircleColor, out h, out v, out s);
        Color c = (CurBary.y > .9999) ? Color.black : Color.HSVToRGB(h, CurBary.x / (1f - CurBary.y), 1f - CurBary.y);
        TheColor = c;
        TheColor.a = AlphaCircleColor.a;
    }

    private void ChangeTriangleColor(Color c)
    {
        Color[] colors = new Color[TMesh.colors.Length];
        colors[0] = Color.black;
        colors[1] = c;
        colors[2] = Color.white;
        TMesh.colors = colors;
    }
    
    private void ChangeAlphaColor(Color c)
    {
        Color[] colors = new Color[AMesh.colors.Length];
        for (int i = 0; i < AMesh.colors.Length; i++)
        {
            float a = 1.0f / AMesh.colors.Length * i; 
            colors[i] = new Color(c.r,c.g,c.b, a);
        }

        AMesh.colors = colors;
    }

    private Vector3 Barycentric(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 bary = Vector3.zero;
        Vector3 v0 = b - a;
        Vector3 v1 = c - a;
        Vector3 v2 = p - a;
        float d00 = Vector3.Dot(v0, v0);
        float d01 = Vector3.Dot(v0, v1);
        float d11 = Vector3.Dot(v1, v1);
        float d20 = Vector3.Dot(v2, v0);
        float d21 = Vector3.Dot(v2, v1);
        float denom = d00 * d11 - d01 * d01;
        bary.y = (d11 * d20 - d01 * d21) / denom;
        bary.z = (d00 * d21 - d01 * d20) / denom;
        bary.x = 1.0f - bary.y - bary.z;
        return bary;
    }


    private void SetTrianglePoints()
    {
        RPoints[0] = Vector3.up * TRadius;
        float c = Mathf.Sin(Mathf.Deg2Rad * 30);
        float s = Mathf.Cos(Mathf.Deg2Rad * 30);
        RPoints[1] = new Vector3 (s, -c, 0) * TRadius;
        RPoints[2] = new Vector3(-s, -c, 0) * TRadius;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Col Poi enter");
        Debug.Log((eventData.pointerCurrentRaycast.gameObject));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Col Poi exit");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Col Poi click");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Col Poi down");
        MousePressed = true;
        startGO = eventData.pointerCurrentRaycast.gameObject.name;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Col Poi up");
        startGO = "";
        MousePressed = false;
        StopDrag();
    }

    public void OnDrag(PointerEventData eventData)
    {
        CurLocalPos = transform.InverseTransformPoint(eventData.pointerCurrentRaycast.worldPosition);
        if (startGO == "ColorCircle")
        {
            CheckCirclePosition();
        } 
        else if (startGO == "AlphaCircle")
        {
            CheckAlphaCirclePosition();
        }
        else if(startGO == "ColorTriangle")
        {
            CheckTrianglePosition();
        }
    }
}
