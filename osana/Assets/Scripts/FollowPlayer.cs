using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {
	public GameObject player; 
	public float speed = 3f;
	public float maxDistance = 13f;
	public float minDistance = 2f;

	private bool alertedFollow = false;
	private bool alertedLatch = false;

	public bool latched = false;

	// Use this for initialization
	void Start () {
		player = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () {
		transform.LookAt(player.transform);
		transform.Rotate(new Vector3(0,-90,0),Space.Self);
		if (Vector3.Distance (this.transform.position, player.transform.position) > 1f && Vector3.Distance (this.transform.position, player.transform.position) < maxDistance) {
			if (!alertedFollow) {
				//DisplayMessage.ins.showMessage ("you are being followed");
				alertedFollow = true;
			}
			speed = 3f;
			this.transform.Translate (new Vector3 (speed * Time.deltaTime, 0, 0));
		} else {
			alertedFollow = false;
		}

		if (Vector3.Distance (this.transform.position, player.transform.position) < minDistance) {
			latched = true;
			speed = 6f;
			this.transform.Translate (new Vector3 (speed * Time.deltaTime, 0, 0));

			if (!alertedLatch) {
				//DisplayMessage.ins.showMessage ("white blood cell has latched");
				alertedLatch = true;
			}
		} else {
			latched = false;
			alertedLatch = false;
		}
	}
}
