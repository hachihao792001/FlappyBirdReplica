using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	//This script's Instance, for other script use
	public static GameManager Instance;		void Awake(){Instance = this;}

	//variables

	public AudioSource clickAudio;

	public delegate void GameDelegate();
	public static event GameDelegate OnGameStarted;
	public static event GameDelegate OnGameoverConfirmed;

	int score;
	bool gameOver = true;		public bool GameOver{ get{return gameOver;} }
	public GameObject startPage, gameoverPage, countdownPage;
	public Text scoreText;


	//-------------------PageState Manager----------------------

	enum PageState{
		None,
		Start,
		Gameover,
		CountDown
	}
	void SetPageState(PageState state){
		switch (state) {
		case PageState.None: 
			startPage.SetActive (false);
			gameoverPage.SetActive (false);
			countdownPage.SetActive (false);
			break;
		case PageState.Start: 
			startPage.SetActive (true);
			gameoverPage.SetActive (false);
			countdownPage.SetActive (false);
			break;
		case PageState.Gameover: 
			startPage.SetActive (false);
			gameoverPage.SetActive (true);
			countdownPage.SetActive (false);
			break;
		case PageState.CountDown: 
			startPage.SetActive (false);
			gameoverPage.SetActive (false);
			countdownPage.SetActive (true);
			break;
		}
	}

	//******************PageState Manager**********************

	//---------------------Event Manager-------------------

	void OnEnable(){
		CountDownScript.OnCountDownFinished += OnCountDownFinished;
		TapController.OnPlayerDied += OnPlayerDied;
		TapController.OnPlayerScored += OnPlayerScored;
	}

	void OnDisable(){
		CountDownScript.OnCountDownFinished -= OnCountDownFinished;
		TapController.OnPlayerDied -= OnPlayerDied;
		TapController.OnPlayerScored -= OnPlayerScored;
	}

	void OnCountDownFinished(){
		SetPageState (PageState.None);
		score = 0;
		gameOver = false;
		OnGameStarted ();		//sent to TapController
	}

	void OnPlayerDied(){
		gameOver = true;
		SetPageState (PageState.Gameover);

		int savedScore = PlayerPrefs.GetInt ("HighScore");
		if (score > savedScore)
			PlayerPrefs.SetInt ("HighScore", score);
	}

	void OnPlayerScored(){
		score++;
		scoreText.text = score.ToString ();
	}

	//*********************Event Manager*******************

	//-----------------------Button Listener-----------------------

	public void PlayButtonClicked(){
		clickAudio.Play ();
		SetPageState (PageState.CountDown);
	}

	public void ReplayButtonClicked(){
		clickAudio.Play ();
		SetPageState (PageState.Start);
		OnGameoverConfirmed ();		//sent to TapController
		scoreText.text = "0";
	}

	public void ExitButtonClicked(){
		Application.Quit ();
	}

	//**********************Button Listener*********************
}
