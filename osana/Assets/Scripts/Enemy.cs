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
	public float sizeOffset = 5f;
	public bool snapDown = true;

	// Use this for initialization
	void Start () {
		traveled = 0;
		direction = 1;
		shot = false;


		// Snap enemy to platform they are on
		RaycastHit2D hit = Physics2D.Raycast(transform.position, (snapDown ? Vector2.down : Vector2.up), 25f, ~(1 << 10));
		if (hit.collider != null && hit.collider.tag == "Obstacle") { 
			transform.position = hit.point + (hit.normal * sizeOffset);
			transform.rotation = Quaternion.FromToRotation (transform.up, hit.normal) * transform.rotation;
			transform.SetParent (hit.transform);
		}
	}
	
	// Update is called once per frame
	void Update () {
		traveled += Vector3.Distance (transform.position, transform.position + transform.right * speed * Time.deltaTime * direction);
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
		RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3((snapDown ? 1 : -1) * direction, 0, 0),
			(snapDown ? Vector2.down : Vector2.up), 10f, ~(1 << 10));
		if (hit.collider == null) {	
			traveled = 0;
			shot = false;
			direction *= -1;
		} else if (hit.collider != null && hit.collider.tag == "Obstacle" && hit.transform != this.transform.parent) {
			transform.parent = null;
			transform.position = hit.point + (hit.normal * sizeOffset);
			transform.rotation = Quaternion.FromToRotation (transform.up, hit.normal) * transform.rotation;
			transform.SetParent (hit.transform);
		}
	}

	public void TakeDamage(int amt, Vector2 dir) {
		this.health -= amt;
		Rigidbody2D rb = this.GetComponent<Rigidbody2D> ();
		rb.gravityScale = 1;
		dir.y = 5f;
		rb.AddForce (dir * amt * 10f);
		StartCoroutine (HitPause ());
	}

	IEnumerator HitPause() {
		yield return new WaitForSeconds(0.75f);
		this.GetComponent<Rigidbody2D> ().gravityScale = 0;
		this.GetComponent<Rigidbody2D> ().velocity = new Vector2 (0, 0);
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
