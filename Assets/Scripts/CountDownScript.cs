using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class CountDownScript : MonoBehaviour {

	public delegate void CountDownDelegate ();
	public static event CountDownDelegate OnCountDownFinished;

	Text countdownText;

	void OnEnable(){
		countdownText = GetComponent<Text> ();
		countdownText.text = "3";
		StartCoroutine (CountDown());
	}

	public IEnumerator CountDown(){
		int countdown = 3;
		for (int i = 0; i < countdown; i++) {
			countdownText.text = (countdown - i).ToString ();
			yield return new WaitForSeconds (1);
		}

		OnCountDownFinished ();		//sent to GameManager
	}
}
