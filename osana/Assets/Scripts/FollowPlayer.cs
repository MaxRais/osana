using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {
	public GameObject player; 
	public float speed = 3f;
	public float maxDistance = 13f;
	public float minDistance = 1f;

	private bool alertedFollow = false;
	private bool alertedLatch = false;

	public bool latched = false;
	private float[] forces;
	// Use this for initialization
	void Start () {
		player = GameObject.Find("Player");
		forces = new float[]{-100, 100};
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Vector3.Distance (this.transform.position, player.transform.position) > 1f && Vector3.Distance (this.transform.position, player.transform.position) < maxDistance && latched == false) {
			transform.LookAt(player.transform);
			transform.Rotate(new Vector3(0,-90,0),Space.Self);
			if (!alertedFollow) {
				//DisplayMessage.ins.showMessage ("you are being followed");
				alertedFollow = true;
			}
			speed = 3f;
			this.transform.Translate (new Vector3 (speed * Time.deltaTime, 0, 0));
		} else {
			alertedFollow = false;
		}

		if (Vector3.Distance (this.transform.position, player.transform.position) < minDistance && latched == false) {
			latched = true;
			StartCoroutine (eject());
			this.transform.position = player.transform.position;
			if (!alertedLatch) {
				//DisplayMessage.ins.showMessage ("white blood cell has latched");
				alertedLatch = true;
			}
		} else {
			//latched = false;
			alertedLatch = false;
		}

		if (latched){
			////Vector3 p = player.transform.position - this.transform.position;
			//this.transform.Translate(p);

		}
	}

	IEnumerator eject(){
		latched = false;
		yield return new WaitForSeconds (.5f);
		this.gameObject.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(forces[Random.Range(0, forces.Length -1)], forces[Random.Range(0, forces.Length -1)]));
		StartCoroutine(kill());
	}

	IEnumerator kill(){
		yield return new WaitForSeconds (1f);
		Destroy(this.gameObject);

	}
}
