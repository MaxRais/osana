using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TendonMusicChange : MonoBehaviour {

	public AudioClip muscle;
	public AudioClip bone;
	public AudioSource player;
	public float fadeTime = 5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D col) {
		StopAllCoroutines ();
		if (col.name == "Player") {
			StartCoroutine (fade (true));
		}
	}

	void OnTriggerExit2D(Collider2D col) {
		StopAllCoroutines ();
		if (col.name == "Player") {
			if (col.transform.position.x < this.transform.position.x) {
				player.clip = muscle;
			} else {
				player.clip = bone;
			}

			StartCoroutine (fade (false));
		}
	}

	IEnumerator fade (bool fadeOut) {
		float startVolume = player.volume;

		if (fadeOut) {
			while (player.volume > 0) {
				player.volume -= startVolume * Time.deltaTime / fadeTime;
				yield return null;
			}
			player.Stop ();
		} else {
			player.Play ();
			while (player.volume < 1) {
				player.volume += Time.deltaTime * fadeTime;
				yield return null;
			}
		}


	}
}
