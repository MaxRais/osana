using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchingButton : MonoBehaviour {

	public bool shot;
	private bool hitFrame;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (hitFrame) {
			shot = true;
			hitFrame = false;
		} else {
			shot = false;
		}
	}


	void OnTriggerEnter2D(Collider2D col) {
		if (col.name.Contains ("Bullet")) {
			hitFrame = true;
		}
	}
}
