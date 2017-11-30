using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	public float speed;
	public int direction; // 1 or -1
	public GameObject source;
	private Vector3 startPos;
	public float destroyDistance = 20f;
	private Vector3 mousePos;
	public int damage = 1;

	// Use this for initialization
	void Start () {
		mousePos = Input.mousePosition;
		startPos = source.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position += transform.right * speed * Time.deltaTime * direction;
		if (Vector3.Distance (this.transform.position, startPos) > destroyDistance && this.gameObject != null) {
			Destroy (this.gameObject);
		}
	}

	void OnTriggerEnter2D(Collider2D c) {
		if (!source || c.gameObject.layer != source.layer) {
			if (c.gameObject.GetComponent<Player>()) {
				c.gameObject.GetComponent<Player> ().TakeDamage(damage, transform.right * direction);
			} else if (c.gameObject.GetComponent<Enemy>()) {
				c.gameObject.GetComponent<Enemy> ().TakeDamage (damage, transform.right * direction);
			}
			Destroy (this.gameObject);
		}
	}
}
