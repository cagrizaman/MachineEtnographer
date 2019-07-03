using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DetectionObject : MonoBehaviour
{
    public int groupId;
    public string objectClass;
    public float confidence;

    public List<Point> points;

    public Bounds bbox;

    public Mesh mesh;
    public Matrix4x4 meshTransform;
    private bool initialized = false;
    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized)
        {
            return;
        }
        //Maybe merge with others..
        DetectionObject[] others = FindObjectsOfType<DetectionObject>();
        List<CombineInstance> combineList = new List<CombineInstance>();

        foreach (DetectionObject other in others)
        {
            if (bbox.Intersects(other.bbox) && other.objectClass.Equals(objectClass) && !other.Equals(this))
            {
                Debug.Log("Combining Meshes!");
                CombineInstance[] combine = new CombineInstance[2];
                combine[0].mesh = mesh;
                combine[1].mesh = other.mesh;
                //combine[0].transform=meshTransform;
                //combine[1].transform=other.meshTransform;
                //mesh=new Mesh();
                //mesh.CombineMeshes(combine, true,false);
                //bbox=mesh.bounds;
                other.transform.parent = transform;
                GetComponent<BoxCollider>().center = bbox.center;
                GetComponent<BoxCollider>().size = bbox.size;
                //Destroy(other.gameObject);
            }
        }
    }

    public void draw()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        mesh = new Mesh();
        mf.mesh = mesh;
        meshTransform = mf.transform.localToWorldMatrix;

        Vector3[] vertices = new Vector3[points.Count];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(points[i].X, points[i].Y, points[i].Z);
        }
        mesh.vertices = vertices;
        bbox = mesh.bounds;
        GetComponent<BoxCollider>().center = bbox.center;
        GetComponent<BoxCollider>().size = bbox.size;
        Vector3 center = mesh.bounds.center;
        Vector3 extent = mesh.bounds.extents;
        if (extent.magnitude < 0.1f)
        {
            Destroy(gameObject);
            return;
        }
        initialized = true;
        Debug.Log("A" + objectClass + " is at " + center + " with extents " + extent);


    }
}

