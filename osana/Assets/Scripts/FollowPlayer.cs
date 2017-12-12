using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {
	public GameObject player; 
	public float speed = 3f;
	public float maxDistance = 13f;
	public float minDistance = 1.5f;

	private bool alertedFollow = false;
	private bool alertedLatch = false;

	public bool latched = false;
	private bool hit = false;
	private float[] forces;
	// Use this for initialization
	void Start () {
		player = GameObject.Find("Player");
		forces = new float[]{-100, 100};
	}
	
	// Update is called once per frame
	void Update () {
		transform.LookAt(player.transform);
		transform.Rotate(new Vector3(0,-90,0),Space.Self);
		if (Vector3.Distance (this.transform.position, player.transform.position) < maxDistance && Vector3.Distance (this.transform.position, player.transform.position) > minDistance) {
			
			if (!alertedFollow) {
				//DisplayMessage.ins.showMessage ("you are being followed");
				alertedFollow = true;
			}
			speed = 3f;
		} else if (Vector3.Distance (this.transform.position, player.transform.position) > maxDistance){
			alertedFollow = false;
		} else if (Vector3.Distance (this.transform.position, player.transform.position) < minDistance){
			//speed = 0f;\
			latched = true;
			this.gameObject.GetComponent<CircleCollider2D>().enabled = false;
			this.gameObject.transform.parent = player.transform;
			//hit = false;
			StartCoroutine (eject());
		}
		if (!latched){
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

	IEnumerator eject(){
		if (!hit){
			player.gameObject.GetComponent<Player>().TakeDamage(1, this.gameObject.transform.right);
			hit = true;
		}
		yield return new WaitForSeconds (Random.Range(1.75f, 3.5f));
		this.gameObject.GetComponent<CircleCollider2D>().enabled = true;
		this.gameObject.transform.SetParent(null);
		transform.Translate(new Vector3 (-speed * Time.deltaTime, 0, 0));
		yield return new WaitForSeconds (Random.Range(2.5f, 4f));
		transform.LookAt(player.transform);
		transform.Rotate(new Vector3(0,-90,0),Space.Self);
		latched = false;
		StartCoroutine(DamageAgain());
	}

	IEnumerator DamageAgain(){
		StopCoroutine(eject());
		yield return new WaitForSeconds (4f);
		hit=false;
		StopAllCoroutines();


	}
}
