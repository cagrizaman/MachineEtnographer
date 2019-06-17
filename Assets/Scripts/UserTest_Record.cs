using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class UserTest_Record : MonoBehaviour {
    string myDevice;
    public string filename;
    AudioClip myClip;
    GameObject newAudio;
    public GameObject CenterEye;
    SavWav save;
    public StreamWriter sr;
    bool recording = true;

    // Use this for initialization
    void Start () {
        save = new SavWav();
        string dataFile = filename + ".txt";
        var FILE_PATH = Path.Combine(Application.dataPath, dataFile);

        sr = System.IO.File.CreateText(FILE_PATH);


        foreach (string device in Microphone.devices)
        {
            myDevice = device;
            int min;
            int max;
            Microphone.GetDeviceCaps(device, out min, out max);
            Debug.Log(min);
            Debug.Log(max);
        }

        myClip = Microphone.Start(myDevice, true, 900, 48000);

    }

    // Update is called once per frame
    void Update () {
        if (recording)
        {
            string pos = CenterEye.transform.position.ToString("G4");
            string rot = CenterEye.transform.rotation.ToString("G4");

            //THIS IS ESSENTIALLY SAVING TIME,POSITION VECTOR, ROTATION VECTOR IN CSV.
            //HOWEVER ALL PARANTHESIS (COMING FROM ROT AND POS) SHOULD BE REMOVED FROM THE FILE FOR EASIER PROCESSING WHEN LOADING BACK. 
            //WHAT I DO IS JUST OPEN TEXT EDITOR FIND "(" and ")" and replace with nothing -- "". That does the job.
            // I CAN FIND A BETTER WAY IF YOU LIKE.

            sr.WriteLine(Time.timeSinceLevelLoad + "," + pos + "," + rot);
        }
        if (Input.GetKeyDown(KeyCode.S)) {

            save.Save(filename, myClip);
            sr.Close();
            recording = false;
        }
        //if (OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.Remote)) {
        //    if (Microphone.IsRecording(myDevice))
        //    {
        //        int pos = Microphone.GetPosition(myDevice);
        //        int diff = pos - lastSample;
        //        Microphone.End(myDevice);
        //    }
        //}

    }


}
