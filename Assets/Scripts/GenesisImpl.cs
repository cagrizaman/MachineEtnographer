using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using Grpc.Core;
using System.Linq;


public class GenesisImpl : GenesisSimulator.GenesisSimulatorBase
{

    [SerializeField]
    readonly VisionInterface myCamReference;
    readonly object myLock = new object();

    private string name;
    private static List<Point> incomingPoints = new List<Point>();

    public GenesisImpl(VisionInterface camReference)
    {
        myCamReference = camReference;
    }
    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        string name = myCamReference.getCamPos();
        return Task.FromResult(new HelloReply { Message = name });
    }

    public override Task<CameraReply> GetCameraTransform(CameraRequest request, ServerCallContext context)
    {
        return Task.FromResult(myCamReference.getCamTransform());
    }

    public override Task<Confirmation> UpdateCamera(CameraUpdate request, ServerCallContext context)
    {
        myCamReference.updateCamera(request);
        return Task.FromResult(new Confirmation { Message = "OK" });

    }


    public override Task<Confirmation> SendSinglePoint(Point request, ServerCallContext context)
    {
        myCamReference.AddSinglePoint(request);
        return Task.FromResult(new Confirmation { Message = "OK" });

    }


    public override async Task<Confirmation> RecordPoints(IAsyncStreamReader<Point> requestStream, ServerCallContext context)
    {
        incomingPoints.Clear();
        int counter = 0;
        Debug.Log("Somebody is trying to talk to us");
        while (await requestStream.MoveNext())
        {
            Debug.Log("Next point...");
            var point = requestStream.Current;
            incomingPoints.Add(point);
            if (counter % 99 == 0)
            {
                myCamReference.updatePointCloud(incomingPoints);

            }
        }
        Debug.Log("We are done babe");
        return new Confirmation { Message = "OK" };
    }

    public override Task<Confirmation> AddPointList(PointList request, ServerCallContext context)
    {
        Debug.Log("Points received");
        Debug.Log("Point Count " + request.Points.Count);
        myCamReference.updatePointCloud(request);
        return Task.FromResult(new Confirmation { Message = "OK" });
    }

    public override Task<Confirmation> AddDetectionPointList(DetectionPointList request, ServerCallContext context)
    {
        Debug.Log("Point Classes received");
        Debug.Log("Number of points with classes " + request.Dpoints.Count);
        myCamReference.addClassesToPointCloud(request);
        return Task.FromResult(new Confirmation { Message = "OK" });

    }
    public override Task<Confirmation> AddPlaneObject(DetectionPlane request, ServerCallContext context)
    {

        myCamReference.addPlane(request);
        return Task.FromResult(new Confirmation { Message = "OK" });
    }

    public override Task<Confirmation> AddPointObject(DetectionPoint request, ServerCallContext context)
    {
        Debug.Log(request.HitId);
        myCamReference.addPoint(request);
        return Task.FromResult(new Confirmation { Message = "OK" });
    }

    public override Task<Confirmation> SaveMap(Empty request, ServerCallContext context)
    {
        Debug.Log("Saving Map...");
        myCamReference.SaveMap();
        return Task.FromResult(new Confirmation { });
    }

    public override async Task LoadMap(Empty request,
    IServerStreamWriter<Point> responseStream,
    ServerCallContext context)
    {
        Dictionary<int, Point> pointMap = myCamReference.getPointMap();

        //Send only a handful of points for testing. 
        for (var i = 0; i < 100 && i < pointMap.Count; i++)
        {
            var pcpoint = pointMap.Values.ElementAt(i);
            Point response = new Point { X = pcpoint.X, Y = pcpoint.Y, Z = pcpoint.Z };
            await responseStream.WriteAsync(response);
        }

        Debug.Log("points send, recovering the map on server...");
        myCamReference.LoadMap();
    }


}