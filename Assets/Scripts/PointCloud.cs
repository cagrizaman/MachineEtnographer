using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PointCloud : MonoBehaviour
{
    ParticleSystem.Particle[] cloud;
    bool bPointsUpdated = false;
    Dictionary<string, Color> classColors;
    public Dictionary<int, Point> allPoints;

    public Dictionary<int, DetectionPoint> semanticPoints;

    private Point[] positions;
    public Dictionary<int, int> groups;
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


    public void SetPointsWithColors(Dictionary<int, Point> pointDict)
    {
        positions = new Point[pointDict.Count];
        pointDict.Values.CopyTo(positions, 0);
        cloud = new ParticleSystem.Particle[positions.Length];

        for (int ii = 0; ii < positions.Length; ++ii)
        {
            cloud[ii].position = new Vector3(positions[ii].X, positions[ii].Y, positions[ii].Z);
            allPoints[positions[ii].Id] = positions[ii];
            cloud[ii].color = new Color((0xFF & positions[ii].Color >> 16) / 255f, (0xFF & positions[ii].Color >> 8) / 255f, (0xFF & positions[ii].Color >> 0) / 255f);
            cloud[ii].size = 0.03f;

        }

        bPointsUpdated = true;
    }



    public void DrawGroups(Dictionary<int, List<Point>> pointClusters, int numPoints)
    {
        cloud = new ParticleSystem.Particle[numPoints];
        int idx = 0;
        foreach (var keyvalue in pointClusters)
        {
            var cluster = keyvalue.Value;
            Color c = new Color(Random.value, Random.value, Random.value);
            foreach (Point p in cluster)
            {
                cloud[idx].position = new Vector3(p.X, p.Y, p.Z);
                cloud[idx].color = c;
                cloud[idx].size = 0.01f;
                idx++;
            }
        }

        bPointsUpdated = true;

    }


    public void DrawClasses(Dictionary<int, List<Point>> pointClusters, int numPoints, Dictionary<int, DetectionPoint> classes)
    {
        cloud = new ParticleSystem.Particle[numPoints];
        int idx = 0;
        foreach (var keyvalue in pointClusters)
        {
            var cluster = keyvalue.Value;
            foreach (Point p in cluster)
            {
                if (classes.ContainsKey(p.Id))
                {
                    string objectClass = classes[p.Id].ObjectClass;
                    cloud[idx].position = new Vector3(p.X, p.Y, p.Z);
                    cloud[idx].size = 0.01f;
                    if (classColors.ContainsKey(objectClass))
                    {
                        cloud[idx].color = classColors[objectClass];
                    }
                    else
                    {
                        Color c = new Color(Random.value, Random.value, Random.value);
                        classColors[objectClass] = c;
                        cloud[idx].color = c;
                    }

                    idx++;
                }
            }
        }

        bPointsUpdated = true;

        // if (semanticCloud.ContainsKey(positions[ii].Id))
        // {
        //     semanticPoints[positions[ii].Id] = semanticCloud[positions[ii].Id];

        //     if (classColors.ContainsKey(semanticCloud[positions[ii].Id].ObjectClass))
        //     {
        //         cloud[ii].color = classColors[semanticCloud[positions[ii].Id].ObjectClass];
        //     }
        //     else
        //     {
        //         Color c = new Color(Random.value, Random.value, Random.value);
        //         classColors[semanticCloud[positions[ii].Id].ObjectClass] = c;
        //         //cloud[ii].color = c;
        //         cloud[ii].color = new Color(.5f, .5f, .5f);

        //     }
        // }
        // else
        // {
        //     //cloud[ii].color = new Color(.2f, .2f, .2f);
        //     cloud[ii].color = new Color((0xFF & positions[ii].Color >> 16) / 255f, (0xFF & positions[ii].Color >> 8) / 255f, (0xFF & positions[ii].Color >> 0) / 255f);
        // }

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


    public void filterGroups(int[] groupSize)
    {
        cloud = new ParticleSystem.Particle[allPoints.Count];
        int idx = 0;
        Debug.Log("There are " + groups.Count + " different point groups");
        Color[] randomGroups = new Color[groupSize.Length];
        for (var i = 0; i < groupSize.Length; i++)
        {
            if (groupSize[i] > 100)
            {
                randomGroups[i] = new Color(Random.value, Random.value, Random.value);
            }
            else
            {
                randomGroups[i] = new Color(.2f, .2f, .2f);
            }
        }
        foreach (var p in allPoints)
        {
            cloud[idx].position = new Vector3(p.Value.X, p.Value.Y, p.Value.Z);
            cloud[idx].size = 0.5f;
            if (groups.ContainsKey(p.Value.Id))
            {
                cloud[idx].color = randomGroups[groups[p.Value.Id]];
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