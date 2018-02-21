using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinMarker : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter2D(Collision2D c) {
		if (c.gameObject.name == "Player") {
			if (GameObject.Find ("GameManager").GetComponent<GameManager> ().IsFinished ())
				SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex + 1);
			else
				DisplayMessage.ins.showMessage ("Not enough kills yet. Kills remaining: " + GameObject.Find ("GameManager").GetComponent<GameManager> ().KillsLeft (), 2);
		}
	}
}
