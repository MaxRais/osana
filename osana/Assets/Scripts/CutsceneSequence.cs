using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CutsceneSequence : MonoBehaviour {

	public Image logo;
	private Text[] messages;
	public float displayTime = 1f;
	public float fadeTime = .0001f;
	private bool fading = false;
	public bool showLogoLast;

	// Use this for initialization
	void Start () {
		messages = this.GetComponentsInChildren<Text> ();
		foreach (Text t in messages) {
			t.color = new Color(1,1,1,0);
		}
		StartCoroutine (playCutscene ());
	}

	private IEnumerator playCutscene() {
		yield return new WaitForSeconds (1.0f);
		if (!showLogoLast) {
			fading = true;
			StartCoroutine (fadeLogoOut (logo));
			while (fading) {
				yield return null;
			}
		}

		foreach(Text t in messages) {
			fading = true;
			StartCoroutine (fadeMessage (t));
			while (fading) {
				yield return null;
			}
		}

		if (showLogoLast) {
			fading = true;
			StartCoroutine (fadeLogoIn (logo));
			while (fading) {
				yield return null;
			}
		} else {
			SceneManager.LoadScene ("Level1");
		}
	}

	private IEnumerator fadeMessage(Text message) {
		while (message.color.a < 1.0f) {
			setTextAlpha (message, message.color.a + Time.deltaTime);
			yield return null;
		}
		yield return new WaitForSeconds (displayTime);
		while (message.color.a > 0.0f)
		{
			setTextAlpha(message, message.color.a - Time.deltaTime);
			yield return null;
		}
		fading = false;
	}

	private IEnumerator fadeLogoOut(Image image) {
		while (image.color.a > 0.0f)
		{
			setImageAlpha(image, image.color.a - Time.deltaTime);
			yield return null;
		}
		fading = false;
	}

	private IEnumerator fadeLogoIn(Image image) {
		while (image.color.a < 1.0f) {
			setImageAlpha (image, image.color.a + Time.deltaTime);
			yield return null;
		}
		fading = false;
	}

	private void setTextAlpha(Text message, float alpha) {
		alpha = Mathf.Clamp01 (alpha);	
		message.color = new Color (1, 1, 1, alpha);
	}

	private void setImageAlpha(Image message, float alpha) {
		alpha = Mathf.Clamp01 (alpha);	
		message.color = new Color (1, 1, 1, alpha);
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			SceneManager.LoadScene ("Level1");
		}
	}
}
