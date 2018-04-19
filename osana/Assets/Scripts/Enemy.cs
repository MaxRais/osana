using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public float range;
	public float speed;
	protected float traveled;
	protected int direction;
	public float detectDistance;
	protected GameObject player;
	public int health;
	public float bulletSpeed;
	public GameObject bulletPrefab;
	public GameObject healthBar;
	protected bool shot;
	public bool snapDown = true;
	public float dmgBounceback = 2f;
	public float bounciness = 15f;
	public float shotDelayMin = 1f;
	public float shotDelayMax = 3f;
	protected float shotDelay;
	public int bumpDamage = 1;
	protected float maxHealth;
	protected float shotTimer;
	public LayerMask layerMask;
	public AudioClip deathSound;
	protected bool dead;
	private Rigidbody2D rb;
	private Transform platform;
	private Quaternion targetRot;
	private Vector3 targetPos;
	public float rotSpeed = 2f;
	private bool rotating = false;
	private bool climbing = false;

	// Use this for initialization
	protected virtual void Start () {
		maxHealth = health;
		traveled = 0;
		direction = 1;
		shotDelay = Random.Range (shotDelayMin, shotDelayMax);
		shot = false;
		player = GameObject.FindGameObjectWithTag ("Player");
		rb = this.GetComponent<Rigidbody2D> ();

		// Snap enemy to platform they are on
		RaycastHit2D hit = Physics2D.Raycast(transform.position, (snapDown ? Vector2.down : Vector2.up), 25f, layerMask);
		if (hit.collider != null && hit.collider.tag == "Obstacle") { 

			//Debug.DrawRay (hit.point, hit.normal, Color.red, 3f);
			SnapTo (hit.transform, hit.point, hit.normal);
		}
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		BoxCollider2D col = this.GetComponent<BoxCollider2D> ();
		healthBar.transform.localScale = new Vector3 ((health / maxHealth) , 0.25f, 0.35f);
		if(shot)
			shotTimer += Time.deltaTime;
		if (shotTimer >= shotDelay) {
			shot = false;
		}
		float top = col.offset.y + (col.size.y / 2f);
		float btm = col.offset.y - (col.size.y / 2f);
		float left = col.offset.x - (col.size.x / 2f);
		float right = col.offset.x + (col.size.x / 2f); 
		Vector3 btmMiddle = transform.TransformPoint (new Vector3 (0f, btm, 0f));
		Vector3 topMiddle = transform.TransformPoint (new Vector3 (0f, top, 0f));
		Vector3 btmRight = transform.TransformPoint(new Vector3(right, btm, 0f));
		Vector3 btmLeft = transform.TransformPoint(new Vector3(left, btm, 0f));
		Vector3 topRight = transform.TransformPoint(new Vector3(right, top, 0f));
		Vector3 topLeft = transform.TransformPoint(new Vector3(left, top, 0f));
		/*RaycastHit2D hit = Physics2D.Raycast((direction == 1 ? (snapDown ? btmRight : btmLeft) : (snapDown ? btmLeft : btmRight)) + (transform.up / 2f), -transform.up, 3f, layerMask);
		RaycastHit2D hit2 = Physics2D.Raycast ((direction == 1 ? (snapDown ? topRight : topLeft) : (snapDown ? topLeft : topRight)), transform.right * direction, 0.1f, layerMask);
		RaycastHit2D hit3 = Physics2D.Raycast((direction == 1 ? (snapDown ? btmRight : btmLeft) : (snapDown ? btmLeft : btmRight)) + (transform.up / 2f) , transform.right * direction, 3f, layerMask);
		Debug.DrawRay ((direction == 1 ? (snapDown ? btmRight : btmLeft) : (snapDown ? btmLeft : btmRight)) + (transform.up / 2f),-transform.up, Color.blue, 1f);*/
		/*RaycastHit2D hit = Physics2D.Raycast(topMiddle + transform.right * direction * speed * Time.deltaTime, -transform.up, col.size.y + 3, layerMask);
		RaycastHit2D hit2 = Physics2D.Raycast (transform.position, -transform.up, col.size.y, layerMask);
		Debug.DrawRay (topMiddle + transform.right * direction * speed * Time.deltaTime, -transform.up, Color.white, 1f);
		RaycastHit2D hit4 = Physics2D.Raycast(transform.position, transform.up, 1f, layerMask);
		if (hit2.collider != null && hit2.collider.tag == "Obstacle")
			targetRot = Quaternion.FromToRotation(transform.up, hit2.normal);

		transform.rotation = Quaternion.Lerp (transform.rotation, targetRot, Time.deltaTime * rotSpeed);
		if ((traveled >= range && range > 0) || hit.collider == null) {
			traveled = 0;
			direction *= -1;
		} //else if (hit.collider.gameObject != null && hit.collider.gameObject.name != platform.name && hit.transform.gameObject.layer == LayerMask.NameToLayer("Obstacle")) {
			
			//SnapTo (hit.transform, hit.point, hit.normal);
			//RaycastHit2D = Physics2D.Raycast ();
			/*else {
				float diff = (hit.collider.offset.y + (hit.transform.gameObject.GetComponent<BoxCollider2D>().size.y / 2f)) - hit.transform.InverseTransformPoint (hit.point).y;
				if(!snapDown)
					diff = (hit.collider.offset.y - (hit.transform.gameObject.GetComponent<BoxCollider2D>().size.y / 2f)) - hit.transform.InverseTransformPoint (hit.point).y;
				if (diff < 0.001) {
					traveled += Vector3.Magnitude (transform.right * speed * Time.deltaTime * direction);
					SnapTo (hit.transform, hit.point, hit.normal);
				}
			}*/
		//}
		/*if (hit3.collider != null && hit3.collider.tag == "Obstacle" && hit3.collider.gameObject.name != platform.name && (hit2.collider == null || hit2.distance > 0.1f)) {
			Vector3 point = new Vector3 (hit3.point.x, hit3.point.y, 0);
			SnapTo (hit3.transform, point + hit3.transform.up * col.size.y, hit3.normal);
			//Debug.DrawRay (hit3.point, hit3.normal, Color.green, 1f);
		}
		else if (hit.collider.gameObject != null && hit.collider.tag == "Obstacle") {
			Debug.Log ("HERE");
			if (platform == null || hit.collider.name != platform.name)
				platform = hit.transform;
			targetPos = (Vector3)hit.point + (snapDown ? platform.up : -platform.up) * col.size.y;
		}
		if (hit4.collider != null && (hit.collider == null || hit.distance > 1)) {
			SnapTo (hit4.transform, hit4.point, hit4.normal);
		}

		if (targetPos != null)
			transform.position = Vector3.Lerp (transform.position, targetPos, Time.deltaTime);
		else
			rb.velocity = transform.right * direction * speed;
		//Debug.Log (this.name +" : " +direction + " " + speed + " " + v);
		if (Quaternion.Angle (transform.rotation, targetRot) <= 1)
			rotating = false;*/
		//transform.position = Vector3.Lerp (transform.position, targetPos, Time.deltaTime * speed);
		//rb.velocity = v;
		/*if (climbing) {
			
			if (Vector3.Distance (transform.position, targetPos) < 1)
				climbing = false;
		} else {
		}*/

		checkForDeath ();
		if (player != null && Vector3.Distance (transform.position, player.transform.position) <= detectDistance) {
			//Debug.Log(this.name + " trying to shoot");
			shootProjectile ();
		}
	}

	protected virtual void SnapTo(Transform surface, Vector3 pos, Vector3 norm) {
		//Debug.DrawRay (pos, norm, Color.green, 1f);
		BoxCollider2D col = this.GetComponent<BoxCollider2D> ();
		//transform.parent = null;
		platform = surface;
		Quaternion rotAmt = (Quaternion.FromToRotation (transform.up, norm));
		//targetRot = rotAmt;
		//Debug.Log(gameObject.name + " : " + rotAmt.z);
		rotating = true;
		snapDown = (norm.y > 0);
		targetRot = (snapDown ? new Quaternion (0, 0, 0, 0) : new Quaternion (0, 0, 180, 0));
		//transform.rotation = Quaternion.FromToRotation (transform.up, norm) * transform.rotation;
		targetPos = pos + norm * col.size.y;
		//transform.SetParent (surface);
	}

	protected void checkForDeath() {
		if (health <= 0) {
			foreach (Transform t in transform)
				if (t.name.Contains ("whitebloodcell"))
					t.GetComponent<FollowPlayer> ().ResetParent ();
			GameObject manager = GameObject.Find ("GameManager");
			if(manager)
				manager.GetComponent<GameManager> ().AddKill ();
			if (dead) {
				return;
			}
			dead = true;
			this.GetComponent<Collider2D> ().enabled = false;
			StartCoroutine (Die ());
		}
	}

	protected IEnumerator Die() {
		this.speed = 0;
		this.detectDistance = 0;
		AudioSource player = this.GetComponent<AudioSource> ();
		player.Stop ();
		player.clip = deathSound;
		player.loop = false;
		player.volume = 1;
		player.Play ();
		while (player.isPlaying) {
			foreach (Transform t in transform)
				if (t.name.Contains ("whitebloodcell"))
					t.GetComponent<FollowPlayer> ().ResetParent ();
			
			Vector3 scale = this.transform.localScale;
			scale -= Vector3.one * Time.deltaTime;
			if (scale.x < 0)
				scale = Vector3.zero;
			this.transform.localScale = scale;
			yield return null;
		}
		Destroy (this.gameObject);
	}

	protected virtual void FixedUpdate() {
		BoxCollider2D col = this.GetComponent<BoxCollider2D> ();
		float top = col.offset.y + (col.size.y / 2f);
		float btm = col.offset.y - (col.size.y / 2f);
		float left = col.offset.x - (col.size.x / 2f);
		float right = col.offset.x + (col.size.x / 2f); 
		Vector3 btmMiddle = transform.TransformPoint (new Vector3 (0f, btm, 0f));
		Vector3 topMiddle = transform.TransformPoint (new Vector3 (0f, top, 0f));
		Vector3 btmRight = transform.TransformPoint(new Vector3(right, btm, 0f));
		Vector3 btmLeft = transform.TransformPoint(new Vector3(left, btm, 0f));
		Vector3 topRight = transform.TransformPoint(new Vector3(right, top, 0f));
		Vector3 topLeft = transform.TransformPoint(new Vector3(left, top, 0f));

		RaycastHit2D hit = Physics2D.Raycast (transform.position + (snapDown ? transform.right * direction : -transform.right * direction), (snapDown ? -Vector2.up : Vector2.up), col.size.y + 3, layerMask);
		RaycastHit2D hit2 = Physics2D.Raycast (transform.position, transform.right * direction, col.size.x + 1, layerMask);
		//Debug.DrawRay (transform.position + (snapDown ? transform.right * direction : -transform.right * direction), (snapDown ? -Vector2.up : Vector2.up), Color.red, 1f);
		if (hit.collider != null && hit.collider.tag == "Obstacle" && Mathf.Abs (hit.normal.x) > 0.1f) {
			rb.velocity = new Vector2 (rb.velocity.x - (hit.normal.x), rb.velocity.y);
			Vector3 pos = transform.position;
			pos.y += -hit.normal.x * Mathf.Abs (rb.velocity.x) * Time.deltaTime * (rb.velocity.x - hit.normal.x > 0 ? 1 : -1);
			transform.position = pos;
			if(hit.collider.name != platform.name)
				SnapTo (hit.transform, hit.point, hit.normal);
		} else if (hit.collider != null && hit.collider.tag == "Obstacle" && (platform == null || hit.collider.name != platform.name)) {
			SnapTo (hit.transform, hit.point, hit.normal);
		}
		if (hit.collider == null && hit2.collider != null) {
			SnapTo (hit2.transform, hit2.point, hit2.normal);
		}
		if (hit.collider == null || (hit2.collider != null && hit2.collider.tag == "Obstacle") || traveled > range) {
			traveled = 0;
			direction *= -1;
			//rb.velocity = Vector2.zero;
		}

		if (snapDown) {
			this.GetComponent<Rigidbody2D> ().gravityScale = 9.8f;
		} else {
			this.GetComponent<Rigidbody2D> ().gravityScale = -9.8f;
		}
		if (platform)
			rb.velocity = platform.right * direction * speed;
		else
			rb.velocity = (targetPos - transform.position) * direction * speed;
		traveled += Vector3.Magnitude (transform.right * speed * Time.deltaTime * direction);
		transform.rotation = Quaternion.Lerp (transform.rotation, targetRot, Time.deltaTime * rotSpeed);

	}

	protected void OnCollisionEnter2D(Collision2D col) {
		if (col.gameObject.tag == "Enemy") {
			traveled = 0;
			direction *= -1;
		}
		if (col.gameObject.tag == "Player") {
			Vector2 bumpDir = (col.transform.position - transform.position).normalized;
			col.gameObject.GetComponent<Player> ().TakeDamage (bumpDamage, bumpDir);
		}
	}

	public void TakeDamage(int amt) {
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

	protected IEnumerator Blink(int amt) {
		for (int i = 0; i < amt * 2; i++) {
			if (i % 2 == 0)
				this.GetComponentInChildren<SpriteRenderer> ().color = Color.red;
			else
				this.GetComponentInChildren<SpriteRenderer> ().color = Color.white;
			yield return new WaitForSeconds (0.1f/((float)amt));
		}

	}

	protected IEnumerator HitPause() {
		yield return new WaitForSeconds(0.75f);
	}

	public void shootProjectile () {
		if (shot)
			return;
		//Debug.Log (this.name + " firing");
		GameObject bullet = Instantiate (bulletPrefab) as GameObject;
		Bullet script = bullet.GetComponent<Bullet> ();
		script.speed = bulletSpeed;
		Vector3 dir = player.transform.position - this.transform.position;
		if (!snapDown)
			dir *= -1;
		script.direction = direction;
		script.source = this.gameObject;
		bullet.transform.position = this.transform.position;
		bullet.transform.parent = player.GetComponent<Player> ().environment;
		//bullet.transform.position += Vector3.right * this.transform.localScale.x * direction;
		bullet.transform.rotation = Quaternion.FromToRotation (transform.right * direction, dir) 
			* bullet.transform.rotation;
		shot = true;

		shotTimer = 0;
		shotDelay = Random.Range (shotDelayMin, shotDelayMax);
	}
}
