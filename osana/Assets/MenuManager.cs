using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Tutorial() {
		SceneManager.LoadScene ("Tutorial");
	}

	public void Level1() {
		SceneManager.LoadScene ("Level1Reverse");
	}

	public void Exit() {
		Application.Quit ();
	}
}
