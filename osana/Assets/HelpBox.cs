using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpBox : MonoBehaviour {
	
	[TextArea]
	public string text;
	public GameObject panel;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
		
	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Player") {
			panel.SetActive (true);
			panel.GetComponentInChildren<Text> ().text = this.text;
		}
	}
	void OnTriggerExit2D(Collider2D other) {
		if (other.gameObject.tag == "Player") 
			panel.SetActive (false);
	}
}
