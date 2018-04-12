using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HoleEntry : MonoBehaviour {

	private GameObject posTracker;
	private PositionTracker pt;

	// Use this for initialization
	void Start () {
		posTracker = GameObject.Find ("PositionTracker");
		pt = posTracker.GetComponent<PositionTracker> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player") {
			pt.SetPlayerPos (this.transform);
			pt.LoadScene ();
		}
	}

}
