using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;
using Grpc.Core;

public class GrpcServer : MonoBehaviour
{
    const int Port=50051;
    Server server;

    private bool connected=false;

    public VisionInterface camReference;
    // Start is called before the first frame update
    void Start()
    {
        server=new Server{
            Services={GenesisSimulator.BindService(new GenesisImpl(camReference))},
            Ports={new ServerPort(GetLocalIPAddress(),Port,ServerCredentials.Insecure)}
        };

        server.Start();
        connected=true;

    }

    public void Trigger(Text text){
        if(connected){
            Disconnect(text);
        }
        else{
            Connect(text);
        }
    }
    public void Connect(Text text){
        server.Start();
        Debug.Log("Greeter server listening on port " + Port);
        Debug.Log("Your IP is "+ GetLocalIPAddress());
        text.text="Disconnect";
    }

    public void Disconnect(Text text){
        Debug.Log("Disconnecting");
        server.ShutdownAsync().Wait();
        text.text="Connect";
    }

    public static string GetLocalIPAddress(){
        var host=Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList){
            if(ip.AddressFamily==AddressFamily.InterNetwork){
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }
    // Update is called once per frame
    void Update()
    {
        
    }

        void OnApplicationQuit()
    {        connected=false;
             server.ShutdownAsync().Wait();

    }
}
