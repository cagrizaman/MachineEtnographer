using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grpc.Core;
public class GrpcClient : MonoBehaviour
{
    Channel channel;
    GenesisSimulator.GenesisSimulatorClient client;
    public string host;
    public string port;
    public bool connected=false;
    // Start is called before the first frame update
    void Start()
    {
        channel = new Channel(host + ":" + port, ChannelCredentials.Insecure);
        client = new GenesisSimulator.GenesisSimulatorClient(channel);
    }

    // Update is called once per frame
    void Update()
    {
        if(connected){
        string user = "Cagri";
        var reply = client.GetCameraTransform(new CameraRequest { Message = user });
        Debug.Log("Greeting" + reply.X);
        }
    }
}
