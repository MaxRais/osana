using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour {

	public float zoomLevel = 10f;
	public float maxZoom = 100f;
	public float zoomSpeed = 1f;
	public float zoomAcceleration = 1f;

	private float currentZoomSpeed;
	private bool levelStarted = false;
	private bool zoomingIn = false;
	private bool zoomingOut = false;
	private bool stopZoom = false;

	private Camera cam;

	// Use this for initialization
	void Start () {
		currentZoomSpeed = zoomSpeed;
		cam = this.GetComponent<Camera> ();
		if (!cam) {
			Debug.LogError ("No camera attached to CameraZoom script");
		} else {
			cam.orthographicSize = maxZoom;
			StartCoroutine(zoomIn ());
		}
		
	}

	private IEnumerator zoomIn() {
		zoomingIn = true;
		stopZoom = false;
		while (cam.orthographicSize >= zoomLevel) {
			if (stopZoom) {
				zoomingIn = false;
				yield break;
			}
			cam.orthographicSize -= Time.deltaTime * currentZoomSpeed;
			currentZoomSpeed += zoomAcceleration;
			yield return 0;
		}
		currentZoomSpeed = zoomSpeed;
		levelStarted = true;
		zoomingIn = false;
	}

	private IEnumerator zoomOut() {
		zoomingOut = true;
		stopZoom = false;
		while (cam.orthographicSize <= maxZoom) {
			if (stopZoom) {
				zoomingOut = false;
				yield break;
			}
			cam.orthographicSize += Time.deltaTime * currentZoomSpeed;
			currentZoomSpeed += zoomAcceleration;
			yield return 0;
		}
		currentZoomSpeed = zoomSpeed;
		zoomingOut = false;
	}

	// Update is called once per frame
	void Update () {
		if (cam && levelStarted) {
			if (Input.GetKey (KeyCode.Z)) {
				if (zoomingIn) {
					stopZoom = true;
				}
				if (!zoomingOut) {
					StartCoroutine (zoomOut ());
				}
			} else {
				if(zoomingOut) {
					stopZoom = true;
				}
				if (!zoomingIn) {
					StartCoroutine (zoomIn ());
				}
			}
		}
	}
}
