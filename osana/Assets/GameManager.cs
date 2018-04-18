using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public int goalKills;
	public int goalCollect;
	private int collected;
	private int killsLeft;
	private bool isFinished;
	public Transform[] checkpoints;
	private int currentCheck = 0;
	public bool horizontal;
	private GameObject player1;

	// Use this for initialization
	void Start () {
		killsLeft = goalKills;
		isFinished = false;
		collected = 0;

		if (goalKills == 0 && goalCollect == 0)
			isFinished = true;
		player1 = GameObject.FindGameObjectWithTag ("Player");
	}
	// Update is called once per frame
	void Update () {
		if (horizontal && player1.transform.position.x > checkpoints [currentCheck].position.x) {
			player1.GetComponent<Player> ().updateSpawnPoint (checkpoints [currentCheck]);
			if(currentCheck + 1 < checkpoints.Length)
				currentCheck++;
		}

		else if (!horizontal && checkpoints.Length > 0 && player1.transform.position.y > checkpoints [currentCheck].position.y) {
			player1.GetComponent<Player> ().updateSpawnPoint (checkpoints [currentCheck]);
			Debug.Log (checkpoints [currentCheck].name);
			if(currentCheck + 1 < checkpoints.Length)
				currentCheck++;
		}
	}
		
	public bool IsFinished() {
		return isFinished;
	}
	public int KillsLeft() {
		return killsLeft;
	}
	public int ItemsLeft() {
		return collected;
	}

	public void Collect() {
		collected++;
		if (collected == goalCollect) {
			isFinished = true;
			DisplayMessage.ins.showMessage ("Collection goal reached");
		}
	}
	public void AddKill() {
		killsLeft--;
		if (killsLeft > 0) {
			DisplayMessage.ins.showMessage (string.Concat (killsLeft, " enemies left"), 3);
		} else if (killsLeft == 0 && goalCollect == 0) {
			isFinished = true;
			DisplayMessage.ins.showMessage ("Proceed to the exit.", 2);
		}
	}
}
