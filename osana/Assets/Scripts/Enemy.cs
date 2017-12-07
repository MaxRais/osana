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
	public bool snapDown = true;
	public float dmgBounceback = 2f;
	public float bounciness = 15f;
	public float shotDelay = 3f;
	public int bumpDamage = 1;
	private float maxHealth;
	private float shotTimer;
	// Use this for initialization
	void Start () {
		maxHealth = health;
		traveled = 0;
		direction = 1;
		shot = false;
		this.GetComponent<Rigidbody2D> ().gravityScale = 0;


		// Snap enemy to platform they are on
		RaycastHit2D hit = Physics2D.Raycast(transform.position, (snapDown ? Vector2.down : Vector2.up), 25f, ~(1 << 10));
		if (hit.collider != null && hit.collider.tag == "Obstacle") { 
			SnapTo (hit.transform, hit.point,  hit.normal);
		}
	}
	
	// Update is called once per frame
	void Update () {
		shotTimer += Time.deltaTime;
		if (shotTimer >= shotDelay) {
			shotTimer = 0;
			shot = false;
		}
		traveled += Vector3.Distance (transform.position, transform.position + transform.right * speed * Time.deltaTime * direction);
		this.transform.position += transform.right * speed * Time.deltaTime * direction;
		if (traveled >= range) {
			traveled = 0;
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

	void SnapTo(Transform surface, Vector3 pos, Vector3 normal) {
		transform.parent = null;
		transform.position = pos;
		transform.rotation = Quaternion.FromToRotation (transform.up, normal) * transform.rotation;
		transform.localScale = new Vector3 (2.5f, 2.5f, 1);
		transform.SetParent (surface);
	}

	void FixedUpdate() {
		RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, 2.8f, ~(1 << 10));
		RaycastHit2D hit2 = Physics2D.Raycast(transform.position + transform.right * direction * 1.3f, -transform.up + transform.right * -direction, 3.1f, ~(1 << 10));
		if (hit.collider == null) {
			if (hit2.collider == null) {	
				traveled = 0;
				direction *= -1;
			} else if (hit2.collider != null && hit2.collider.tag == "Obstacle") {
				SnapTo (hit2.transform, hit2.point, hit2.normal);
			}
		} else if (hit.collider != null && hit.collider.tag == "Obstacle" && hit.transform != this.transform.parent) {
			SnapTo (hit.transform, hit.point, hit.normal);
		}
		RaycastHit2D hit3 = Physics2D.Raycast(transform.position, transform.right * direction, 3f, ~(1 << 10));
		if (hit3.collider != null && hit3.collider.tag == "Obstacle" && hit3.transform != this.transform.parent) {
			SnapTo (hit3.transform, hit3.point, hit3.normal);
		}
		snapDown = (this.transform.up.y > 0);
	}

	void OnCollisionEnter2D(Collision2D col) {
		if (col.gameObject.tag == "Obstacle") {
			Rigidbody2D rb = this.GetComponent<Rigidbody2D> ();
			if (col.relativeVelocity.y > 10) {
				rb.velocity = Vector2.zero;
				rb.AddForce (Vector2.up * col.relativeVelocity.y * bounciness);
			} else if (col.relativeVelocity.y < 10) {
				rb.gravityScale = 0;
				rb.velocity = new Vector2 (0, 0);
			}
		}
		if (col.gameObject.tag == "Enemy") {
			direction *= -1;
		}
		if (col.gameObject.tag == "Player") {
			Vector2 bumpDir = (col.transform.position - transform.position).normalized;
			col.gameObject.GetComponent<Player> ().TakeDamage (bumpDamage, bumpDir);
		}
	}

	public void TakeDamage(int amt, Vector2 dir) {
		Debug.Log (this.name + " taking " + amt + " dmg");
		this.health -= amt;
		Rigidbody2D rb = this.GetComponent<Rigidbody2D> ();
		if (snapDown) {
			dir.x *= dmgBounceback;
			dir.y = 1.2f * dmgBounceback;
			rb.constraints = RigidbodyConstraints2D.FreezeRotation;
			rb.velocity = (dir * amt);
			rb.gravityScale = 1;
		} else {
			if (this.health / maxHealth <= 0.5) {
				rb.velocity = Vector2.zero;
				rb.gravityScale = 1;
				transform.parent = null;
				transform.rotation = new Quaternion (0, 0, 0, 0);
				snapDown = true;
			}
		}
		StartCoroutine (Blink (amt));
	}

	IEnumerator Blink(int amt) {
		for (int i = 0; i < amt * 2; i++) {
			if (i % 2 == 0)
				this.GetComponentInChildren<SpriteRenderer> ().color = Color.red;
			else
				this.GetComponentInChildren<SpriteRenderer> ().color = Color.white;
			yield return new WaitForSeconds (0.1f/((float)amt));
		}

	}

	IEnumerator HitPause() {
		yield return new WaitForSeconds(0.75f);
	}

	public void shootProjectile () {
		if (shot)
			return;
		Debug.Log (this.name + " firing");
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
