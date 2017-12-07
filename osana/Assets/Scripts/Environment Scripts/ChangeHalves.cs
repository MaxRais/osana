using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeHalves : MonoBehaviour {
	private GameObject player, leftHalf, rightHalf;
	public float switchPos, endPos;
	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Player");
		leftHalf = GameObject.Find ("LeftHalf");
		rightHalf = GameObject.Find ("RightHalf");
		player.GetComponent<Player> ().environment = leftHalf.transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (player.transform.position.x > endPos) {
			leftHalf.GetComponent<BoneRotate> ().enabled = false;
			rightHalf.GetComponent<BoneRotate> ().enabled = false;
		}
		else if (player.transform.position.x > switchPos) {
			leftHalf.GetComponent<BoneRotate> ().enabled = false;
			rightHalf.GetComponent<BoneRotate> ().enabled = true;
			player.GetComponent<Player> ().environment = rightHalf.transform;
		}
		else if (player.transform.position.x <= switchPos) {
			rightHalf.GetComponent<BoneRotate> ().enabled = false;
			leftHalf.GetComponent<BoneRotate> ().enabled = true;
			player.GetComponent<Player> ().environment = leftHalf.transform;
		}

	}
}
