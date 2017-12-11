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
	private float xScale;

    public float wallSlideSpeedMax = 3f;
    public float wallStickTime = .25f;
    private float timeToWallUnstick;

    private float gravity;
    private float maxJumpVelocity;
    private float minJumpVelocity;
    private Vector3 velocity;
    private float velocityXSmoothing;

    private Controller2D controller;
	private Animator animator;

	public AudioClip jumpClip;
	public AudioClip walkClip;

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

    private void Start()
    {
		dashCooldownTimer = 1f;
		dashCooldown = false;
		dead = false;
		dashTimer = 0;
		recharge = GameObject.Find ("RechargeBar");
        controller = GetComponent<Controller2D>();
		animator = this.gameObject.transform.GetChild (0).GetComponent<Animator> ();
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
		//spawnPoint = GameObject.Find("SpawnMarker").transform;
		spawnPoint.position = this.transform.position;
		startHealth = health;
    }

	public void restart() {
		this.health = startHealth;
		this.transform.position = spawnPoint.position;
		DisplayMessage.ins.clearQueue ();
		dead = false;
		animator.SetBool ("dead", false);
	}

	IEnumerator Die() {
		if (dead)
			yield break;

		animator.SetBool ("dead", true);
		dead = true;
		yield return new WaitForSeconds(3);
		restart();
	}

	public void updateSpawnPoint(Transform newPos) {
		spawnPoint.position = newPos.position;
	}

	public bool Jumping() {
		return isJumping;
	}

    private void Update()
	{
		healthBar.transform.localScale = new Vector3 ((health / startHealth) * 3f, 0.25f, 0.35f);

		shotTimer += Time.deltaTime;
		if (shotTimer >= shotDelay) {
			shotTimer = 0;
			shot = false;
		}
		CalculateVelocity ();
		HandleWallSliding ();

		RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector3.up, 2f, ~(1 << 8));
		if (hit.collider != null && hit.collider.tag == "Obstacle") {
			float distanceToGround = hit.distance;
			if (distanceToGround < 1.23f)
				transform.Translate (new Vector3 (0, 1.23f - distanceToGround));
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
			RaycastHit2D hit = Physics2D.Raycast (transform.position, Vector2.down, 2f, ~(1 << 8));
			if (hit.collider != null && hit.collider.tag == "Obstacle") {
				transform.position = hit.point + Vector2.up;
				rb.constraints = (RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY);
				rb.velocity = new Vector2 (0, 0);
			}
		}
	}

	public void TakeDamage(int amt, Vector2 dir) {
		if (!dead) {
			this.health -= amt;
			ApplyPush (amt, dir);
			StartCoroutine (Blink (amt));
		}
	}

	void ApplyPush(int amt, Vector2 dir) {
		Rigidbody2D rb = this.GetComponent<Rigidbody2D> ();
		rb.constraints = RigidbodyConstraints2D.FreezeRotation;
		dir.y += 1f;
		rb.velocity = (dir * amt * 10f);
	}

	void OnCollisionEnter2D(Collision2D c) {
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
			//this.gameObject.transform.GetChild (0).GetComponent<SpriteRenderer> ().sprite = osanaLeft;
			facingRight = false;
			if (controller.collisions.below) {
				animator.SetInteger ("xDir", -1);
				SoundManager.ins.PlaySingle (walkClip);
			}
			this.transform.localScale = new Vector3 (-1, 1, 1);
		} else if (input.x == 1 && !dead) {
			//this.gameObject.transform.GetChild (0).GetComponent<SpriteRenderer> ().sprite = osanaRight;
			facingRight = true;
			if (controller.collisions.below) {
				animator.SetInteger ("xDir", 1);
				SoundManager.ins.PlaySingle (walkClip);
			}
			this.transform.localScale = Vector3.one;
		} else if (input.y == 1) {
			//Debug.Log ("Up");
		} else if (input.y == -1) {
			//Debug.Log ("down");
		} else if (input.x == 0) {
			animator.SetInteger ("xDir", 0);
		}
    }

    public void OnJumpInputDown()
    {
		animator.SetInteger ("xDir", 0);
		animator.SetBool ("jumping", true);
        if (wallSliding)
        {
            if (wallDirX == directionalInput.x)
            {
                velocity.x = -wallDirX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;
            }
            else if (directionalInput.x == 0)
            {
                velocity.x = -wallDirX * wallJumpOff.x;
                velocity.y = wallJumpOff.y;
            }
            else
            {
                velocity.x = -wallDirX * wallLeap.x;
                velocity.y = wallLeap.y;
            }
            isDoubleJumping = false;
			SoundManager.ins.PlaySingle (jumpClip);
        }
        if (controller.collisions.below)
        {
			velocity.y = maxJumpVelocity;
			isJumping = true;
			isDoubleJumping = false;
			SoundManager.ins.PlaySingle (jumpClip);
        }
        if (canDoubleJump && !controller.collisions.below && !isDoubleJumping && !wallSliding)
        {
            velocity.y = maxJumpVelocity;
            isDoubleJumping = true;
			animator.SetBool ("jumping", false);
			animator.SetBool ("jumping", true);
			SoundManager.ins.PlaySingle (jumpClip);
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
			this.SetDirectionalInput (new Vector2 (dashSpeed * (facingRight ? 1 : -1), directionalInput.y));
			dashCooldown = true;
		}
	}

	public void ShootProjectile()
	{
		if (shot)
			return;
		GameObject bullet = Instantiate (bulletPrefab) as GameObject;
		bullet.transform.position = this.transform.position;
		Vector2 dir = new Vector2 (0f, 0f);
		if (directionalInput.x < 0 && directionalInput.y > 0)
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
			dir.Set (0, -1);
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
		if (velocity.x < 0 && directionalInput.x > 0 || velocity.x > 0 && directionalInput.x < 0)
			velocity.x = 0;
		if (dead) {
			targetVelocityX = 0;
		}
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne));
        velocity.y += gravity * Time.deltaTime;
    }

	private void checkForDead() {
		if (this.transform.position.y < deathMarker.position.y) {
			restart ();
		} else if (this.health <= 0) {
			StartCoroutine (Die ());
		}
	}
}
