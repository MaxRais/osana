using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    public float maxJumpHeight = 4f;
    public float minJumpHeight = 1f;
    public float timeToJumpApex = .35f;
    private float accelerationTimeAirborne = .2f;
    private float accelerationTimeGrounded = .1f;
    public float moveSpeed = 6f;
	public float dashSpeed = 20f;
	public Sprite osanaLeft, osanaRight;

    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;

    public bool canDoubleJump;
	private bool isJumping = false;
    private bool isDoubleJumping = false;
	private bool facingRight = true;
	public bool sliding = false;
	private float xScale;
	private bool bounced = false;

    public float wallSlideSpeedMax = 3f;
    public float wallStickTime = .25f;
    private float timeToWallUnstick;

    private float gravity;
    private float maxJumpVelocity;
    private float minJumpVelocity;
    private Vector3 velocity;
    private float velocityXSmoothing;
	public LayerMask layerMask;

    private Controller2D controller;
	private Animator animator;

	public AudioSource sfx;
	public AudioClip jumpClip;
	public AudioClip walkClip;
	public AudioClip dashClip;

    private Vector2 directionalInput;
    private bool wallSliding;
    private int wallDirX;

	public Transform deathMarker;
	public Transform spawnPoint;
	public Transform environment;

	public float aimSensitivity;
	private float aimHeight;
	public float bulletSpeed;
	public float shotDelay = 0.75f;
	private float shotTimer;
	private bool shot = false;
	public GameObject bulletPrefab;
	public float health;
	private float startHealth;
	public float dashCooldownTimer;
	private bool dashCooldown;
	private bool dead;
	private float dashTimer;
	private GameObject recharge;
	public GameObject healthBar;
	public float bounceHeight;
	public float bounceDist;

    private void Start()
    {
		dashCooldownTimer = 1f;
		dashCooldown = false;
		dead = false;
		dashTimer = 0;
		recharge = GameObject.Find ("RechargeBar");
        controller = GetComponent<Controller2D>();
		animator = this.GetComponent<Animator> ();
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

		startHealth = health;
		GameObject spawnObject = GameObject.Find ("SpawnMarker");
		if (spawnObject) {
			spawnPoint = GameObject.Find ("SpawnMarker").transform;
		}
		//spawnPoint.position = this.transform.position + transform.up;
    }

	public void restart() {
		this.health = startHealth;
		this.healthBar.GetComponent<SpriteRenderer> ().enabled = true;
		this.transform.position = spawnPoint.position;
		foreach (GameObject wbc in GameObject.FindGameObjectsWithTag("WBC")) {
			FollowPlayer fb = wbc.GetComponent<FollowPlayer> ();
			wbc.transform.position = fb.startPos;
			fb.StopCoroutines ();
			fb.ResetParent ();
		}

		DisplayMessage.ins.clearQueue ();
		dead = false;
		animator.SetBool ("dead", false);
	}

	IEnumerator Die(float sec) {	

		this.healthBar.GetComponent<SpriteRenderer> ().enabled = false;
		animator.SetBool ("dead", true);
		//dead = true;
		yield return new WaitForSeconds(sec);
		restart();
	}

	public void updateSpawnPoint(Transform newPos) {
		spawnPoint = newPos;
	}

	public bool Jumping() {
		return isJumping;
	}

    private void Update()
	{
		if (health > startHealth)
			health = startHealth;
		healthBar.transform.localScale = new Vector3 ((health / startHealth) * 3f, 0.25f, 0.35f);

		shotTimer += Time.deltaTime;
		if (shotTimer >= shotDelay) {
			shotTimer = 0;
			shot = false;
		}
		CalculateVelocity ();
		HandleWallSliding ();

		RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector3.up, 2f, layerMask);
		if (hit.collider != null && hit.transform.gameObject.layer == LayerMask.NameToLayer ("Obstacle")) {
			float distanceToGround = hit.distance;
		}
		if (controller.collisions.below && isJumping && velocity.y < 0) {
			isJumping = false;
		}

		if (dashCooldown) {
			recharge.GetComponent<SpriteRenderer> ().enabled = true;
			dashTimer += Time.deltaTime;
			recharge.transform.localScale = new Vector3 (1 - (dashTimer / dashCooldownTimer) + 0.3f, 0.25f, 0.35f);
			if (dashTimer >= dashCooldownTimer) {
				dashTimer = 0;
				dashCooldown = false;
				recharge.GetComponent<SpriteRenderer> ().enabled = false;
			}
		}
		controller.Move (velocity * Time.deltaTime, directionalInput);
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0f;

			if (controller.collisions.below) {
				animator.SetBool ("jumping", false);
			}
        }

		if (Input.GetKey(KeyCode.R))
		{
			restart ();
		}

		if (Input.GetKey (KeyCode.A)) {
			SetDirectionalInput (Vector2.left);
		}
		if (Input.GetKey (KeyCode.D)) {
			SetDirectionalInput (Vector2.right);
		}

		checkForDead ();
    }

	void FixedUpdate() {
		Rigidbody2D rb = this.GetComponent<Rigidbody2D> ();
		if (velocity.y < 0) {
			RaycastHit2D hit = Physics2D.Raycast (transform.position, Vector2.down, 2f, layerMask);
			if (hit.collider != null && hit.transform.gameObject.tag == "Obstacle") {
				//transform.position = hit.point + Vector2.up;
				rb.constraints = (RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY);
				rb.velocity = new Vector2 (0, 0);
			}
		}
	}

	public void TakeDamage(int amt, Vector2 dir) {
		this.healthBar.GetComponent<SpriteRenderer> ().enabled = true;
		if (!dead) {
			this.health -= amt;
			ApplyPush (amt, dir);
			StartCoroutine (Blink (amt));
		}
	}

	void ApplyPush(int amt, Vector2 dir) {
		Rigidbody2D rb = this.GetComponent<Rigidbody2D> ();
		rb.constraints = (RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY);
		dir.y += 0.5f;
		rb.velocity = (dir * amt * 10f);
	}

	void OnCollisionStay2D(Collision2D c) {
		if (c.gameObject.tag == "Obstacle") {
			Rigidbody2D rb = this.GetComponent<Rigidbody2D> ();
			rb.constraints = (RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY);
			rb.velocity = new Vector2 (0, 0);
		}
		if (c.gameObject.tag == "Crushing") {
			Vector3 contact = c.contacts [0].point;
			if ((controller.collisions.below && contact.y > transform.position.y) || (controller.collisions.above && contact.y < transform.position.y))
				restart ();
		}
	}
	void OnCollisionEnter2D(Collision2D c) {
		if (c.gameObject.tag == "Bounce") {
			isDoubleJumping = false;
			isJumping = true;
			Vector2 dir = c.contacts [0].point - new Vector2 (transform.position.x, transform.position.y);
			dir = -dir.normalized;

			dir.x = dir.x * bounceDist;
			dir.y = bounceHeight;
			velocity = dir;

			if (!bounced) {
				DisplayMessage.ins.showMessage ("These alveoli are bouncy.");
				bounced = true;
			}
		}
	}

	IEnumerator Blink(int amt) {
		for (int i = 0; i < amt * 2; i++) {
			this.directionalInput.x = 0;
			if (i % 2 == 0)
				this.GetComponentInChildren<SpriteRenderer> ().color = Color.red;
			else
				this.GetComponentInChildren<SpriteRenderer> ().color = Color.white;
			yield return new WaitForSeconds (0.1f/((float)amt));
		}
	}

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
		if (input.x == -1 && !dead) {
			facingRight = false;
			animator.SetInteger ("xDir", -1);
			this.GetComponent<SpriteRenderer> ().flipX = true;
			if (controller.collisions.below) {
				playWalkSound();
			}
		} else if (input.x == 1 && !dead) {
			facingRight = true;
			animator.SetInteger ("xDir", 1);
			this.GetComponent<SpriteRenderer> ().flipX = false;
			if (controller.collisions.below) {
				playWalkSound();
			}
		} else if (input.x == 0) {
			animator.SetInteger ("xDir", 0);
		}
		animator.SetBool ("sliding", sliding);
    }

    public void OnJumpInputDown()
    {
		if (!dead) {
			animator.SetInteger ("xDir", 0);
			animator.SetBool ("jumping", true);
			if (wallSliding) {
				if (wallDirX == directionalInput.x) {
					velocity.x = -wallDirX * wallJumpClimb.x;
					velocity.y = wallJumpClimb.y;
					directionalInput.x *= -1;
				} else if (directionalInput.x == 0) {
					velocity.x = -wallDirX * wallJumpOff.x;
					velocity.y = wallJumpOff.y;
				} else {
					velocity.x = -wallDirX * wallLeap.x;
					velocity.y = wallLeap.y;
				}
				isDoubleJumping = false;
				playJumpSound();
			}
			if (controller.collisions.below || Physics2D.Raycast(this.transform.position, -transform.up).collider.tag == "Obstacle" && Vector2.Distance(Physics2D.Raycast(this.transform.position, -transform.up).collider.transform.position, transform.position) < 1) {
				velocity.y = maxJumpVelocity;
				isJumping = true;
				isDoubleJumping = false;
				playJumpSound ();
			}
			if (canDoubleJump && !controller.collisions.below && !isDoubleJumping && !wallSliding) {
				velocity.y = maxJumpVelocity;
				isDoubleJumping = true;
				animator.SetBool ("jumping", false);
				animator.SetBool ("jumping", true);
				playJumpSound ();
			}
		}
    }

    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

	public void Dash() {
		if (!dashCooldown) {
			playDashSound();
			this.SetDirectionalInput (new Vector2 (dashSpeed * (facingRight ? 1 : -1), directionalInput.y));
			dashCooldown = true;
		}
	}
	public void Slide() {
		if (!sliding && directionalInput.x != 0) {
			this.SetDirectionalInput (new Vector2 (5 * (facingRight ? 1 : -1), directionalInput.y));
			sliding = true;
			animator.SetBool ("sliding", sliding);
		}
	}
	public void ShootProjectile()
	{
		if (shot || dead)
			return;

		GameObject bullet = Instantiate (bulletPrefab) as GameObject;
		bullet.transform.position = this.transform.position;
		Vector2 dir = directionalInput;//new Vector2 (0f, 0f);
		/*if (directionalInput.x < 0 && directionalInput.y > 0)
			dir.Set (-0.5f, (isJumping ? 0.5f : 0.25f));
		if (directionalInput.x > 0 && directionalInput.y > 0)
			dir.Set (0.5f, (isJumping ? 0.5f : 0.25f));
		if (directionalInput.x < 0 && directionalInput.y < 0)
			dir.Set (-0.5f, (isJumping ? -0.5f : -0.25f));
		if (directionalInput.x > 0 && directionalInput.y < 0)
			dir.Set (0.5f, (isJumping ? -0.5f : -0.25f));
		if (directionalInput.x == 0 && directionalInput.y > 0)
			dir.Set (0, 1);
		if (directionalInput.x == 0 && directionalInput.y < 0)
			dir.Set (0, -1);*/
		bullet.transform.rotation = Quaternion.FromToRotation ((facingRight ? transform.right : -transform.right), dir) 
			* bullet.transform.rotation;
		
		Bullet script = bullet.GetComponent<Bullet> ();
		script.speed = bulletSpeed;
		script.direction = facingRight ? 1 : -1;
		bullet.transform.position += new Vector3(dir.x * 1.5f, dir.y * 1.5f, 0);
		bullet.transform.parent = environment;
		script.source = this.gameObject;
		shot = true;
	}

    private void HandleWallSliding()
    {
        wallDirX = (controller.collisions.left) ? -1 : 1;
        wallSliding = false;
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
        {
            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0f)
            {
                velocityXSmoothing = 0f;
                velocity.x = 0f;
                if (directionalInput.x != wallDirX && directionalInput.x != 0f)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }
    }

    private void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
		//if (velocity.x < 0 && directionalInput.x > 0 || velocity.x > 0 && directionalInput.x < 0)
			//velocity.x = 0;
		if (dead) {
			targetVelocityX = 0;
		}
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne));
        velocity.y += gravity * Time.deltaTime;

    }

	private void checkForDead() {
		if (deathMarker && this.transform.position.y < deathMarker.position.y) {
			restart ();
		} else if (this.health <= 0) {
			dead = true;
			StartCoroutine (Die (3));
		}
	}

	private void playJumpSound() {
		sfx.pitch = 1.6f;
		sfx.volume = 1.0f;
		sfx.clip = jumpClip;
		sfx.Play ();
	}

	private void playDashSound() {
		sfx.pitch = 1.0f;
		sfx.volume = 0.25f;
		sfx.clip = dashClip;
		sfx.Play ();
	}

	private void playWalkSound() {
		PlatformAudio p = controller.platform.GetComponent<PlatformAudio>();
		if (p) {
			controller.platform.GetComponent<PlatformAudio>().playWalkSound();
		}
	}
}
