using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {
	public GameObject player; 
	public float speed;
	public float maxDistance;
	public float minDistance;

	private bool alertedFollow = false;
	private bool alertedLatch = false;

	public bool latched = false;
	private bool hit = false;
	private float[] forces;
	private GameObject[] enemyList;
	private GameObject target;
	// Use this for initialization
	void Start () {
		player = GameObject.Find("Player");
		forces = new float[]{-100, 100};
		enemyList = GameObject.FindGameObjectsWithTag ("Enemy");
	}
	
	// Update is called once per frame
	void Update () {

		target = player;
		float closest = Vector3.Distance(enemyList[0].transform.position, this.transform.position);
		foreach (GameObject o in enemyList) {
			if (o != null && Vector3.Distance(o.transform.position, this.transform.position) < Vector3.Distance(player.transform.position, this.transform.position) && Vector3.Distance(o.transform.position, this.transform.position) <= closest) {
				target = o;
				closest = Vector3.Distance (o.transform.position, this.transform.position);
			}
		}

		transform.LookAt(target.transform);
		transform.Rotate(new Vector3(0,-90,0));
		if (Vector3.Distance (this.transform.position, target.transform.position) < maxDistance && Vector3.Distance (this.transform.position, target.transform.position) > minDistance) {
			
			if (!alertedFollow) {
				//DisplayMessage.ins.showMessage ("you are being followed");
				alertedFollow = true;
			}
			//speed = 3f;
		} else if (Vector3.Distance (this.transform.position, target.transform.position) > maxDistance){
			alertedFollow = false;

		} else if (Vector3.Distance (this.transform.position, target.transform.position) < minDistance){
			//speed = 0f;\
			latched = true;
			this.gameObject.GetComponent<CircleCollider2D>().enabled = false;
			this.gameObject.transform.parent = target.transform;
			//hit = false;
			StartCoroutine (eject(target));
		}
		if (!latched && alertedFollow){
			this.transform.Translate (new Vector3 (speed * Time.deltaTime, 0, 0));
		}
		/*
		if (Vector3.Distance (this.transform.position, player.transform.position) < minDistance && latched == false) {
			latched = true;
			//StartCoroutine (eject());
			//this.transform.position = player.transform.position;
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
			this.transform.Translate (new Vector3 (speed * 3 * Time.deltaTime, 0, 0));

		}*/
	}

	IEnumerator eject(GameObject target){
		if (target != null) {
			if (!hit && target.tag != "Enemy") {
				player.gameObject.GetComponent<Player> ().TakeDamage (1, this.gameObject.transform.right);
				hit = true;
			}
			if (!hit && target.tag == "Enemy") {
				target.gameObject.GetComponent<Enemy> ().TakeDamage (1, this.gameObject.transform.right);
				hit = true;
			}
			yield return new WaitForSeconds (Random.Range (1.75f, 3.5f));
			this.gameObject.GetComponent<CircleCollider2D> ().enabled = true;
			this.gameObject.transform.SetParent (null);
			transform.Translate (new Vector3 (-speed * Time.deltaTime, 0, 0));
			yield return new WaitForSeconds (Random.Range (3f, 4.5f));
			transform.LookAt (target.transform);
			transform.Rotate (new Vector3 (0, -90, 0));
			latched = false;
			StartCoroutine (DamageAgain (target));
		}
	}

	IEnumerator DamageAgain(GameObject target){
		StopCoroutine(eject(target));
		yield return new WaitForSeconds (4f);
		hit=false;
		StopAllCoroutines();


	}
}
