using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using ExitGames.Client.Photon;
using interop;
using Photon.Voice;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;
using HTC.UnityPlugin.Vive;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class TransferFunctionEditor : MonoBehaviour, IJsonStringSendable, IPointerEnterHandler, IPunObservable
    , IPointerExitHandler
    , IPointerClickHandler {

    public GameObject swatchPrefab;
    public TransferFunction currentTransferFunction;
    private HashSet<PointerEventData> hovers = new HashSet<PointerEventData>();
    // Start is called before the first frame update
    void Start()
    {
        //Register type for Photon
        PhotonPeer.RegisterType(typeof(TransferFunction), (byte)250, TransferFunction.Serialize, TransferFunction.Deserialize);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    TransferFunction SerializeToTransferFunction()
    {
        TransferFunction serTf = new TransferFunction();
        serTf.type = 0;
        serTf.maskMax = 1.0f;
        serTf.maskMin = 0.0f;        
        
        serTf.points = new List<TfPoint>();

        var pContainer = transform.Find("Points");
        //Clear old
        foreach (Transform child in pContainer.transform)
        {
            //Position
            TfPoint tp = new TfPoint();
            tp.pos = Mathf.Abs(child.localPosition.y / 3.0f);
            
            //Color
            Color colTemp = child.GetComponent<TransferFunctionSwatch>().selectedColor;
            tp.rgba = new Vector4(colTemp.r, colTemp.g, colTemp.b, colTemp.a);
            serTf.points.Add(tp);
        }

        return serTf;
    }
    
    void PopulateFromTransferFunction(TransferFunction tf)
    {
        var pContainer = transform.Find("Points");
        //Clear old
        foreach (Transform child in pContainer.transform)
        {
            Destroy(child.gameObject);
        }
        //Create new points from tf
        foreach (TfPoint point in tf.points)
        {
            GameObject capsule = Instantiate(swatchPrefab, new Vector3(0, -point.pos*3.0f, -0.2f), Quaternion.identity);
            //GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            capsule.transform.parent = pContainer.transform;
            capsule.transform.localPosition = new Vector3(0, -point.pos*3.0f, -0.2f);
            capsule.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
            capsule.GetComponent<TransferFunctionSwatch>().setColor(new Color(point.rgba.x,point.rgba.y,point.rgba.z, point.rgba.w));
        }
        
        //Set min max
        var minGO = transform.Find("Min").position;
        minGO = new Vector3(minGO.x, -tf.maskMin*3.0f ,minGO.z);
        
        var maxGO = transform.Find("Max").position;
        maxGO = new Vector3(maxGO.x, -tf.maskMax*3.0f ,maxGO.z);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hovers.Add(eventData) && hovers.Count == 1)
        {
            //Debug.Log("Bar Hover");
            Debug.Log(eventData.position);
            Debug.Log(eventData.pointerCurrentRaycast.worldPosition);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (hovers.Remove(eventData) && hovers.Count == 0)
        {
            //Debug.Log("Bar UnHover");// turn to normal state
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.IsViveButton(ControllerButton.Trigger) || true)
        {
            Debug.Log("Bar Click");
            var pContainer = transform.Find("Points");
            GameObject capsule = Instantiate(swatchPrefab, new Vector3(0, transform.InverseTransformPoint(eventData.pointerCurrentRaycast.worldPosition).y, -0.1f), Quaternion.identity);
            //GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            capsule.transform.parent = pContainer.transform;
            capsule.transform.localPosition = new Vector3(0, transform.InverseTransformPoint(eventData.pointerCurrentRaycast.worldPosition).y, -0.2f);
            capsule.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
            capsule.GetComponent<TransferFunctionSwatch>().setColor(Color.yellow);
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("Bar ClickLeft");
        }
    }
    
    public string nameString()
    {
        return "TransferFunction";
    }

    public string jsonString()
    {
        currentTransferFunction = SerializeToTransferFunction();
        string json = currentTransferFunction.json();
        return json;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //We own this dataset: send the others our data
            stream.SendNext(currentTransferFunction);
            Debug.Log("Send TF");
        }
        else
        {
            //Network dataset change, receive data
            TransferFunction recv = (TransferFunction) stream.ReceiveNext();
            PopulateFromTransferFunction(recv);
            Debug.Log("Received TF");
            Debug.Log(recv.json());
        }
    }
}
