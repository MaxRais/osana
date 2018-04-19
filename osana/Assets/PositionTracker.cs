using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PositionTracker : MonoBehaviour {

	private Vector3 playerPos;
	private static bool created = false;
	private static bool wonGame = false;
	public string[] minigames;
	private int currentGame = -1;
	void Awake() {
		if (!created) {
			DontDestroyOnLoad (this.gameObject);
			created = true;
		}
	}

	// Use this for initialization
	void Start () {
		for (int i = 0; i < minigames.Length; i++) {
			GameObject.Find(minigames [i]).GetComponentInChildren<HoleEntry> ().minigameNumber = i;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetPlayerPos(Vector3 t) {
		this.playerPos = t;
	}

	void OnEnable() {
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnDisable() {
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	public void WinGame() {
		wonGame = true;
		if (SceneManager.GetActiveScene ().name == "Minigame1") {
			SceneManager.LoadScene ("Level2");
		}
		if (SceneManager.GetActiveScene ().name == "Matching") {
			SceneManager.LoadScene ("Level3");
		}
	}
	public void LoseGame() {
		playerPos.x -= 3;
		playerPos.y += 5;
		if (SceneManager.GetActiveScene ().name == "Minigame1") {
			SceneManager.LoadScene ("Level2");
		}
		if (SceneManager.GetActiveScene ().name == "Matching") {
			SceneManager.LoadScene ("Level3");
		}
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		if (wonGame) {
			GameObject.Find("GameManager").GetComponent<GameManager> ().Collect ();
			GameObject.Find(minigames [currentGame]).GetComponentInChildren<HoleEntry> ().DisableMinigame ();
			wonGame = false;
		}
		if (playerPos != Vector3.zero && SceneManager.GetActiveScene().name == "Level2" || SceneManager.GetActiveScene().name == "Level3")
			GameObject.FindGameObjectWithTag ("Player").transform.position = playerPos;
	}

	public void LoadGame(int minigameNumber) {
		currentGame = minigameNumber;
		if (SceneManager.GetActiveScene ().name == "Level2") {
			SceneManager.LoadScene ("Minigame1");
		}
		if (SceneManager.GetActiveScene ().name == "Level3") {
			SceneManager.LoadScene ("Matching");
		}
	}
}
