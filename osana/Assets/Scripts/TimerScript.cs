using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TimerScript : MonoBehaviour {

	private Text text;
	public Text endText;
	private float time;
	private bool b = false;
	const int THRESHOLD = 10;
	// Use this for initialization
	void Start () {
		endText.enabled = false;
		text = this.gameObject.GetComponent<Text> ();
		time = 60f;
	}
	
	// Update is called once per frame
	void Update () {
		text.text = "Timer: " + (int) time;
		time -= 1 * Time.deltaTime;

		if (time <= THRESHOLD && !b) {
			b = true;
			StartCoroutine(showEndText ());
		}
	
	}

	IEnumerator showEndText(){
		endText.enabled = true;
		Debug.Log ("hit");

		yield return new WaitForSeconds(THRESHOLD);

		SceneManager.LoadScene ("SpeedrunLevel");
	}
}
