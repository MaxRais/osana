using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayMessage : MonoBehaviour {

	public static DisplayMessage ins;

	public float fadeTime = 5f;
	public float messageTimeToLive = 2f;
	private Text message;
	private bool showing;

	Queue<Message> messageQueue;

	// Use this for initialization
	void Start () {
		message = this.GetComponent<Text> ();
		messageQueue = new Queue<Message> ();
		if (!message) {
			Debug.LogError ("No text attached to display message");
		} else {
			setTextAlpha (0); 
		}
		ins = this;
	}

	public void showMessage(string text, float duration = 1f) {
		if(showing) {
			messageQueue.Enqueue (new Message(text, duration));
			return;
		}

		showing = true;
		if (!message) {
			Debug.LogWarning ("Can't show message");
		} else {
			StartCoroutine (fadeMessage (text, duration));
		}
	}

	public void clearQueue() {
		messageQueue.Clear ();
	}

	private IEnumerator fadeMessage(string text, float duration) {
		message.text = text;
		while (message.color.a < 1.0f) {
			setTextAlpha (message.color.a + Time.deltaTime / fadeTime);
			yield return null;
		}
		yield return new WaitForSeconds (duration);
		while (message.color.a > 0.0f)
		{
			setTextAlpha(message.color.a - Time.deltaTime / fadeTime);
			yield return null;
		}
		message.text = "";
		showing = false;
		while (messageQueue.Count > 0) {
			Message message1 = messageQueue.Dequeue ();
			/*if (message.waitTime > messageTimeToLive) {
				continue;
			}*/
			showMessage (message1.text, message1.duration);
		}
	}

	private void setTextAlpha(float alpha) {
		alpha = Mathf.Clamp01 (alpha);	
		message.color = new Color (1, 1, 1, alpha);
	}
	
	// Update is called once per frame
	void Update () {
		if (messageQueue != null) {
			foreach (Message message in messageQueue) {
				message.waitTime += Time.deltaTime;
			}
		}
	}
}
		
public class Message {
	public string text { get; private set; }
	public float duration { get; private set; }
	public float waitTime;

	public Message(string text, float duration) {
		this.text = text;
		this.duration = duration;
		this.waitTime = 0f;
	}
}