using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PositionTracker : MonoBehaviour {

	private Transform playerPos;
	private static bool created = false;
	void Awake() {
		if (!created) {
			DontDestroyOnLoad (this.gameObject);
			created = true;
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetPlayerPos(Transform t) {
		this.playerPos = t;
	}

	void OnEnable() {
		SceneManager.sceneLoaded += OnLevelLoaded;
	}

	void OnDisable() {
		SceneManager.sceneLoaded -= OnLevelLoaded;
	}

	void OnLevelLoaded(Scene scene, LoadSceneMode mode) {
		if (playerPos != null && SceneManager.GetActiveScene().name == "Level2")
			GameObject.FindGameObjectWithTag ("Player").transform.position = playerPos.position;
	}

	public void LoadScene() {
		if (SceneManager.GetActiveScene ().name == "Level2") {
			SceneManager.LoadScene ("Minigame1");
		}
		if (SceneManager.GetActiveScene ().name == "Minigame1") {
			SceneManager.LoadScene ("Level2");
		}
		if (SceneManager.GetActiveScene ().name == "Level3") {
			SceneManager.LoadScene ("Matching");
		}
		if (SceneManager.GetActiveScene ().name == "Matching") {
			SceneManager.LoadScene ("Level3");
		}
	}
}
