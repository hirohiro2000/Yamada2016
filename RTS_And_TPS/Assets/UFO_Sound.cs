using UnityEngine;
using System.Collections;

public class UFO_Sound : MonoBehaviour {

	SoundController StartSE;
	SoundController RunningSE;
	// Use this for initialization
	void Start () {
		StartSE = SoundController.Create("EnemySound_Fly_Start",transform);
		RunningSE = SoundController.Create("EnemySound_Fly2",transform);
		StartSE.transform.localPosition = Vector3.zero;
		RunningSE.transform.localPosition = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Play()
	{
		StartSE.Play();
		RunningSE.Play();
	}
}
