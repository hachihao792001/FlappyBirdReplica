using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreScript : MonoBehaviour {

	Text highscoreText;

	void OnEnable(){
		highscoreText = GetComponent<Text> ();
		highscoreText.text = "High Score : " + PlayerPrefs.GetInt ("HighScore").ToString ();
	}
}
