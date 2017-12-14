using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneRotate : MonoBehaviour {
	public float upperBound, lowerBound, pauseTime, speed;
	public bool goingCW, pausing;
	private float pauseSecs;
	// Use this for initialization
	void Start () {
		pauseSecs = 0;
		pausing = true;
		foreach (Transform t in transform) {
			if(t.name.Contains("whitebloodcell"))
				t.GetComponent<FollowPlayer>().enabled = true;
		}
	}

	void OnDisable() {
		foreach (Transform t in transform) {
			if(t.name.Contains("whitebloodcell"))
				t.GetComponent<FollowPlayer>().enabled = false;
		}
	}

	// Update is called once per frame
	void Update () {
		if (!pausing) {
			float zRotation = this.transform.rotation.eulerAngles.z;
			if(zRotation > 180)
				zRotation -= 360;
			if (goingCW) {
				if (zRotation > lowerBound) {
					this.transform.Rotate (Vector3.forward * speed * -Time.deltaTime);
				} else if (zRotation <= lowerBound) {
					pausing = true;
					goingCW = false;
				}
			} else {
				if (zRotation < upperBound) {
					this.transform.Rotate (Vector3.forward * speed * Time.deltaTime);
				} else if (zRotation >= upperBound) {
					pausing = true;
					goingCW = true;
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