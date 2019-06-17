/*MIT License

Copyright(c) 2018 Vili Volčini / viliwonka

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataStructures.ViliWonka.KDTree;


    public class KDTree_Vertices : MonoBehaviour {


        public int K = 13;

        [Range(0f, 100f)]
        public float Radius = 3f;

        public bool DrawQueryNodes = true;

        public Vector3 IntervalSize = new Vector3(0.2f, 0.2f, 0.2f);
        private Mesh mesh;
        private bool treeReady=false;
        Vector3[] pointCloud;
        KDTree tree;

        KDQuery query;
        MeshSmoother smoother;
        
       void Awake(){
                mesh=GetComponent<MeshFilter>().mesh;
                Debug.Log(mesh.vertices.Length);
                Debug.Log(transform);
                smoother= transform.GetComponent<MeshSmoother>();
                if(smoother!=null){
                   mesh=smoother.Smooth();
                }

                pointCloud=mesh.vertices;
                Color[] initcolors = new Color[pointCloud.Length];

                for(int i=0;i<pointCloud.Length;i++){
                    pointCloud[i]+=new Vector3(Random.value * 0.0025f,Random.value * 0.0025f,Random.value * 0.0025f);
                    initcolors[i]=Color.black;
                }
                mesh.colors=initcolors;
                tree = new KDTree(pointCloud, 16);
                query = new KDQuery();
       } 
        
        void Start() {
            
       
        }


        void Update() {

        }


        public void updateMeshColor(Vector3 hitpoint, float distance){
            if(query==null || tree==null){
                return;
            }
            var resultIndices = new List<int>();
            Radius = Mathf.Tan(Mathf.PI*5/180)*distance;
            query.Radius(tree, hitpoint, Radius, resultIndices);
            Color[] colors = new Color[mesh.vertices.Length];
            colors=mesh.colors;
            for(int i = 0; i < resultIndices.Count; i++) {
                // float dist=Vector3.Distance(mesh.vertices[resultIndices[i]],hitpoint);
                // colors[resultIndices[i]]= colors[resultIndices[i]] *0.95f + 0.05f* Color.Lerp(Color.red, colors[resultIndices[i]],dist);
                
                colors[resultIndices[i]]+=Color.red / 255f;
            }
            mesh.colors=colors;
    
        } 
        private void OnDrawGizmos() {

        }
    }
