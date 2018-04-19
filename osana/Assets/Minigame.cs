using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigame : MonoBehaviour {

	public int goalCollect;
	private int collected;
	private Miniboss mb;
	private PositionTracker pt;
	private bool introSpeech = false;

	// Use this for initialization
	void Start () {
		mb = GameObject.Find ("Miniboss").GetComponent<Miniboss> ();
		pt = GameObject.Find ("PositionTracker").GetComponent<PositionTracker> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!introSpeech) {
			DisplayMessage.ins.showMessage ("Collect 15 protein to break down the boss shield", 4);
			introSpeech = true;
		}
	}

	public void Collect() {
		if(collected < goalCollect)
			collected += 1;
		else if (collected >= goalCollect) {
			mb.BreakShield ();
			DisplayMessage.ins.showMessage ("Shields down, defeat the boss", 4);
			collected = -10;
		}
	}

	public void Win() {
		DisplayMessage.ins.showMessage ("Boss defeated. Returning to lungs", 4);
		StartCoroutine (ReturnLungs ());
	}

	IEnumerator ReturnLungs() {
		yield return new WaitForSeconds (3);
		pt.WinGame ();
	}
}
