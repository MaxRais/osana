using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public int goalKills;
	private int killsLeft;
	private bool isFinished;

	// Use this for initialization
	void Start () {
		killsLeft = goalKills;
		isFinished = false;
	}
	// Update is called once per frame
	void Update () {
		
	}
		
	public bool IsFinished() {
		return isFinished;
	}
	public int KillsLeft() {
		return killsLeft;
	}

	public void AddKill() {
		killsLeft--;
		if (killsLeft > 0) {
			DisplayMessage.ins.showMessage (string.Concat (killsLeft, " enemies left"), 3);
		} else if (killsLeft == 0) {
			isFinished = true;
			DisplayMessage.ins.showMessage ("Proceed to the exit.", 2);
		}
	}
}
