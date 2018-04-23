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
	public void Level2() {
		SceneManager.LoadScene ("Level2");
	}
	public void Level3() {
		SceneManager.LoadScene ("Level3");
	}
	public void Mini1() {
		SceneManager.LoadScene ("SpeedrunLevel");
	}
	public void Mini2() {
		SceneManager.LoadScene ("Minigame1");
	}
	public void Mini3() {
		SceneManager.LoadScene ("Matching");
	}

	public void Exit() {
		Application.Quit ();
	}
}
