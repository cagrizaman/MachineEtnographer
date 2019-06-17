using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyMaterial : MonoBehaviour
{
    // Start is called before the first frame update
    public Material ovverrideMaterial;
    void Start()
    {
             Material[] materials=transform.GetComponent<Renderer>().materials;
             for(int i=0;i<materials.Length;i++)
             {
               materials[i]= ovverrideMaterial;  
             } ;
             transform.GetComponent<Renderer>().materials = materials;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
