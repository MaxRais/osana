using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {
	public GameObject player; 
	public GameObject returnPoint;
	public float speed;
	public float maxDistance;
	public float minDistance;
	public Vector3 startPos;

	private bool alertedFollow = false;
	private bool alertedLatch = false;

	public bool latched = false;
	private bool hit = false;
	private float[] forces;
	private GameObject[] enemyList;
	private GameObject target;
	private Transform mapParent;
	// Use this for initialization
	void Start () {
		
		startPos = this.transform.position;
		player = GameObject.Find("Player");
		forces = new float[]{-100, 100};
		enemyList = GameObject.FindGameObjectsWithTag ("Enemy");
		returnPoint = new GameObject ();
		returnPoint.transform.position = transform.position;
		returnPoint.transform.SetParent (transform.parent);
		mapParent = this.transform.parent;
	}

	public void Restart(){
		StopAllCoroutines ();
		ResetParent ();

		this.transform.position = startPos;
		player = GameObject.Find("Player");
		forces = new float[]{-100, 100};
		enemyList = GameObject.FindGameObjectsWithTag ("Enemy");
		returnPoint.transform.position = startPos;
		returnPoint.transform.SetParent (transform.parent);
		mapParent = this.transform.parent;

	}
	
	// Update is called once per frame
	void Update () {


		target = player;
		if (enemyList.Length > 0) {
			float closest = Vector3.Distance (enemyList [0].transform.position, this.transform.position);
			foreach (GameObject o in enemyList) {
				if (o != null && Vector3.Distance (o.transform.position, this.transform.position) < Vector3.Distance (player.transform.position, this.transform.position) && Vector3.Distance (o.transform.position, this.transform.position) <= closest) {
					target = o;
					closest = Vector3.Distance (o.transform.position, this.transform.position);
				}
			}
		}

		transform.LookAt(target.transform);
		transform.Rotate(new Vector3(0,-90,0));

		//transform.eulerAngles = new Vector3 (transform.eulerAngles.x, 0, transform.eulerAngles.z);

		if (Vector3.Distance (this.transform.position, target.transform.position) < maxDistance && Vector3.Distance (this.transform.position, target.transform.position) > minDistance) {
			
			if (!alertedFollow) {
				//DisplayMessage.ins.showMessage ("you are being followed");
				alertedFollow = true;

			}
			//speed = 3f;
		} else if (Vector3.Distance (this.transform.position, target.transform.position) > maxDistance){
			alertedFollow = false;
			if (this.gameObject.transform.position == returnPoint.transform.position) {
				transform.LookAt (returnPoint.transform.position - this.transform.position);
				transform.Rotate (new Vector3 (0, -90, 0));
				this.transform.Translate (new Vector3 (speed * Time.deltaTime, 0, 0));
			}

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
	
	}

	IEnumerator eject(GameObject target){
		if (target != null) {
			if (!hit && target.tag != "Enemy") {
				player.gameObject.GetComponent<Player> ().TakeDamage (1, this.gameObject.transform.right);
				hit = true;
			}
			if (!hit && target.tag == "Enemy") {
				target.gameObject.GetComponent<Enemy> ().TakeDamage (1);
				hit = true;
			}
			yield return new WaitForSeconds (Random.Range (1.75f, 3.5f));
			this.gameObject.GetComponent<CircleCollider2D> ().enabled = true;
			this.gameObject.transform.SetParent (mapParent);
			transform.Translate (new Vector3 (-speed * Time.deltaTime, 0, 0));
			yield return new WaitForSeconds (Random.Range (3f, 4.5f));
			if (target == null) {
				yield break;
			}
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

	public void StopCoroutines(){
		StopAllCoroutines();
	}

	public void ResetParent() {
		transform.SetParent (mapParent);
	}

}
