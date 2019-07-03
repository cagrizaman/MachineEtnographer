using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Linq;
public class VisionInterface : MonoBehaviour
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

    // List<Point> newPoints;

    Dictionary<int, Vector3> recoveredPointMap;

    public GameObject detectionPrefab;
    private int pointCount = 0;
    private StreamWriter sr;

    private bool isIncomingPoints = false;

    public PointCloud recoveredMap;

    public static Point[] particles;
    string FILE_PATH;
    private readonly object myLock = new object();

    Dictionary<int, Point> pointMap;

    Dictionary<int, Point> pointCache;
    Dictionary<int, DetectionPoint> semanticCache;

    private Dictionary<int, DetectionPoint> semanticCloud;
    private Dictionary<int, GameObject> objectCache;
    private DetectionPlane myPlane;
    private DetectionPoint myPoint;

    private string[] exclusion_list = new string[] { "office building", "building", "house" };
    public delegate void OnDataAvailableCallbackFunc(Dictionary<int, Point> pCloud, Dictionary<int, DetectionPoint> sCloud);
    /// <summary>
    /// Callback function handle for receiving the available data.
    /// </summary>
    public event OnDataAvailableCallbackFunc OnDataAvailableCallback = null;

    void Start()
    {
        string dataFile = outputFile + ".xyz";
        FILE_PATH = Path.Combine(Application.dataPath, dataFile);
        camReply = new CameraReply { X = 0f, Y = 0f, Z = 0f, Q = 0f, W = 0f, R = 0f, T = 0f };
        pointMap = new Dictionary<int, Point>();
        recoveredPointMap = new Dictionary<int, Vector3>();
        objectCache = new Dictionary<int, GameObject>();
        semanticCloud = new Dictionary<int, DetectionPoint>();

        pointCache = new Dictionary<int, Point>();
        semanticCache = new Dictionary<int, DetectionPoint>();

    }

    // Update is called once per frame
    void Update()
    {
        if (connectionEstablished)
        {
            cam.transform.position = new Vector3(camUpdate.X, camUpdate.Y, camUpdate.Z);
            cam.transform.rotation = new Quaternion(camUpdate.W, camUpdate.R, camUpdate.T, camUpdate.Q);

            if (isIncomingPoints && OnDataAvailableCallback != null)
            {
                //OnDataAvailableCallback(pointMap, semanticCloud);
                OnDataAvailableCallback(pointCache,semanticCache);
                pointCache.Clear();
                semanticCache.Clear();
    
            }
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

        semanticCloud[(int)point.HitId] = point;

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
            pointCache[p.Id] = p;
        }
        //isIncomingPoints = true;


    }

    public void addClassesToPointCloud(DetectionPointList newPoints)
    {
        Debug.Log("Receiving something");


        foreach (DetectionPoint p in newPoints.Dpoints)
        {
            if (!exclusion_list.Contains(p.ObjectClass))
                semanticCache[(int)p.HitId] = p;

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
