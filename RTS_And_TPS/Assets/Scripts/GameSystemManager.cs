using UnityEngine;
using System.Collections;

public class GameSystemManager : MonoBehaviour {

	bool isGameOver;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public bool IsGameWaiting()
	{
		return false;
	}

	public bool IsGameCountdown()
	{
		return false;
	}

	public bool IsGamePlaying()
	{
		return (isGameOver == false);
	}

	public bool IsGameOver()
	{
		return (isGameOver == true);
	}

	public bool IsGameResult()
	{
		return false;
	}



	public void BeginGameOver()
	{
		isGameOver = true;
	}
}
