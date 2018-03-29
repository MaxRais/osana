using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAt : MonoBehaviour {

	public GameObject bulletPrefab;
	public float bulletSpeed;
	private GameObject player;
	private float shotTimer;
	private float currentDelay;
	public float delay1, delay2, delay3;
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		currentDelay = delay1;
		shotTimer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		shotTimer += Time.deltaTime;
		if (shotTimer > currentDelay) {
			shootProjectile ();
			if (currentDelay == delay1)
				currentDelay = delay2;
			else if (currentDelay == delay2)
				currentDelay = delay3;
			else if (currentDelay == delay3)
				currentDelay = delay1;
		}
	}

	public void shootProjectile () {
		//Debug.Log (this.name + " firing");
		GameObject bullet = Instantiate (bulletPrefab) as GameObject;
		Bullet script = bullet.GetComponent<Bullet> ();
		script.speed = bulletSpeed;
		Vector3 dir = player.transform.position - this.transform.position;
		script.direction = (int)Mathf.Sign(dir.x);
		if (player.transform.position.x < this.transform.position.x)
			script.direction = (int)Mathf.Sign (dir.x) * -1;
		script.source = this.gameObject;
		bullet.transform.position = this.transform.position;
		bullet.transform.parent = player.GetComponent<Player> ().environment;
		//bullet.transform.position += Vector3.right * this.transform.localScale.x * direction;

		bullet.transform.rotation = Quaternion.FromToRotation (transform.right, dir) 
			* bullet.transform.rotation;
		
		shotTimer = 0;

	}
}
