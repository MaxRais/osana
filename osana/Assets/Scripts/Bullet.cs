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

	void OnTriggerEnter2D(Collider2D c) {
		if (c.gameObject != source) {
			if (c.gameObject.GetComponentInChildren<Player>()) {
				Debug.Log ("player");
				c.gameObject.GetComponent<Player> ().TakeDamage(2, transform.right);
			} else if (c.gameObject.GetComponentInChildren<Enemy>()) {
				Debug.Log ("enemy");
				c.gameObject.GetComponent<Enemy> ().health--;
				c.gameObject.GetComponent<Enemy> ().GetComponent<Rigidbody2D> ().AddForce (Vector3.right * direction * 200f);
				c.gameObject.GetComponent<Enemy> ().GetComponent<Rigidbody2D> ().AddForce (Vector3.up * 20f);

			}
			Destroy (this.gameObject);
		}
	}
}
