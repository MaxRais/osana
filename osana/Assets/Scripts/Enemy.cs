﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public float range;
	public float speed;
	private float traveled;
	private int direction;
	public float detectDistance;
	private GameObject player;
	public int health;
	public float bulletSpeed;
	public GameObject bulletPrefab;
	public GameObject healthBar;
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
		player = GameObject.FindGameObjectWithTag ("Player");

		// Snap enemy to platform they are on
		RaycastHit2D hit = Physics2D.Raycast(transform.position, (snapDown ? Vector2.down : Vector2.up), 25f, ~(1 << 10));
		if (hit.collider != null && hit.collider.tag == "Obstacle") { 
			SnapTo (hit.transform, hit.point,  hit.normal);
		}
	}
	
	// Update is called once per frame
	void Update () {
		BoxCollider2D col = this.GetComponent<BoxCollider2D> ();
		RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.right * speed * Time.deltaTime * direction, -transform.up, 6f, ~(1 << 10));
		healthBar.transform.localScale = new Vector3 ((health / maxHealth) , 0.25f, 0.35f);
		if(shot)
			shotTimer += Time.deltaTime;
		if (shotTimer >= shotDelay) {
			shotTimer = 0;
			shot = false;
		}
		if ((traveled >= range && range > 0) || hit.collider == null) {
			traveled = 0;
			direction *= -1;
		} else if (hit.collider != null && hit.collider.tag == "Obstacle") {
			traveled += Vector3.Magnitude (transform.right * speed * Time.deltaTime * direction);
			SnapTo (hit.transform, hit.point, hit.normal);
		}
		if (health <= 0) {
			GameObject manager = GameObject.Find ("GameManager");
			if(manager)
				manager.GetComponent<GameManager> ().AddKill ();
			Destroy (this.gameObject);
		}
		if (Vector3.Distance (transform.position, player.transform.position) <= detectDistance) {
			//Debug.Log(this.name + " trying to shoot");
			shootProjectile ();
		}

	}

	void SnapTo(Transform surface, Vector3 pos, Vector3 normal) {
		BoxCollider2D col = this.GetComponent<BoxCollider2D> ();
		transform.parent = null;
		Vector3 oldPos = transform.position;
		transform.rotation = Quaternion.FromToRotation (transform.up, normal) * transform.rotation;
		transform.position = pos + transform.up * col.size.y;
		transform.SetParent (surface);
	}

	void FixedUpdate() {
		/*BoxCollider2D col = this.GetComponent<BoxCollider2D> ();
		float btm = col.offset.y - (col.size.y / 2f);
		float left = col.offset.x - (col.size.x / 2f);
		float right = col.offset.x + (col.size.x / 2f); 
		Vector3 btmMiddle = transform.TransformPoint (new Vector3 (0f, btm, 0f));
		Vector3 btmRight = transform.TransformPoint(new Vector3(right, btm, 0f));
		Vector3 btmLeft = transform.TransformPoint(new Vector3(left, btm, 0f));
		Debug.DrawRay ((direction == 1 ? btmRight : btmLeft) + transform.up, transform.right * direction, Color.cyan, 3f);
		RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, 7f, ~(1 << 10));
		RaycastHit2D hit2 = Physics2D.Raycast((direction == 1 ? btmRight : btmLeft),-transform.up + (transform.right * -direction), 4f, ~(1 << 10));
		if (hit.collider == null || hit.transform != transform.parent) {
			if (hit2.collider == null) {	
				traveled = 0;
				direction *= -1;
			} else if (hit2.collider != null && hit2.collider.tag == "Obstacle" && hit2.transform != transform.parent) {
				SnapTo (hit2.transform, hit2.point, hit2.normal);
				Debug.DrawRay (hit2.point, hit2.normal, Color.red, 3);
			}
		} else if (hit.collider != null && hit.collider.tag == "Obstacle" && hit.transform == this.transform.parent) {
			if(Vector3.Distance(btmMiddle, new Vector3(hit.point.x,hit.point.y, btmMiddle.z)) > 1f)
				SnapTo (hit.transform, hit.point, hit.normal);
		}
			//
		RaycastHit2D hit3 = Physics2D.Raycast((direction == 1 ? btmRight : btmLeft) + transform.up, transform.right * direction, 1f, ~(1 << 10));
		if (hit3.collider != null && hit3.collider.tag == "Obstacle") {
			SnapTo (hit3.transform, hit3.point, hit3.normal);
			Debug.DrawRay (hit3.point, hit3.normal,  Color.green, 3);
		}*/
		snapDown = (this.transform.up.y > 0f);
	}

	void OnCollisionEnter2D(Collision2D col) {
		if (col.gameObject.tag == "Obstacle") {
			ContactPoint2D[] contacts = new ContactPoint2D[10];
			col.GetContacts (contacts);
			bool tooTall = false;
			foreach (ContactPoint2D c in contacts) {
				Vector3 center = this.GetComponent<Collider2D>().bounds.center;
				if (transform.InverseTransformPoint(c.point).y > transform.InverseTransformPoint(center).y && c.point != new Vector2(0,0)) {
					tooTall = snapDown;
				}
			}
			if (tooTall) {
				traveled = 0;
				direction *= -1;
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
		/*Rigidbody2D rb = this.GetComponent<Rigidbody2D> ();
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
			}
		}*/
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
		Vector3 dir = player.transform.position - this.transform.position;
		script.direction = direction;
		script.source = this.gameObject;
		bullet.transform.position = this.transform.position;
		//bullet.transform.position += Vector3.right * this.transform.localScale.x * direction;
		bullet.transform.rotation = Quaternion.FromToRotation (transform.right * direction, dir) 
			* bullet.transform.rotation;
		shot = true;
	}
}
