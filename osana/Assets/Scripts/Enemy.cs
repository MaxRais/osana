using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public float range;
	public float speed;
	private float center;
	private int direction;
	public float detectDistance;
	public GameObject player;
	public int health;
	public float bulletSpeed;
	public GameObject bulletPrefab;
	private bool shot;

	// Use this for initialization
	void Start () {
		center = this.transform.position.x;
		direction = 1;
		shot = false;
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position += transform.right * speed * Time.deltaTime * direction;
		if (this.transform.position.x <= center - range || this.transform.position.x >= center + range) {
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
