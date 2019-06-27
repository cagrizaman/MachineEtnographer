using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class GetCameraObject : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject cam;

    public string outputFile;
    private string name = "";
    private Transform camtransform;

    private CameraReply camReply;
    private CameraUpdate camUpdate;
    private bool connectionEstablished = false;

    private bool incomingPlane = false;
    private bool incomingPointObject = false;

    Dictionary<int, Point> pointMap;
    List<Point> newPoints;

    Dictionary<int, Vector3> recoveredPointMap;

    public GameObject detectionPrefab;
    private int pointCount = 0;
    private StreamWriter sr;

    private bool isIncomingPoints = false;

    public PointCloud recoveredMap;

    public static Point[] particles;
    string FILE_PATH;
    private readonly object myLock = new object();
    private Dictionary<int,DetectionPoint> semanticCloud;
    private Dictionary<int, GameObject> objectCache;
    private DetectionPlane myPlane;
    private DetectionPoint myPoint;


    void Start()
    {
        string dataFile = outputFile + ".xyz";
        FILE_PATH = Path.Combine(Application.dataPath, dataFile);
        camReply = new CameraReply { X = 0f, Y = 0f, Z = 0f, Q = 0f, W = 0f, R = 0f, T = 0f };
        pointMap = new Dictionary<int, Point>();
        recoveredPointMap = new Dictionary<int, Vector3>();
        newPoints = new List<Point>();
        objectCache = new Dictionary<int, GameObject>();
        semanticCloud= new Dictionary<int,DetectionPoint>();
    }

    // Update is called once per frame
    void Update()
    {
        if (connectionEstablished)
        {
            cam.transform.position = new Vector3(camUpdate.X, camUpdate.Y, camUpdate.Z);
            cam.transform.rotation = new Quaternion(camUpdate.W, camUpdate.R, camUpdate.T, camUpdate.Q);

            particles = new Point[pointMap.Count];
            pointMap.Values.CopyTo(particles, 0);
            GetComponent<PointCloud>().SetPointsWithColors(particles, semanticCloud);
            isIncomingPoints = false;

        }

        // if (incomingPlane)
        // {
        //     if (myPoint.ObjectClass.Equals("desk") || myPoint.ObjectClass.Equals("table"))
        //     {
        //         GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        //         plane.transform.rotation = new Quaternion(myPlane.Rot.X, myPlane.Rot.Y, myPlane.Rot.Z, myPlane.Rot.W);
        //         plane.transform.position = new Vector3(myPlane.Pos.X, myPlane.Pos.Y, myPlane.Pos.Z);
        //         plane.transform.localScale = new Vector3(myPlane.Ex.X / 10, .1f, myPlane.Ex.Y / 10);
        //     }
        //     else
        //     {
        //         if (myPlane.HitId > -1 && objectCache.ContainsKey((int)myPlane.HitId))
        //         {
        //             Destroy(objectCache[(int)myPlane.HitId]);
        //             Debug.Log("Updating an already existing detection point");
        //         }
        //         GameObject cube = Instantiate(detectionPrefab);
        //         cube.GetComponent<DetectionObject>().objectClass = myPlane.ObjectClass;
        //         cube.GetComponent<DetectionObject>().confidence = myPlane.Confidence;
        //         cube.transform.position = new Vector3(myPlane.HitPos.X, myPlane.HitPos.Y, myPlane.HitPos.Z);
        //         cube.transform.LookAt(cam.transform);
        //         cube.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);

        //         objectCache[(int)myPlane.HitId] = cube;
        //     }


        //     incomingPlane = false;
        // }

        // if (incomingPointObject)
        // {

        //     if (myPoint.HitId > -1 && objectCache.ContainsKey((int)myPoint.HitId))
        //     {
        //         Destroy(objectCache[(int)myPoint.HitId]);
        //         Debug.Log("Updating an already existing detection point");
        //     }
        //     GameObject cube = Instantiate(detectionPrefab);
        //     cube.GetComponent<DetectionObject>().objectClass = myPoint.ObjectClass;
        //     cube.GetComponent<DetectionObject>().confidence = myPoint.Confidence;
        //     cube.transform.position = new Vector3(myPoint.Pos.X, myPoint.Pos.Y, myPoint.Pos.Z);
        //     cube.transform.LookAt(cam.transform);
        //     cube.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);

        //     objectCache[(int)myPoint.HitId] = cube;


        //     incomingPointObject = false;
        // }
        name = cam.transform.position.ToString();
    }

    public string getCamPos()
    {
        return name;
    }

    public CameraReply getCamTransform()
    {
        return camReply;

    }

    public Dictionary<int, Point> getPointMap()
    {
        return pointMap;
    }

    public void updateCamera(CameraUpdate update)
    {
        connectionEstablished = true;
        camUpdate = update;

    }

    public void addPlane(DetectionPlane plane)
    {
        if (!incomingPlane)
        {
            incomingPlane = true;
            myPlane = plane;
        }
    }

    public void addPoint(DetectionPoint point)
    {
        
        semanticCloud[(int)point.HitId]= point;

    }
    public void AddSinglePoint(Point p)
    {
        Debug.Log("Received New point with id " + p.Id);
        pointMap[p.Id] = p;

    }
    public void updatePointCloud(List<Point> newPoints)
    {
        Debug.Log("Receiving something");

        foreach (Point p in newPoints)
        {
            pointMap[p.Id] = p;
        }
        isIncomingPoints = true;


    }

        public void updatePointCloud(PointList newPoints)
    {
        Debug.Log("Receiving something");

        foreach (Point p in newPoints.Points)
        {
            pointMap[p.Id] = p;
        }
        isIncomingPoints = true;


    }


    public void SaveMap()
    {

        sr = System.IO.File.CreateText(FILE_PATH);
        foreach (KeyValuePair<int, Point> pcpoint in pointMap)
        {
            sr.WriteLine(pcpoint.Value.X + " " + pcpoint.Value.Y + " " + pcpoint.Value.Z);
        }
        sr.Close();
    }

    public void LoadMap()
    {
        using (StreamReader sr = new StreamReader(FILE_PATH))
        {
            string line;
            var counter = 0;
            // Read and display lines from the file until the end of
            // the file is reached.
            while ((line = sr.ReadLine()) != null)
            {
                string[] values = line.Split(' ');
                recoveredPointMap[counter] = new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
                counter++;
            }
            Vector3[] particles = new Vector3[recoveredPointMap.Count];
            recoveredPointMap.Values.CopyTo(particles, 0);
            recoveredMap.SetPoints(particles);
            Debug.Log(particles.Length);
            Debug.Log("Map recovered.");
        }
    }
    public void OnApplicationQuit()
    {
        connectionEstablished = false;
    }
}
