using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClusterPoints : MonoBehaviour
{
    // Start is called before the first frame update

    public string className;
    public bool filter;


    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(filter){
            GetComponent<PointCloud>().filterClass(className);
            filter=false;
        }
        
    }
}
