using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

	private Vector3 startPos;
	public float horizontalMovement = 0f;
	public float verticalMovement = 0f;
	public float speed = 1f;
	public float stopSecs = 0;
	private bool up, right, moving;
	private GameObject passenger;
	// Use this for initialization
	void Start () {
		up = true;
		right = true;
		moving = true;
		startPos = transform.position;
	}

	void OnCollisionEnter2D(Collision2D c) {
		if (c.gameObject.tag == "Player") {
			passenger = c.gameObject;
		}
	}
	void OnCollisionExit2D(Collision2D c) {
		if (c.gameObject.tag == "Player") {
			if(c.rigidbody.velocity.y > 0)
				passenger = null;
		}
	}

	// Update is called once per frame
	void Update () {
		if (moving) {
			if (right && Mathf.Abs (transform.position.x - startPos.x) < horizontalMovement) {
				transform.Translate (new Vector3 (speed * Time.deltaTime, 0, 0));
				if(passenger)
					passenger.transform.Translate (new Vector3 (speed * Time.deltaTime, 0, 0));
				if (Mathf.Abs (transform.position.x - startPos.x) >= horizontalMovement) {
					right = false;
					if(horizontalMovement > verticalMovement)
						StartCoroutine (Pause ());
				}
			}
			if (!right && Mathf.Abs (transform.position.x - startPos.x) >= 0) {
				transform.Translate (new Vector3 (-speed * Time.deltaTime, 0, 0));
				if(passenger)
					passenger.transform.Translate (new Vector3 (-speed * Time.deltaTime, 0, 0));
				if (transform.position.x - startPos.x <= 0) {
					right = true;
					if(horizontalMovement > verticalMovement)
						StartCoroutine (Pause ());
				}
			}
			if (up && Mathf.Abs (transform.position.y - startPos.y) < verticalMovement) {
				transform.Translate (new Vector3 (0, speed * Time.deltaTime, 0));
				if(passenger)
					passenger.transform.Translate (new Vector3 (0, speed * Time.deltaTime, 0));
				if (Mathf.Abs (transform.position.y - startPos.y) >= verticalMovement) {
					up = false;
					if(verticalMovement > horizontalMovement)
						StartCoroutine (Pause ());
				}
			}
			if (!up && Mathf.Abs (transform.position.y - startPos.y) >= 0) {
				transform.Translate (new Vector3 (0, -speed * Time.deltaTime, 0));
				if(passenger)
					passenger.transform.Translate (new Vector3 (0, -speed * Time.deltaTime, 0));
				if (transform.position.y - startPos.y <= 0) {
					up = true;
					if(verticalMovement > horizontalMovement)
						StartCoroutine (Pause ());
				}
			}
		}
	}

	IEnumerator Pause() {
		moving = false;
		yield return new WaitForSeconds (stopSecs);
		moving = true;
	}
}
