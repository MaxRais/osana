using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour {


	private bool paused = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Start")) {
			paused = !paused;
			if (paused) {
				Time.timeScale = 0;
				this.transform.Find("PauseMenu").gameObject.SetActive (true);
			} else {
				Time.timeScale = 1;
				this.transform.Find("PauseMenu").gameObject.SetActive (false);
			}
		}	
	}

	public void Resume() {
		Time.timeScale = 1;
		this.transform.Find("PauseMenu").gameObject.SetActive (false);
	}

	public void Quit() {
		SceneManager.LoadScene ("Menu");
	}

}
