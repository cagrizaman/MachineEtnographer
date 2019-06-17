using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastPaint : MonoBehaviour
{
    // Start is called before the first frame update

    Camera camera;
    List<Mesh> objects;

    public bool paintLocation;
    void Start()
    {
        camera=GetComponent<Camera>();
        objects=new List<Mesh>();

        // create new colors array where the colors will be created.

    }

    // Update is called once per frame
    void Update()
    {
        Ray raydirection;
        if(paintLocation)
                 raydirection = new Ray(camera.transform.position, Vector3.down);
        else
                 raydirection = new Ray(camera.transform.position, camera.transform.forward);
        int vertexIndex= ClosestIndexToPoint(raydirection);

    }

    public int ClosestIndexToPoint(Ray ray)
{
     RaycastHit hit;
     if(Physics.Raycast(ray, out hit, 500))
     {

         Mesh m = hit.transform.GetComponent<MeshFilter>().mesh;
        //  if(!objects.Contains(m)){
        //      objects.Add(m);
        //      Material newMat = Resources.Load("Handle1Mat", typeof(Material)) as Material;
        //      Material[] materials=hit.transform.GetComponent<Renderer>().materials;
        //      for(int i=0;i<materials.Length;i++)
        //      {
        //        materials[i]= newMat;  
        //      } ;
        //      hit.transform.GetComponent<Renderer>().materials = materials;
        //     //  Vector3[] vertices = m.vertices;
        //     //  Color[] initcolors = new Color[vertices.Length];
        //     //  for (int i = 0; i < vertices.Length; i++)
        //     //     initcolors[i] = Color.green;
        //     //  m.colors = initcolors;
        //  }


        // Mesh mesh =hit.transform.GetComponent<MeshFilter>().mesh;
         int[] tri = new int[3] {
             m.triangles[hit.triangleIndex * 3 + 0],
             m.triangles[hit.triangleIndex * 3 + 1],
             m.triangles[hit.triangleIndex * 3 + 2]    
         };
   
         // loop through hit triangle and see which vertex is closest to the hit point
         float closestDistance = Vector3.Distance(m.vertices[tri[0]], hit.point);
         int closestVertexIndex = tri[0];
        

        // Color[] colors = new Color[m.vertices.Length];
        // colors=m.colors;

         for(int i = 0; i < tri.Length; i++)
         {
             float dist = Vector3.Distance(m.vertices[tri[i]], hit.point);
             if(dist < closestDistance)
             {
                  closestDistance = dist;
                  closestVertexIndex = tri[i];
             }
         }

        KDTree_Vertices paintTarget=hit.transform.GetComponent<KDTree_Vertices>();
        if(paintTarget!=null){
            paintTarget.updateMeshColor(m.vertices[closestVertexIndex], hit.distance);
        }
        //  for (int i=Mathf.Max(0,closestVertexIndex-400);i<Mathf.Min(m.vertices.Length,closestVertexIndex+400);i++){
        //      float dist= Vector3.Distance(m.vertices[i],m.vertices[closestVertexIndex]);
        //      if(dist<4){
        //          Debug.Log("painting");
                
        //         colors[i] = (colors[i] * 0.8f) + (Color.Lerp(Color.red,m.colors[i],dist) *0.2f);
        //      }
        //  }
        
        // // Debug.Log(closestVertexIndex);
        // // colors[closestVertexIndex]=Color.red;
        // m.colors=colors;
         // returns the index of the closest vertex to hit point.
         return 0;
     }
     else
          return -1;
}
}
