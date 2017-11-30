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
	public Sprite osanaLeft, osanaRight;

    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;

    public bool canDoubleJump;
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

    private Vector2 directionalInput;
    private bool wallSliding;
    private int wallDirX;

	public Transform deathMarker;
	public Transform spawnPoint;

	public float aimSensitivity;
	private float aimHeight;
	public float bulletSpeed;
	public GameObject bulletPrefab;
	public int health;

    private void Start()
    {
        controller = GetComponent<Controller2D>();
		animator = this.gameObject.transform.GetChild (0).GetComponent<Animator> ();
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
		//spawnPoint = GameObject.Find("SpawnMarker").transform;
		spawnPoint.position = this.transform.position;
    }

	public void restart() {
		this.health = 3;
		this.transform.position = spawnPoint.position;
		DisplayMessage.ins.clearQueue ();
	}

    private void Update()
    {
        CalculateVelocity();
        HandleWallSliding();

        controller.Move(velocity * Time.deltaTime, directionalInput);

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
		checkForWin ();
    }

	public void TakeDamage(int amt, Vector2 dir) {
		this.health -= amt;
		this.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (amt / 2, amt / 2) + dir + Vector2.up * 2);
	}

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
		if (input.x == -1) {
			//this.gameObject.transform.GetChild (0).GetComponent<SpriteRenderer> ().sprite = osanaLeft;
			facingRight = false;
			if (controller.collisions.below) {
				animator.SetInteger ("xDir", -1);
			}
			this.transform.localScale = new Vector3 (-1, 1, 1);
		} else if (input.x == 1) {
			//this.gameObject.transform.GetChild (0).GetComponent<SpriteRenderer> ().sprite = osanaRight;
			facingRight = true;
			if (controller.collisions.below) {
				animator.SetInteger ("xDir", 1);
			}
			this.transform.localScale = Vector3.one;
		} else if (input.y == 1) {
			Debug.Log ("Up");
		} else if (input.y == -1) {
			Debug.Log ("down");
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
        }
        if (controller.collisions.below)
        {
            velocity.y = maxJumpVelocity;
            isDoubleJumping = false;
        }
        if (canDoubleJump && !controller.collisions.below && !isDoubleJumping && !wallSliding)
        {
            velocity.y = maxJumpVelocity;
            isDoubleJumping = true;
			animator.SetBool ("jumping", false);
			animator.SetBool ("jumping", true);
        }
    }

    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

	public void ShootProjectile()
	{
		GameObject bullet = Instantiate (bulletPrefab) as GameObject;
		bullet.transform.position = this.transform.position;
		Vector2 dir = new Vector2 (0f, 0f);
		if (directionalInput.x < 0 && directionalInput.y > 0)
			dir.Set (-0.5f, 0.3f);
		if (directionalInput.x > 0 && directionalInput.y > 0)
			dir.Set (0.5f, 0.3f);
		if (directionalInput.x < 0 && directionalInput.y < 0)
			dir.Set (-0.5f, -0.3f);
		if (directionalInput.x > 0 && directionalInput.y < 0)
			dir.Set (0.5f, -0.3f);
		if (directionalInput.x == 0 && directionalInput.y > 0)
			dir.Set (0, 1);
		if (directionalInput.x == 0 && directionalInput.y < 0)
			dir.Set (0, -1);
		bullet.transform.rotation = Quaternion.FromToRotation ((facingRight ? transform.right : -transform.right), dir) 
			* bullet.transform.rotation;
		
		Bullet script = bullet.AddComponent<Bullet> ();
		script.speed = bulletSpeed;
		script.direction = facingRight ? 1 : -1;
		bullet.transform.position += new Vector3(dir.x * 1.5f, dir.y * 1.5f, 0);
		script.source = this.gameObject;
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
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne));
        velocity.y += gravity * Time.deltaTime;
    }

	private void checkForDead() {
		if (this.transform.position.y < deathMarker.position.y) {
			restart ();
		} else if (this.health <= 0) {
			restart ();
		}
	}

	private void checkForWin() {
		Transform winMarker = GameObject.FindGameObjectWithTag ("Finish").transform;
		if (Vector3.Distance (this.transform.position, winMarker.position) < 5f) {
			restart ();
		}
	}

	public void pushBack(float distance) {
		StartCoroutine (push (distance));
	}	

	IEnumerator push(float distance) {
		float curX = transform.position.x;
		float startX = curX;
		float endX = curX + distance * (facingRight ? -1 : 1);
		float speed = .5f;

		if (facingRight) {
			while (curX >= endX) {
				transform.position += Vector3.left * speed;
				curX = transform.position.x;
				yield return new WaitForEndOfFrame ();
			}
		} else {
			while (curX <= endX) {
				transform.position += Vector3.right * speed;
				curX = transform.position.x;
				yield return new WaitForEndOfFrame ();
			}
		}
	}
}
