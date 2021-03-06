﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeHalves : MonoBehaviour {
	private GameObject player, leftHalf, rightHalf, muscle;
	public Transform switchPos, endPos;
	private bool alertedMuscleChange;
	private bool alertedBoneChange1;
	private bool alertedBoneChange2;

	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Player");
		leftHalf = GameObject.Find ("LeftHalf");
		rightHalf = GameObject.Find ("RightHalf");
		muscle = GameObject.Find ("GameMapMuscle");
		player.GetComponent<Player> ().environment = leftHalf.transform;
	}
	
	// Update is called once per frame
	void Update () {
		BoneRotate leftRotate = leftHalf.GetComponent<BoneRotate> ();
		BoneRotate rightRotate = rightHalf.GetComponent<BoneRotate> ();
		if (!leftRotate.colliding && !rightRotate.colliding) {
			leftRotate.Disable ();
			rightRotate.Disable ();
			leftHalf.transform.Find ("BoneMask").gameObject.SetActive (false);
			rightHalf.transform.Find ("BoneMask").gameObject.SetActive (false);
			muscle.transform.Find ("MuscleMask").gameObject.SetActive (true);
			foreach(Transform t in muscle.transform)
				if(t.name.Contains("whitebloodcell"))
					t.GetComponent<FollowPlayer>().enabled = true;
			
			//player.GetComponent<Player> ().updateSpawnPoint (endPos);
			//DisplayMessage.ins.showMessage ("Checkpoint. HP Restored", 1);
			if (!alertedMuscleChange) {
				DisplayMessage.ins.showMessage ("Muscles contract. avoid being crushed");
				alertedMuscleChange = true;
			}
		} else if (rightRotate.colliding) {
			leftRotate.Disable ();
			rightRotate.Enable ();
			leftHalf.transform.Find ("BoneMask").gameObject.SetActive (false);
			rightHalf.transform.Find ("BoneMask").gameObject.SetActive (true);
			muscle.transform.Find ("MuscleMask").gameObject.SetActive (false);
			player.GetComponent<Player> ().environment = rightHalf.transform;
			//player.GetComponent<Player> ().updateSpawnPoint (endPos);
			if (!alertedBoneChange1) {
				player.GetComponent<Player> ().health += 10;
				DisplayMessage.ins.showMessage ("Checkpoint. HP Restored", 1);
				DisplayMessage.ins.showMessage ("These bones are moving. He must be walking");
				alertedBoneChange1 = true;
			}
		} else if (leftRotate.colliding) {
			rightRotate.Disable ();
			leftRotate.Enable ();
			leftHalf.transform.Find ("BoneMask").gameObject.SetActive (true);
			rightHalf.transform.Find ("BoneMask").gameObject.SetActive (false);
			muscle.transform.Find ("MuscleMask").gameObject.SetActive (false);
			player.GetComponent<Player> ().environment = leftHalf.transform;
			//player.GetComponent<Player> ().updateSpawnPoint (switchPos);
			if (!alertedBoneChange2) {
				player.GetComponent<Player> ().health += 10;
				DisplayMessage.ins.showMessage ("Checkpoint. HP Restored", 1);
				DisplayMessage.ins.showMessage ("Crossed joint");
				alertedBoneChange2 = true;
			}
		}


		/*if (player.transform.position.x > endPos.position.x) {
			leftHalf.GetComponent<BoneRotate> ().enabled = false;
			rightHalf.GetComponent<BoneRotate> ().enabled = false;
			rightHalf.transform.Find ("BoneMask").gameObject.SetActive (false);
			muscle.transform.Find ("MuscleMask").gameObject.SetActive (true);
			foreach(Transform t in muscle.transform)
				if(t.name.Contains("whitebloodcell"))
					t.GetComponent<FollowPlayer>().enabled = true;
			if (player.GetComponent<Player> ().spawnPoint.position.x < endPos.position.x) {
				player.GetComponent<Player> ().health += 10;
				player.GetComponent<Player> ().updateSpawnPoint (endPos);
				DisplayMessage.ins.showMessage ("Checkpoint. HP Restored", 1);
				DisplayMessage.ins.showMessage ("Muscles contract. avoid being crushed");
			}
		} else if (player.transform.position.x > switchPos.position.x) {
			leftHalf.GetComponent<BoneRotate> ().enabled = false;
			rightHalf.GetComponent<BoneRotate> ().enabled = true;
			leftHalf.transform.Find ("BoneMask").gameObject.SetActive (false);
			rightHalf.transform.Find ("BoneMask").gameObject.SetActive (true);
			player.GetComponent<Player> ().environment = rightHalf.transform;
			if (player.GetComponent<Player> ().spawnPoint.position.x < switchPos.position.x) {
				player.GetComponent<Player> ().updateSpawnPoint (switchPos);
				player.GetComponent<Player> ().health += 10;
				DisplayMessage.ins.showMessage ("Checkpoint. HP Restored", 1);
				DisplayMessage.ins.showMessage ("Crossed joint");
			}
		} else if (player.transform.position.x <= switchPos.position.x) {
			rightHalf.GetComponent<BoneRotate> ().enabled = false;
			leftHalf.GetComponent<BoneRotate> ().enabled = true;
			leftHalf.transform.Find ("BoneMask").gameObject.SetActive (true);
			rightHalf.transform.Find ("BoneMask").gameObject.SetActive (false);
			player.GetComponent<Player> ().environment = leftHalf.transform;
		} if (player.transform.position.x < endPos.position.x) {
			muscle.transform.Find ("MuscleMask").gameObject.SetActive (false);
			foreach(Transform t in muscle.transform)
				if(t.name.Contains("whitebloodcell"))
					t.GetComponent<FollowPlayer>().enabled = false;
		}*/
	}
}
