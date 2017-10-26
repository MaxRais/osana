using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {
	public GameObject player; 
	public float speed = 3f;
	public float maxDistance = 10f;
	// Use this for initialization
	void Start () {
		player = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () {
		transform.LookAt(player.transform);
		transform.Rotate(new Vector3(0,-90,0),Space.Self);
		if (Vector3.Distance(this.transform.position, player.transform.position) > 1f && Vector3.Distance(this.transform.position, player.transform.position) < maxDistance)
			this.transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
	}
}
