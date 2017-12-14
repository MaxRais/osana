using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuscleContract : MonoBehaviour {

	private bool expanding = true;
	public float expandSpeed = 1f;
	public float delay = 1f;
	private float timer = 0;
	private bool pausing = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (pausing) {
			timer += Time.deltaTime;
			if (timer > delay) {
				pausing = false;
				timer = 0;
			}
		} else {
			if (expanding && transform.localScale.y < 2) {
				transform.localScale += new Vector3 (0, Time.deltaTime * expandSpeed, 0);
			} else if (expanding && transform.localScale.y >= 2) {
				pausing = true;
				expanding = false;
			} else if (!expanding && transform.localScale.y > 1) {
				transform.localScale -= new Vector3 (0, Time.deltaTime * expandSpeed, 0);
			} else if (!expanding && transform.localScale.y <= 1) {
				pausing = true;
				expanding = true;
			}
		}

	}
}
