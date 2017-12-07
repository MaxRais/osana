using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeHalves : MonoBehaviour {
	private GameObject player, leftHalf, rightHalf;
	public Transform switchPos, endPos;
	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Player");
		leftHalf = GameObject.Find ("LeftHalf");
		rightHalf = GameObject.Find ("RightHalf");
		player.GetComponent<Player> ().environment = leftHalf.transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (player.transform.position.x > endPos.position.x) {
			leftHalf.GetComponent<BoneRotate> ().enabled = false;
			rightHalf.GetComponent<BoneRotate> ().enabled = false;
			if(player.GetComponent<Player>().spawnPoint.position.x < endPos.position.x)
				player.GetComponent<Player> ().updateSpawnPoint (endPos);
		} else if (player.transform.position.x > switchPos.position.x) {
			leftHalf.GetComponent<BoneRotate> ().enabled = false;
			rightHalf.GetComponent<BoneRotate> ().enabled = true;
			player.GetComponent<Player> ().environment = rightHalf.transform;
			if(player.GetComponent<Player>().spawnPoint.position.x < switchPos.position.x)
				player.GetComponent<Player> ().updateSpawnPoint (switchPos);
		}
		else if (player.transform.position.x <= switchPos.position.x) {
			rightHalf.GetComponent<BoneRotate> ().enabled = false;
			leftHalf.GetComponent<BoneRotate> ().enabled = true;
			player.GetComponent<Player> ().environment = leftHalf.transform;
		}

	}
}
