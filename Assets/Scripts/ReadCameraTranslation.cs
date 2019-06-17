using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;
using System;

public class ReadCameraTranslation : MonoBehaviour {

    // Use this for initialization

    public ArrayList data = new ArrayList();
    public string subjectName;
    private bool dataAvailalbe=false;
    private int counter=0;
    private Camera camera ;

    private string filename;
	void Start () {
        
        camera= GetComponent<Camera>();
        filename="Assets/Data/"+subjectName+".txt";
        try
        {
            // Create an instance of StreamReader to read from a file.
            // The using statement also closes the StreamReader.
            using (StreamReader sr = new StreamReader(filename))
            {
                string line;
                // Read and display lines from the file until the end of 
                // the file is reached.
                while ((line = sr.ReadLine()) != null)
                {
                    data.Add(line);
                    
                }
            }
        }
        catch (Exception e)
        {
            // Let the user know what went wrong.
            print("The file could not be read:");
            print(e.Message);
        }
       print("DONE!");
    }
	
	// Update is called once per frame
	void Update () {
	
        if (data.Count > 0) { 
        dataAvailalbe=true;
        String dataAtIndex=data[0].ToString();
        // Debug.Log(dataAtIndex);
        dataAtIndex=dataAtIndex.Replace("(","").Replace(")","");        
            String[] values = dataAtIndex.Split(',');
        // Debug.Log(values[0]);
        // Debug.Log(values[1]);
        // Debug.Log(values[2]);
        // Debug.Log(values[3]);
        float time = float.Parse(values[0]);
        float posx = float.Parse(values[1]);
        float posy = float.Parse(values[2]);
        float posz = float.Parse(values[3]);


        float rot1 = float.Parse(values[4]);
        float rot2 = float.Parse(values[5]);
        float rot3 = float.Parse(values[6]);
        float rot4 = float.Parse(values[7]);

        transform.position=new Vector3(posx, posy, posz);
         transform.rotation = new Quaternion(rot1, rot2, rot3, rot4);
        data.RemoveAt(0);
        counter++;

        }
        else{
            dataAvailalbe=false;
            
        }

    }

     public int resWidth = 300; 
     public int resHeight = 300;
 
     public bool takeHiResShot = false;
 
     public static string ScreenShotName(int width, int height, int count, string subject) {
         return string.Format("{0}/{4}_{1}x{2}_{3}.png", 
                              "/media/zaman/Data/Mediate/MediateVR/Outputs/1", 
                              width, height, count,subject);
     }
 
     public void TakeHiResShot() {
         takeHiResShot = true;
     }
 
     void LateUpdate() {
    if(counter%10==0 && takeHiResShot){
             RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
             camera.targetTexture = rt;
             Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
             camera.Render();
             RenderTexture.active = rt;
             screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
             camera.targetTexture = null;
             RenderTexture.active = null; // JC: added to avoid errors
             Destroy(rt);
             byte[] bytes = screenShot.EncodeToPNG();
             string filename = ScreenShotName(resWidth, resHeight,counter, subjectName);
             System.IO.File.WriteAllBytes(filename, bytes);
            //  Debug.Log(string.Format("Took screenshot to: {0}", filename));
             takeHiResShot = false;
    }
         
     }
}
