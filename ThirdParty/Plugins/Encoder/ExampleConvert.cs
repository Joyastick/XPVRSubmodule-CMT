using UnityEngine;

public class ExampleConvert : MonoBehaviour {
	public AudioClip clip;
	// Use this for initialization
	void Start () {
		

		EncodeMP3.Convert (clip, Application.dataPath + "/convertedMp3.mp3", 128);

	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
