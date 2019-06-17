using UnityEngine;
using System.Threading.Tasks;
using Grpc.Core;


public class GenesisImpl:GenesisSimulator.GenesisSimulatorBase{

    [SerializeField]
    readonly  GetCameraObject myCamReference;
    readonly object myLock = new object();

    private string name;
    
    public GenesisImpl( GetCameraObject camReference){
      myCamReference=camReference;
    }
    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context){
        string name = myCamReference.getCamPos();
        return Task.FromResult(new HelloReply{Message=name});
    }

    public override Task<CameraReply> GetCameraTransform(CameraRequest request, ServerCallContext context){
         return Task.FromResult(myCamReference.getCamTransform());
    }

    public override Task<Confirmation> UpdateCamera(CameraUpdate request,ServerCallContext context){
        myCamReference.updateCamera(request);
        return Task.FromResult(new Confirmation{Message="OK"});

    }

    public override async Task<Confirmation> RecordPoints(IAsyncStreamReader<Point> requestStream, ServerCallContext context){
        while(await requestStream.MoveNext(context.CancellationToken)){
            var point = requestStream.Current;
            myCamReference.updatePointCloud(point);
        }
        return new Confirmation{Message="OK"};
    }

    public override Task<Confirmation> SaveMap(Empty request, ServerCallContext context){
        Debug.Log("Saving Map...");
        myCamReference.SaveMap();
        return Task.FromResult(new Confirmation{});
    }

   
}