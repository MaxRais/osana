using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class flicker : MonoBehaviour {

	public float interval;
	private float timer;
	private bool enabled = false;
	// Use this for initialization
	void Start () {
		timer = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if (timer > interval) {
			this.GetComponent<Renderer> ().enabled = enabled;
			timer = 0;
			enabled = !enabled;
		}
	}
}
