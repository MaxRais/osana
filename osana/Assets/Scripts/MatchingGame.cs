﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchingGame : MonoBehaviour {

	public GameObject[] buttons;
	public Color[] colors;
	private int activeButton; 
	public GameObject path;
	public GameObject neuroPrefab;
	public Transform neuroSpawnPoint;
	public float minSpeed;
	public float maxSpeed;
	public int maxScore;
	private int score;
	public int maxFail;
	private int failed;
	public float minSpawnDelay; // in seconds
	public float randomRange;
	private float spawnWait;
	private bool started;
	private bool won;
	private bool lost;

	// Use this for initialization
	void Start () {
		if (buttons.Length != colors.Length) {
			Debug.LogError ("Colors and buttons not same size");
		}
		for (int i = 0; i < buttons.Length; i++) {
			buttons [i].GetComponent<SpriteRenderer> ().color = colors [i];
		}
		setActiveButton(0);
		StartCoroutine (waitAndStart ());
	}

	IEnumerator waitAndStart() {
		yield return new WaitForSeconds (1.0f);
		DisplayMessage.ins.showMessage ("Try shooting those growths in the neuron");
		yield return new WaitForSeconds (5.0f);
		started = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (!started) {
			return;
		}

		if (won || lost) {
			return;
		}

		int i = 0;
		foreach (GameObject go in buttons) {
			if (go.GetComponent<MatchingButton> ().shot) {
				setActiveButton (i);
			}
			i++;
		}

		if (spawnWait <= 0) {
			GameObject g = Instantiate (neuroPrefab) as GameObject;
			g.transform.position = neuroSpawnPoint.transform.position;
			g.GetComponent<SpriteRenderer> ().color = colors [Random.Range (0, colors.Length)];
			g.GetComponent<Bullet> ().source = GameObject.Find("Player");
			float percentage = (float)score / (float)maxScore;
			g.GetComponent<Bullet> ().speed = minSpeed + (maxSpeed - minSpeed) * percentage;
			spawnWait = minSpawnDelay + Random.Range (0, randomRange);
		} else {
			spawnWait -= Time.deltaTime;
		}
	}

	void setActiveButton(int index) {
		activeButton = index;
		this.GetComponent<SpriteRenderer> ().color = colors [index];
		foreach (SpriteRenderer sr in path.GetComponentsInChildren<SpriteRenderer>()) {
			sr.color = colors [index];
		}
	}

	void OnTriggerEnter2D(Collider2D col) {
		if (col.name.Contains ("Neurotransmitter")) {
			int id = getIdFromColor (col.gameObject);
			if (id == activeButton) {
				score++;
				DisplayMessage.ins.showMessage (score + " neurotransmitters through", 0.25f);
				if (score == maxScore) {
					won = true;
					DisplayMessage.ins.showMessage ("That is all of them. Nice work Osana");
				}
			} else {
				failed++;
				DisplayMessage.ins.showMessage ("That was not right", 0.25f);
				if (failed == maxFail) {
					lost = true;
					DisplayMessage.ins.showMessage ("You have failed");
				}
			}
			Destroy (col.gameObject);
		}
	}

	int getIdFromColor(GameObject go) {
		Color c = go.GetComponent<SpriteRenderer> ().color;
		int i = 0;
		foreach (Color color in colors) {
			if (color.Equals (c)) {
				return i;
			}
			i++;
		}
		return -1;
	}
}
