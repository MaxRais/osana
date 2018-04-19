using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HoleEntry : MonoBehaviour {

	private PositionTracker pt;
	public int minigameNumber;
	private bool created = false;

	// Use this for initialization
	void Start () {
		pt = GameObject.Find ("PositionTracker").GetComponent<PositionTracker>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void DisableMinigame() {
		this.GetComponent<Collider2D> ().enabled = false;
		this.transform.parent.Find ("Indicator").gameObject.SetActive (false);
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player") {
			pt.SetPlayerPos (this.transform.position);
			pt.LoadGame (minigameNumber);
		}
	}

}
