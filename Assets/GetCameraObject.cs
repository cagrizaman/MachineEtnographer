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
    Dictionary<int, Vector3> pointMap;
    private int pointCount = 0;
    private StreamWriter sr;

    string FILE_PATH;

    void Start()
    {   string dataFile = outputFile + ".xyz";
        FILE_PATH = Path.Combine(Application.dataPath, dataFile);
        camReply = new CameraReply { X = 0f, Y = 0f, Z = 0f, Q = 0f, W = 0f, R = 0f, T = 0f };
        pointMap = new Dictionary<int, Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        if (connectionEstablished)
        {
            cam.transform.position = new Vector3(camUpdate.X, camUpdate.Y, camUpdate.Z);
            cam.transform.rotation = new Quaternion(camUpdate.W, camUpdate.R, camUpdate.T, camUpdate.Q);

            if (pointCount % 10 == 0)
            {
                Vector3[] particles = new Vector3[pointMap.Count];
                pointMap.Values.CopyTo(particles, 0);
                GetComponent<PointCloud>().SetPoints(particles);
            }

        }
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

    public void updateCamera(CameraUpdate update)
    {
        connectionEstablished = true;
        camUpdate = update;

    }

    public void updatePointCloud(Point point)
    {
        pointCount++;
        pointMap[point.Id] = new Vector3(point.X, point.Y, point.Z);
    }


    public void SaveMap()
    {

        sr = System.IO.File.CreateText(FILE_PATH);
        foreach (KeyValuePair<int, Vector3> pcpoint in pointMap)
        {
            sr.WriteLine(pcpoint.Value.x +" "+pcpoint.Value.y + " " + pcpoint.Value.z);
        }
            sr.Close();
    }

    public void LoadMap(){
        using (StreamReader sr = new StreamReader(FILE_PATH))
            {
                string line;
                var counter=233000;
                // Read and display lines from the file until the end of 
                // the file is reached.
                while ((line = sr.ReadLine()) != null)
                {
                    string[] values = line.Split(' ');
                    pointMap[counter]=new Vector3(float.Parse(values[0]),float.Parse(values[1]),float.Parse(values[2]));
                    counter ++;
                }
                Vector3[] particles = new Vector3[pointMap.Count];
                pointMap.Values.CopyTo(particles, 0);
                GetComponent<PointCloud>().SetPoints(particles);
            }
    }
    public void OnApplicationQuit()
    {
        connectionEstablished = false;
    }
}
