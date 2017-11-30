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
	public float dmgBounceback = 20f;
	private float maxHealth;

	// Use this for initialization
	void Start () {
		maxHealth = health;
		traveled = 0;
		direction = 1;
		shot = false;
		if (!snapDown)
			this.GetComponent<Rigidbody2D> ().gravityScale = 0;


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
		RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.right * direction, (snapDown ? Vector2.down : Vector2.up), 2.8f, ~(1 << 10));
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

	void OnCollisionEnter2D(Collision2D col) {
		if (col.gameObject.tag == "Obstacle") {
			Rigidbody2D rb = this.GetComponent<Rigidbody2D> ();
			if (col.relativeVelocity.y > 10) {
				rb.velocity = Vector2.zero;
				rb.AddForce (Vector2.up * col.relativeVelocity.y);
			} else if (col.relativeVelocity.y < 10) {
				rb.gravityScale = 0;
				rb.velocity = new Vector2 (0, 0);
				transform.parent = null;
				transform.rotation = Quaternion.FromToRotation (transform.up, (snapDown ? col.transform.up : -col.transform.up)) * transform.rotation;
				transform.SetParent (col.transform);
			}
		}
	}
	void OnCollisionExit2D(Collision2D col) {
		if (col.gameObject.tag == "Obstacle" && snapDown) {
			Rigidbody2D rb = this.GetComponent<Rigidbody2D> ();
			rb.gravityScale = 1;
		}
	}

	public void TakeDamage(int amt, Vector2 dir) {
		Debug.Log (this.name + " taking " + amt + " dmg");
		this.health -= amt;
		Rigidbody2D rb = this.GetComponent<Rigidbody2D> ();
		if (snapDown) {
			rb.gravityScale = 1;
			dir.x *= dmgBounceback;
			dir.y = 5 * dmgBounceback;
			rb.AddForce (dir * amt);
		} else {
			if (this.health / maxHealth <= 0.5) {
				rb.gravityScale = 1;
				snapDown = true;
			}
		}
		StartCoroutine (HitPause ());
	}

	IEnumerator HitPause() {
		yield return new WaitForSeconds(0.75f);
	}

	public void shootProjectile() {
		if (shot)
			return;
		
		GameObject bullet = Instantiate (bulletPrefab) as GameObject;
		Bullet script = bullet.GetComponent<Bullet> ();
		script.speed = bulletSpeed;
		script.direction = direction;
		script.source = this.gameObject;
		bullet.transform.position = this.transform.position;
		bullet.transform.position += Vector3.right * this.transform.localScale.x * direction;
		shot = true;
	}
}
