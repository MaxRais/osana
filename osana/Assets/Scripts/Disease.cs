using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disease : MonoBehaviour {

	public float dmgDelay = 1f;
	private float timer;
	// Use this for initialization
	void Start () {
		timer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
	}

	void OnCollisionStay2D(Collision2D c) {
		string name = c.collider.name;
		if(name.Contains("Player")) {
			GameObject player = GameObject.Find (name);
			if (timer > dmgDelay) {
				player.GetComponent<Player> ().TakeDamage (1, (c.transform.position - transform.position).normalized);
				timer = 0;
			}
		}
	}
}
