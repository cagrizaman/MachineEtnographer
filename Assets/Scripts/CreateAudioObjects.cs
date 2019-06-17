using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateAudioObjects : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static IEnumerator createSoundMarkers(AudioObject[] objects) {
        foreach(AudioObject audio in objects) {
            if (audio._id != null)
            {
                GameObject sourceObject;
                if (audio.user.Equals("Cagri"))
                {
                    sourceObject = (GameObject)Instantiate(Resources.Load("SoundMarker"), new Vector3(audio.position_x, audio.position_y, audio.position_z), Quaternion.identity);
                }
                else
                {
                    sourceObject = (GameObject)Instantiate(Resources.Load("SoundMarker_Other"), new Vector3(audio.position_x, audio.position_y, audio.position_z), Quaternion.identity);

                }
                yield return sourceObject;
                AudioSource source = sourceObject.GetComponent<AudioSource>();
                Byte[] b = Convert.FromBase64String(audio.audioObject);
                yield return b;
                source.clip = WavUtility.ToAudioClip(b);
				TextMesh mesh = sourceObject.transform.GetChild(0).GetComponent<TextMesh>();
        		mesh.transform.localPosition = Vector3.zero;
				mesh.text = audio.transcript;
        		yield return mesh;

            }
        }


    }


    public static IEnumerator createQuestionMarkers(AudioObject[] objects)
    {
        foreach (AudioObject audio in objects)
        {
            if (audio._id != null)
            {
                GameObject sourceObject;
                 sourceObject = (GameObject)Instantiate(Resources.Load("QuestionMarker"), new Vector3(audio.position_x, audio.position_y, audio.position_z), Quaternion.identity);
                yield return sourceObject;
                AudioSource source = sourceObject.GetComponent<AudioSource>();
                Byte[] b = Convert.FromBase64String(audio.audioObject);
                yield return b;
                source.clip = WavUtility.ToAudioClip(b);
                //TextMesh mesh = sourceObject.transform.GetChild(0).GetComponent<TextMesh>();
               // mesh.transform.localPosition = Vector3.zero;
               // mesh.text = audio.transcript;
               // yield return mesh;

            }
        }


    }
}
