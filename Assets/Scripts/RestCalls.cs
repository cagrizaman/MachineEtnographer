using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class RestCalls : MonoBehaviour {
    public string url = "192.168.0.35:3000/getAudio";
    public string url2;
    public string user = "Cagri";
     IEnumerator Start() {
        WWW res = new WWW(url);
        yield return res;
        string response = res.text;
        AudioObject[] objects = JsonHelper.getJsonArray<AudioObject>(response);
        yield return CreateAudioObjects.createSoundMarkers(objects);

        WWW res2 = new WWW(url2);
        yield return res2;
        string response2 = res2.text;
        AudioObject[] questions = JsonHelper.getJsonArray<AudioObject>(response2);
        yield return CreateAudioObjects.createQuestionMarkers(questions);
    }

	
	// Update is called once per frame
	void Update () {
		
	}
}
