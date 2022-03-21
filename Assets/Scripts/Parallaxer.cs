using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxer : MonoBehaviour {

	class PoolObject{

		public Transform transform;
		public PoolObject(Transform t){transform = t;}

		public bool inUse;
		public void Use(){
			inUse = true;
		}
		public void Dispose(){
			inUse = false;
		}
	}

	//**********************************************************

	[System.Serializable]
	public struct YSpawnRange{
		public float min;
		public float max;
	}

	public GameObject Prefabs;
	public int poolSize;
	public float shiftSpeed, spawnRate;
	public YSpawnRange ySpawnRange;

	public Vector3 defaultSpawnPos, immediateSpawnPos;
	public bool spawnImmeadiate;

	public Vector2 targetAspectRatio;
    float targetAspect;

	float spawnTimer;
	GameManager gameManager;

	PoolObject[] poolObjects;

	//***********************************************************************

	void Awake(){Configure ();}
	void Configure(){
		targetAspect = targetAspectRatio.x / targetAspectRatio.y;
		poolObjects = new PoolObject[poolSize];

		for (int i = 0; i < poolObjects.Length; i++) {
			GameObject go = Instantiate (Prefabs) as GameObject;
			Transform t = go.transform;
			t.SetParent (transform);
			t.position = Vector3.one * 1000;
			poolObjects [i] = new PoolObject (t);
		}

		if (spawnImmeadiate)
			SpawnImmeadiate ();
	}

	void OnEnable(){GameManager.OnGameoverConfirmed += OnGameoverConfirmed;}
	void OnDisable(){GameManager.OnGameoverConfirmed -= OnGameoverConfirmed;}
	void OnGameoverConfirmed ()
	{
		for (int i = 0; i < poolObjects.Length; i++) {
			poolObjects [i].Dispose ();
			poolObjects [i].transform.position = Vector3.one * 1000;
		}

		if (spawnImmeadiate)
			SpawnImmeadiate ();
	}

	void Start(){
		gameManager = GameManager.Instance;
	}

	void Update(){
		if (gameManager.GameOver)
			return;
		Shift ();

		spawnTimer += Time.deltaTime;
		if (spawnTimer > spawnRate) {
			Spawn ();
			spawnTimer = 0;
		}
	}

	void Shift(){
		for (int i = 0; i < poolObjects.Length; i++) {
			poolObjects [i].transform.localPosition += Vector3.left * shiftSpeed * Time.deltaTime;
			CheckDisposeObject (poolObjects [i]);
		}
	}

	void Spawn(){
		Transform t = GetPoolObject ();
		if (t == null)	return;
		t.position = new Vector3((defaultSpawnPos.x * Camera.main.aspect)/targetAspect, Random.Range(ySpawnRange.min, ySpawnRange.max),0);
	}

	void SpawnImmeadiate(){
		Transform t = GetPoolObject ();
		if (t == null)	return;
		t.position = new Vector3(immediateSpawnPos.x, Random.Range(ySpawnRange.min, ySpawnRange.max),0);

		Spawn ();
	}

	void CheckDisposeObject(PoolObject PO){
		if (PO.transform.position.x < -(defaultSpawnPos.x * Camera.main.aspect)/targetAspect) {
			PO.Dispose ();
			PO.transform.position = Vector3.one * 1000;
		}
	}

	Transform GetPoolObject(){
		for (int i = 0; i < poolObjects.Length; i++) {
			if (!poolObjects [i].inUse) {
				poolObjects [i].Use ();
				return poolObjects [i].transform;
			}
		}
		return null;
	}
}
