using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DataStructures.ViliWonka.KDTree;
public class ClusterPoints : MonoBehaviour
{


    //Public parameters for inquiry
    public string className;
    public bool filter;



    //Id's and Points distangled for KDTree. We can change KDTree to accept Dictinory.
    int[] pointIds;
    Vector3[] points;
    KDTree tree;
    KDQuery query;

    //Raw Values.
    Dictionary<int, Point> pointCloud;
    Dictionary<int, DetectionPoint> semanticCloud;


    // Result of class augmentation and clustering.
    Dictionary<int, DetectionPoint> classesDict;
    Dictionary<int, int> groups;
    Dictionary<int, List<Point>> pointClusters;
    Dictionary<int, Dictionary<string, List<Point>>> instanceMap;
    int numPointsInClusters = 0;
    //Clusterin Parameters.
    float radius = .1f;
    private static int groupId = 0;

    private GameObject detectionPrefab;

    class pointCluster
    {
        Vector3 center;
        Vector3 extent;
    }
    void Start()
    {
        VisionInterface dataSource = GetComponent<VisionInterface>();
        dataSource.OnDataAvailableCallback += new VisionInterface.OnDataAvailableCallbackFunc(OnDataAvailable);
        groups = new Dictionary<int, int>();
        pointCloud = new Dictionary<int, Point>();
        semanticCloud = new Dictionary<int, DetectionPoint>();

        classesDict = new Dictionary<int, DetectionPoint>();
        pointClusters = new Dictionary<int, List<Point>>();
        instanceMap = new Dictionary<int, Dictionary<string, List<Point>>>();

        detectionPrefab = GetComponent<VisionInterface>().detectionPrefab;
    }



    void OnDataAvailable(Dictionary<int, Point> pCloud, Dictionary<int, DetectionPoint> sCloud)
    {
        pointCloud = pCloud;
        semanticCloud = sCloud;
        //Draw the initial pointcloud with colors. 
        GetComponent<PointCloud>().SetPointsWithColors(pCloud);
        //Try to classify unclassified points based on neighbors and assign each point to an intial group.
        classifyAndGroupPoints(pCloud, sCloud);
        //Remove noise (groups with <100 points in them) and create a map of clusters <groupId, List<Point>>
        cleanGroups();

        groupsToDetectionObjects();
        //Draw Groups.
        GetComponent<PointCloud>().DrawGroups(pointClusters, numPointsInClusters);
        GetComponent<PointCloud>().DrawClasses(pointClusters, numPointsInClusters, classesDict);

    }
    // Update is called once per frame
    void Update()
    {
        // if (filter)
        // {
        //     GetComponent<PointCloud>().filterClass(className);
        //     filter = false;
        // }

        // if (cluster)
        // {
        //     classifyPoints(GetComponent<PointCloud>().allPoints, GetComponent<PointCloud>().semanticPoints);
        //     cluster = false;
        // }

    }

    void initKDTree(Dictionary<int, Point> pointDict)
    {
        pointIds = new int[pointDict.Count];
        points = new Vector3[pointDict.Count];
        int idx = 0;
        foreach (var p in pointDict)
        {
            pointIds[idx] = p.Key;
            points[idx] = new Vector3(p.Value.X, p.Value.Y, p.Value.Z);
            idx++;
        }
        tree = new KDTree(points, 16);
        query = new KDQuery();


    }

    void updateKDTree(Dictionary<int, Point> pointDict)
    {

        pointIds = new int[pointDict.Count];
        points = new Vector3[pointDict.Count];
        int idx = 0;
        foreach (var p in pointDict)
        {
            pointIds[idx] = p.Key;
            points[idx] = new Vector3(p.Value.X, p.Value.Y, p.Value.Z);
            idx++;
        }
        tree.Build(points);
    }

    public void classifyAndGroupPoints(Dictionary<int, Point> pointDict, Dictionary<int, DetectionPoint> classes)
    {
        classesDict.Clear();
        groups.Clear();

        if (tree == null || query == null)
        {
            initKDTree(pointDict);
        }

        else
        {
            updateKDTree(pointDict);
        }

        List<int> results = new List<int>();
        Dictionary<string, int> neighborClasses = new Dictionary<string, int>();
        Dictionary<int, string> assignedClasses = new Dictionary<int, string>();
        foreach (var p in pointDict)
        {
            query.Radius(tree, new Vector3(p.Value.X, p.Value.Y, p.Value.Z), radius, results);
            int maxValue = 0;
            string newClass = "";
            int groupidx;
            if (groups.ContainsKey(p.Value.Id))
            {
                groupidx = groups[p.Value.Id];
            }
            else
            {
                groupidx = groupId++;
                groups[p.Value.Id] = groupidx;
            }
            for (var i = 0; i < results.Count; i++)
            {
                var pId = pointIds[results[i]];
                groups[pId] = groupidx;

                if (classes.ContainsKey(pId))
                {
                    var nClass = classes[pId].ObjectClass;
                    if (neighborClasses.ContainsKey(nClass))
                    {
                        neighborClasses[nClass] += 1;
                    }
                    else
                    {
                        neighborClasses[nClass] = 1;
                    }
                    if (neighborClasses[nClass] > maxValue)
                    {
                        maxValue = neighborClasses[nClass];
                        newClass = nClass;
                    }
                }

            }
            if (maxValue > 0)
            {
                assignedClasses[p.Value.Id] = newClass;
            }
            maxValue = 0;
            newClass = "";
            results.Clear();
            neighborClasses.Clear();

        }
        foreach (var c in assignedClasses)
        {
            classes[c.Key] = new DetectionPoint { ObjectClass = c.Value };

        }
        classesDict = new Dictionary<int, DetectionPoint>(classes);
        //cleanGroups();
    }

    void cleanGroups()
    {
        numPointsInClusters = 0;
        pointClusters.Clear();

        int[] groupSize = new int[groupId + 1];
        foreach (var g in groups)
        {
            groupSize[g.Value] += 1;
        }

        foreach (var g in groups)
        {
            if (groupSize[g.Value] > 100)
            {
                if (pointClusters.ContainsKey(g.Value))
                {
                    pointClusters[g.Value].Add(pointCloud[g.Key]);
                    numPointsInClusters++;
                }
                else
                {
                    pointClusters[g.Value] = new List<Point>();
                    pointClusters[g.Value].Add(pointCloud[g.Key]);
                    numPointsInClusters++;
                }
            }
        }

    }

    void groupsToDetectionObjects()
    {

        foreach (var clusters in pointClusters)
        {
            var cluster = clusters.Value;
            instanceMap[clusters.Key] = new Dictionary<string, List<Point>>();
            var groupObjectDictionary = instanceMap[clusters.Key];
            foreach (Point p in cluster)
            {
                if (classesDict.ContainsKey(p.Id))
                {
                    if (groupObjectDictionary.ContainsKey(classesDict[p.Id].ObjectClass))
                    {
                        groupObjectDictionary[classesDict[p.Id].ObjectClass].Add(p);
                    }
                    else
                    {
                        groupObjectDictionary[classesDict[p.Id].ObjectClass] = new List<Point>();
                        groupObjectDictionary[classesDict[p.Id].ObjectClass].Add(p);

                    }
                }

            }

            foreach (var classVertices in groupObjectDictionary)
            {
                if (classVertices.Value.Count > 20)
                {
                    DetectionObject obj = Instantiate(detectionPrefab).GetComponent<DetectionObject>();
                    obj.objectClass = classVertices.Key;
                    obj.points = classVertices.Value;
                    obj.groupId = clusters.Key;
                    obj.draw();
                }
            }

        }


    }

}
