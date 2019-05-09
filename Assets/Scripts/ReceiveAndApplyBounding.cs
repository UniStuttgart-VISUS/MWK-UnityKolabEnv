using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using NetMQ;
using NetMQ.Sockets;
using Valve.Newtonsoft.Json;
using WebSocketSharp;

public class ReceiveAndApplyBounding : MonoBehaviour
{
    public GameObject netManager;
    private Thread subscriberThread;
    private SubscriberSocket subSocket;
    private bool running;
    private bool dirty = false;
    public SendAll zmqSenderScript;
    private InvBoundingBox bbox;
    public Vector3 targetSize = new Vector3(3.0f, 3.0f, 3.0f);
    
    // Start is called before the first frame update
    void Start()
    {
        running = true;
        subscriberThread = new Thread(NetMqSubscriber);
        subscriberThread.Start("tcp://127.0.0.1:12346");
    }

    // Update is called once per frame
    void Update()
    {
        if (dirty)
        {
            //Prepare
            Vector3 centerPos = new Vector3(bbox.leftLower.x + bbox.rightUpper.x, bbox.leftLower.y + bbox.rightUpper.y,bbox.leftLower.z + bbox.rightUpper.z ) / 2;
            Debug.Log("New center pos: "+centerPos);
 
            float scaleX = Mathf.Abs(bbox.leftLower.x - bbox.rightUpper.x);
            float scaleY = Mathf.Abs(bbox.leftLower.y - bbox.rightUpper.y);
            float scaleZ = Mathf.Abs(bbox.leftLower.z - bbox.rightUpper.z);
            
            Debug.Log("Scale factors: "+scaleX+"  "+scaleY+"  "+scaleZ);
 
            //Apply largest multiplier to zmqsender
            var mult = Mathf.Max(scaleX / targetSize.x, scaleY / targetSize.y, scaleZ / targetSize.z);
            zmqSenderScript.multiplier = mult;
          
            transform.localScale = new Vector3(1/mult*scaleX, 1/mult*scaleY, 1/mult*scaleZ);
            
            dirty = false;
        }
    }
    
    // Update these functions
    void OnEnable()
    {
        running = true;
        if (subscriberThread == null || !subscriberThread.IsAlive)
        {
            subscriberThread = new Thread(NetMqSubscriber);
            subscriberThread.Start("tcp://127.0.0.1:12346");
        }
    }

    private void OnDisable()
    {
        running = false;
        subscriberThread.Join();
    }

    private void OnDestroy()
    {
        running = false;
        subscriberThread.Join();
    }

    /// <summary>
    /// Listens for all topics on the given ZMQ endpoint
    /// 
    /// Intended to be run as in a Thread:
    ///   _subscriberThread = new Thread(NetMqSubscriber);
    ///   _subscriberThread.Start("tcp://127.0.0.1:13337");
    /// </summary>
    /// <param name="param">The ZMQ-endpoint as a string</param>
    private void NetMqSubscriber(object param)
    {
        AsyncIO.ForceDotNet.Force();
        var endpoint = (string)param;

        Debug.Log("Creating subscriber");
        var timeout = new TimeSpan(0, 0, 0, 1); //1 s

        try
        {
            subSocket = new SubscriberSocket();
            subSocket.Options.Linger = TimeSpan.Zero;
            subSocket.Subscribe("volume_extremes");
            subSocket.Connect(endpoint);
            Debug.Log("Sub connected");
            var msg = new List<string>();
            
            while (running)
            {               
                if (!subSocket.TryReceiveMultipartStrings(new TimeSpan(0, 0, 0, 0, 300), ref msg)) continue;
                Debug.Log("Got BBox message: " + msg[1]);
                bbox = JsonConvert.DeserializeObject<InvBoundingBox>(msg[1]);
                dirty = true;
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Got exception: " + ex);
        }
        finally
        {
            if (subSocket != null)
                subSocket.Close();
        }
        Debug.Log("Subscriber has the deds");
    }
}

public class InvBoundingBox
{
    public Vector3 leftLower;
    public Vector3 rightUpper;

    public override string ToString()
    {
        return "LL:" +leftLower.ToString() + "  RU:"+rightUpper.ToString();
    }
}
