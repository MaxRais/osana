using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackWhiteBloodCells : MonoBehaviour {

	private GameObject[] wbc;
	public int health = 3;

	// Use this for initialization
	void Start () {
		wbc = GameObject.FindGameObjectsWithTag ("WBC");
	}
	
	// Update is called once per frame
	void Update () {
		int currentLatched = 0;
		foreach (GameObject cell in wbc) {
			FollowPlayer fp = cell.GetComponent<FollowPlayer> ();
			if (fp.latched) {
				currentLatched++;
			}
		}
		if (currentLatched >= health) {
			this.GetComponent<Player> ().restart ();
		}
	}
}
