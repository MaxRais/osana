using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miniboss : MonoBehaviour {

	public GameObject bulletPrefab;
	public GameObject healthBar;
	public AudioClip deathSound;
	public float bulletSpeed;
	public int bulletDamage;
	public float health;
	private float maxHealth;
	private GameObject player;
	private float shotTimer;
	private float currentDelay;
	private bool shielded;
	private bool dead = false;
	public float delay1, delay2, delay3;
	// Use this for initialization
	void Start () {
		maxHealth = health;
		player = GameObject.FindGameObjectWithTag ("Player");
		currentDelay = delay1;
		shotTimer = 0;
		shielded = true;
	}
	
	// Update is called once per frame
	void Update () {
		healthBar.transform.localScale = new Vector3 ((health / maxHealth) , 0.25f, 0.35f);
		shotTimer += Time.deltaTime;
		if (shotTimer > currentDelay) {
			shootProjectile ();
			if (currentDelay == delay1)
				currentDelay = delay2;
			else if (currentDelay == delay2)
				currentDelay = delay3;
			else if (currentDelay == delay3)
				currentDelay = delay1;
		}
	}

	public void BreakShield() {
		shielded = false;
		healthBar.GetComponent<SpriteRenderer> ().color = Color.white;
	}

	public void TakeDamage(int amt) {
		if (!shielded) {
			Debug.Log (this.name + " taking " + amt + " dmg");
			this.health -= amt;
			StartCoroutine (Blink (amt));
			checkForDeath ();
		}
	}

	protected void checkForDeath() {
		if (health <= 0) {
			if (dead) {
				return;
			}
			dead = true;
			this.GetComponent<Collider2D> ().enabled = false;
			StartCoroutine (Die ());
		}
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

	protected IEnumerator Die() {
		AudioSource player = this.GetComponent<AudioSource> ();
		player.Stop ();
		player.clip = deathSound;
		player.loop = false;
		player.volume = 1;
		player.Play ();
		while (player.isPlaying) {
			Vector3 scale = this.transform.localScale;
			scale -= Vector3.one * Time.deltaTime;
			if (scale.x < 0)
				scale = Vector3.zero;
			this.transform.localScale = scale;
			yield return null;
		}
		GameObject.Find ("MinigameManager").GetComponent<Minigame> ().Win ();
		Destroy (this.gameObject);
	}

	public void shootProjectile () {
		//Debug.Log (this.name + " firing");
		GameObject bullet = Instantiate (bulletPrefab) as GameObject;
		Bullet script = bullet.GetComponent<Bullet> ();
		script.speed = bulletSpeed;
		script.damage = bulletDamage;
		Vector3 dir = player.transform.position - this.transform.position;
		script.direction = (int)Mathf.Sign(dir.x);
		if (player.transform.position.x < this.transform.position.x)
			script.direction = (int)Mathf.Sign (dir.x) * -1;
		script.source = this.gameObject;
		bullet.transform.position = this.transform.position;
		bullet.transform.parent = player.GetComponent<Player> ().environment;
		//bullet.transform.position += Vector3.right * this.transform.localScale.x * direction;

		bullet.transform.rotation = Quaternion.FromToRotation (transform.right, dir) 
			* bullet.transform.rotation;
		
		shotTimer = 0;

	}
}
