using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PointCloud : MonoBehaviour
{
    ParticleSystem.Particle[] cloud;
    bool bPointsUpdated = false;
    Dictionary<string, Color> classColors;
    Dictionary<int, Point> allPoints;

    Dictionary<int, DetectionPoint> semanticPoints;
    void Start()
    {
        classColors = new Dictionary<string, Color>();
        allPoints = new Dictionary<int, Point>();
        semanticPoints = new Dictionary<int, DetectionPoint>();
        classColors["chair"] = new Color(1f, 0f, 0f);
        classColors["desk"] = new Color(0f, 1f, 0f);
        classColors["table"] = new Color(0f, 1f, 0f);
        classColors["computer monitor"] = new Color(0f, 0f, 1f);
        classColors["computer keyboard"] = new Color(0f, 1f, 1f);
        classColors["book"] = new Color(1f, 0f, 1f);
    }

    void Update()
    {
        if (bPointsUpdated)
        {
            Debug.Log("Updating Point Cloud!!");
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
            allPoints[positions[ii].Id] = positions[ii];
            if (semanticCloud.ContainsKey(positions[ii].Id))
            {
                semanticPoints[positions[ii].Id] = semanticCloud[positions[ii].Id];

                if (classColors.ContainsKey(semanticCloud[positions[ii].Id].ObjectClass))
                {
                    cloud[ii].color = classColors[semanticCloud[positions[ii].Id].ObjectClass];
                }
                else
                {
                    Color c = new Color(Random.value, Random.value, Random.value);
                    classColors[semanticCloud[positions[ii].Id].ObjectClass] = c;
                    //cloud[ii].color = c;
                    cloud[ii].color = new Color(.5f, .5f, .5f);

                }
            }
            else
            {
                //cloud[ii].color = new Color(.2f, .2f, .2f);
                cloud[ii].color = new Color((0xFF & positions[ii].Color >> 16) / 255f, (0xFF & positions[ii].Color >> 8) / 255f, (0xFF & positions[ii].Color >> 0) / 255f);
            }
            cloud[ii].size = 0.5f;
        }

        bPointsUpdated = true;
    }


    public void filterClass(string className)
    {
        cloud = new ParticleSystem.Particle[allPoints.Count];
        int idx = 0;
        Debug.Log("Filtering points to show " + className);
        Debug.Log("We have " + allPoints.Count + " points in total");
        Debug.Log("Among those " + semanticPoints.Count + " are semantically labeled");
        foreach (var p in allPoints)
        {
            cloud[idx].position = new Vector3(p.Value.X, p.Value.Y, p.Value.Z);
            cloud[idx].size = 0.5f;
            if (semanticPoints.ContainsKey(p.Value.Id) && semanticPoints[p.Value.Id].ObjectClass.Equals(className))
            {
                Debug.Log("Coloring " + className);
                cloud[idx].color = classColors[className];
            }
            else
            {

                cloud[idx].color = new Color(.2f, .2f, .2f);

            }
            idx++;
        }
        bPointsUpdated = true;
    }
}