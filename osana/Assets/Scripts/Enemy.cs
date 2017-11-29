using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public float range;
	public float speed;
	private float traveled;
	private int direction;
	public float detectDistance;
	public GameObject player;
	public int health;
	public float bulletSpeed;
	public GameObject bulletPrefab;
	private bool shot;

	// Use this for initialization
	void Start () {
		traveled = 0;
		direction = 1;
		shot = false;
	}
	
	// Update is called once per frame
	void Update () {
		traveled += Vector3.Distance (transform.position, transform.position + transform.right * speed * Time.deltaTime * direction);
		Debug.Log (traveled);
		this.transform.position += transform.right * speed * Time.deltaTime * direction;
		if (traveled >= range) {
			traveled = 0;
			shot = false;
			direction *= -1;
		}
		if (health <= 0) {
			Destroy (this.gameObject);
		}

		if (Vector3.Distance (this.transform.position, player.transform.position) <= detectDistance &&
			Mathf.Sign(direction) == Mathf.Sign(player.transform.position.x - this.transform.position.x)) {
			shootProjectile ();
		}
	}

	void FixedUpdate() {
		RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(direction * 3, 0, 0), -transform.up, 5f, ~(1 << 10));
		if (hit.collider == null) {	
			traveled = 0;
			shot = false;
			direction *= -1;
		}
	}

	public void shootProjectile() {
		if (shot)
			return;
		
		GameObject bullet = Instantiate (bulletPrefab) as GameObject;
		bullet.transform.position = this.transform.position;
		Bullet script = bullet.AddComponent<Bullet> ();
		script.speed = bulletSpeed;
		script.direction = direction;
		bullet.transform.position += Vector3.right * this.transform.localScale.x * direction;
		script.source = this.gameObject;
		shot = true;
	}
}
