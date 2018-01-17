using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformAudio : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void playWalkSound() {
		if (this.GetComponent<AudioSource> ().isPlaying) {
			return;
		}
		this.GetComponent<AudioSource> ().Play ();
	}

	public void stopSound() {
		this.GetComponent<AudioSource> ().Stop ();
	}
}
