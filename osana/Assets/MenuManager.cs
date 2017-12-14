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
		SceneManager.LoadScene ("Level1Dave");
	}

	public void Level1() {
		SceneManager.LoadScene ("Intro");
	}

	public void Exit() {
		Application.Quit ();
	}
}
