using UnityEngine;
 using System.Collections;
 public class PointCloud : MonoBehaviour
 {
     ParticleSystem.Particle[] cloud;
     bool bPointsUpdated = false;
     
     void Start ()
     {
     }
     
     void Update () 
     {
         if (bPointsUpdated)
         {
             GetComponent<ParticleSystem>().SetParticles(cloud, cloud.Length);
             bPointsUpdated = false;
         }
     }
     
     public void SetPoints(Vector3[] positions)
     {        
         cloud = new ParticleSystem.Particle[positions.Length];
         
         for (int ii = 0; ii < positions.Length; ++ii)
         {
             cloud[ii].position = positions[ii];            
             cloud[ii].color = Color.blue;
             cloud[ii].size = 0.5f;            
         }
 
         bPointsUpdated = true;
     }
 }