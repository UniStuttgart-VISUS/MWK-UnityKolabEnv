using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetMQ;
using NetMQ.Sockets;

public class NetMQManager : MonoBehaviour
{
    public PublisherSocket _pubSocket;
    // Start is called before the first frame update
    private void Start()
    {
        AsyncIO.ForceDotNet.Force();
        if (_pubSocket == null)
        {
            _pubSocket = new PublisherSocket();
            _pubSocket.Options.SendHighWatermark = 1000;
            _pubSocket.Bind("tcp://127.0.0.1:12345");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnApplicationQuit()
    {
        _pubSocket.Dispose();
        NetMQConfig.Cleanup(false);
    }
}
