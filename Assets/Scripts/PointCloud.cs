using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PointCloud : MonoBehaviour
{
    ParticleSystem.Particle[] cloud;
    bool bPointsUpdated = false;
    Dictionary<string, Color> classColors;
    void Start()
    {
        classColors = new Dictionary<string, Color>();
        classColors["chair"]= new Color(1f,0f,0f);
        classColors["desk"]= new Color(0f,1f,0f);
        classColors["table"] = new Color(0f,1f,0f);
        classColors["computer monitor"] = new Color(0f,0f,1f);
        classColors["computer keyboard"] = new Color(0f,1f,1f);
        classColors["book"] = new Color(1f,0f,1f);
    }

    void Update()
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
            cloud[ii].color = new Color((1f * ii) / positions.Length, .5f, 1 - ((1f * ii) / positions.Length));
            cloud[ii].size = 0.5f;
        }

        bPointsUpdated = true;
    }

    public void SetPointsWithColors(Point[] positions, Dictionary<int, DetectionPoint> semanticCloud)
    {
        cloud = new ParticleSystem.Particle[positions.Length];

        for (int ii = 0; ii < positions.Length; ++ii)
        {
            cloud[ii].position = new Vector3(positions[ii].X, positions[ii].Y, positions[ii].Z);

            if (semanticCloud.ContainsKey(positions[ii].Id))
            {
                if (classColors.ContainsKey(semanticCloud[positions[ii].Id].ObjectClass))
                {
                    cloud[ii].color = classColors[semanticCloud[positions[ii].Id].ObjectClass];
                }
                else
                {
                    Color c = new Color(Random.value, Random.value, Random.value);
                    classColors[semanticCloud[positions[ii].Id].ObjectClass] = c;
                    cloud[ii].color = c;
                    //cloud[ii].color = new Color(.5f,.5f,.5f);

                }
            }
            else
            {
                //cloud[ii].color = new Color(.5f,.5f,.5f);
                cloud[ii].color = new Color((0xFF & positions[ii].Color >> 16) / 255f, (0xFF & positions[ii].Color >> 8) / 255f, (0xFF & positions[ii].Color >> 0) / 255f);
            }
            cloud[ii].size = 0.5f;
        }

        bPointsUpdated = true;
    }
}