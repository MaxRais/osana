using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverEnemy : Enemy {

	public float minHoverHeight;
	public float maxHoverHeight;
	private bool flying;

	protected override void Start() {
		snapDown = true;
		base.Start ();
	}

	protected override void Update() {
		if (flying)
			return;

		BoxCollider2D col = this.GetComponent<BoxCollider2D> ();
		RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.right * speed * Time.deltaTime * direction, -transform.up, maxHoverHeight, layerMask);
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
			shootProjectile ();
		}
	}

	protected override void SnapTo(Transform surface, Vector3 pos, Vector3 normal) {
		BoxCollider2D col = this.GetComponent<BoxCollider2D> ();
		transform.parent = null;
		Vector3 oldPos = transform.position;
		//transform.position = pos + transform.up * col.size.y + Vector3.up * minHoverHeight;
		Vector3 endPosition = pos + transform.up * col.size.y + Vector3.up * minHoverHeight;
		StartCoroutine (flyTowards (endPosition));
	}

	private IEnumerator flyTowards(Vector3 position) {
		flying = true;
		if (transform.position.y - position.y >= minHoverHeight) {
			position.x += direction * 2f;
		}
		while (transform.position != position) {
			float step = speed * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, position, step);
			yield return null;
		}
		flying = false;
	}
}
