using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {
	public AudioSource efxSource;
	public AudioSource musicSource;
	public static SoundManager ins = null;

	void Awake (){
		ins = this;
	}


	//Used to play single sound clips.
	public void PlaySingle(AudioClip clip) {
		if (clip.name.Contains ("Walking") && efxSource.isPlaying)
			return;

		if (clip.name.Contains ("Jump")) {
			efxSource.pitch = 1.6f;
			efxSource.volume = 1.0f;
		} else {
			efxSource.pitch = 1.0f;
			efxSource.volume = 0.25f;
		}
				
		//Play the clip
		efxSource.clip = clip;
		efxSource.Play ();
	}
}