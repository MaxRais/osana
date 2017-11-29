using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToGround : MonoBehaviour {

	public float sizeOffset = 5f;
	public bool snapDown = true;
	// Use this for initialization
	void Start () {
		if (snapDown) {
			RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 25f, ~(1 << 10));
			if (hit.collider != null) {
				if(Vector2.Distance(transform.position, hit.point) > sizeOffset)
					transform.position = hit.point + (hit.normal * sizeOffset);
				transform.rotation = Quaternion.FromToRotation (transform.up, hit.normal) * transform.rotation;
				transform.SetParent (hit.transform);
				//transform.Translate 
			}
		} else {
			RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 25f, ~(1 << 10));
			if (hit.collider != null) {
				if(Vector2.Distance(transform.position, hit.point) > sizeOffset)
					transform.position = hit.point + (hit.normal * sizeOffset);
				transform.rotation = Quaternion.FromToRotation (transform.up, hit.normal) * transform.rotation;
				transform.SetParent (hit.transform);
			}
		}
	}

	void FixedUpdate () {
	}
}
