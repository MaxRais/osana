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
			player.GetComponent<Player> ().health--;
			if (player.GetComponent<Player> ().health > 0){
				player.GetComponent<Player> ().pushBack (3f);
				StartCoroutine (flashRed (player.GetComponentInChildren<SpriteRenderer> (), 3));
			}
		}
	}

	IEnumerator flashRed(SpriteRenderer sr, int times) {
		for (int i = 0; i < times; i++) {
			sr.color = Color.red;
			yield return new WaitForSeconds (.15f);
			sr.color = Color.white;
			yield return new WaitForSeconds (.15f);
		}
	}
}
