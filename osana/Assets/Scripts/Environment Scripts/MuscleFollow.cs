using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuscleFollow : MonoBehaviour {
	public Transform target;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position = target.position;
		//this.transform.position += Vector3.left * 210;
		//this.transform.position += Vector3.down * 25;
	}
}
