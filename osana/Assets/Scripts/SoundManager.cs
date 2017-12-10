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
		if (clip.name.Contains ("Walking") && efxSource.isPlaying && efxSource.clip.name == clip.name)
			return;
				
		//Play the clip
		efxSource.clip = clip;
		efxSource.Play ();
	}
}