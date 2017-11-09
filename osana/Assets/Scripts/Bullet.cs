using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	public float speed;
	public int direction; // 1 or -1
	public GameObject source;
	public float destroyDistance = 20f;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position += Vector3.right * speed * Time.deltaTime * direction;

		if (Vector3.Distance (this.transform.position, source.transform.position) > destroyDistance) {
			Destroy (this.gameObject);
		}
	}

	void OnCollisionEnter2D(Collision2D c) {
		string name = c.collider.name;
		if(name.Contains("Player")) {
			GameObject.Find (name).GetComponent<Player> ().health--;
		} else if(name.Contains("Nanobot")) {
			GameObject.Find (name).GetComponent<Enemy> ().health--;
		}

		Destroy (this.gameObject);
	}
}
