using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	public float speed;
	public int direction; // 1 or -1
	public GameObject source;
	public float destroyDistance = 20f;
	private Vector3 mousePos;
	// Use this for initialization
	void Start () {
		mousePos = Input.mousePosition;
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position += transform.right * speed * Time.deltaTime * direction;
		if (Vector3.Distance (this.transform.position, source.transform.position) > destroyDistance && this.gameObject != null) {
			Destroy (this.gameObject);
		}
	}

	void OnCollisionEnter2D(Collision2D c) {
		string name = c.collider.name;
		if(name.Contains("Player")) {
			GameObject.Find (name).GetComponent<Player> ().health--;
		} else if(name.Contains("Nanobot")) {
			GameObject.Find (name).GetComponent<Enemy> ().health--;
			GameObject.Find (name).GetComponent<Enemy> ().GetComponent<Rigidbody2D>().AddForce(Vector3.right * direction * 200f);
			GameObject.Find (name).GetComponent<Enemy> ().GetComponent<Rigidbody2D>().AddForce(Vector3.up * 20f);

		}

		Destroy (this.gameObject);
	}
}
