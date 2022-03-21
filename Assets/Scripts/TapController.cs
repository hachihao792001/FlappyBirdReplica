using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TapController : MonoBehaviour {

	public AudioSource jumpAudio, diedAudio, scoreAudio;

	GameManager gameManager;

	public delegate void PlayerDelegate();
	public static event PlayerDelegate OnPlayerDied;
	public static event PlayerDelegate OnPlayerScored;

	Rigidbody2D rb;

	public Vector3 startPos;

	public float tapForce;
	public float rotationSmooth;

	Quaternion forwardRotation;
	Quaternion downRotation;

	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		rb.simulated = false;

		forwardRotation = Quaternion.Euler (new Vector3 (0, 0, 45));
		downRotation = Quaternion.Euler (new Vector3 (0, 0, -90));

		gameManager = GameManager.Instance;
	}

	void Update () {

		if (gameManager.GameOver)
			return;

		if (Input.GetMouseButtonDown (0)) {
			jumpAudio.Play ();
			transform.rotation = forwardRotation;
			rb.velocity = Vector3.zero;
			rb.AddForce (Vector2.up * tapForce, ForceMode2D.Force);
		}

		transform.rotation = Quaternion.Lerp (transform.rotation, downRotation, rotationSmooth * Time.deltaTime);
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "scoreZone") {
			scoreAudio.Play ();
			OnPlayerScored (); //sent to GameManager
		}

		if (other.tag == "deadZone") {
			diedAudio.Play ();
			rb.simulated = false;
			OnPlayerDied ();	//sent to GameManager
		}
	}

	//****************************Event*********************

	void OnEnable(){
		GameManager.OnGameStarted += OnGameStarted;
		GameManager.OnGameoverConfirmed += OnGameoverConfirmed;
	}

	void OnDisable(){
		GameManager.OnGameStarted -= OnGameStarted;
		GameManager.OnGameoverConfirmed -= OnGameoverConfirmed;
	}


	void OnGameStarted ()
	{
		//the continue of OnCountDownFinished, but these 2 line can only be use in TapController (which affect the bird)
		rb.velocity = Vector3.zero;
		rb.simulated = true;
	}

	void OnGameoverConfirmed(){
		//continue of ReplayButtonClicked
		transform.localPosition = startPos;
		transform.rotation = Quaternion.identity;
	}
}
