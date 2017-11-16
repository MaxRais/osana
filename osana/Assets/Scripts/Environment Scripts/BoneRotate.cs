﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneRotate : MonoBehaviour {
	public float upperBound, lowerBound, pauseTime, speed;
	public bool goingUp, pausing;
	private float pauseSecs;
	// Use this for initialization
	void Start () {
		pauseSecs = 0;
		pausing = true;
	}

	// Update is called once per frame
	void Update () {
		if (!pausing) {
			float zRotation = this.transform.rotation.eulerAngles.z;
			if(zRotation > 180)
				zRotation -= 360;
			Debug.Log (zRotation);
			if (goingUp) {
				if (zRotation > lowerBound) {
					this.transform.Rotate (Vector3.forward * speed * -Time.deltaTime);
				} else if (zRotation <= lowerBound) {
					pausing = true;
					goingUp = false;
				}
			} else {
				if (zRotation < upperBound) {
					this.transform.Rotate (Vector3.forward * speed * Time.deltaTime);
				} else if (zRotation >= upperBound) {
					pausing = true;
					goingUp = true;
				}
			}
		} else {
			pauseSecs += Time.deltaTime;
			if(pauseSecs > pauseTime) {
				pauseSecs = 0;
				pausing = false;
			}
		}
	}
}