using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disease : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter2D(Collision2D c) {
		string name = c.collider.name;
		if(name.Contains("Player")) {
			GameObject player = GameObject.Find (name);
			player.GetComponent<Player> ().TakeDamage(1,(c.transform.position - transform.position).normalized);
		}
	}
}
