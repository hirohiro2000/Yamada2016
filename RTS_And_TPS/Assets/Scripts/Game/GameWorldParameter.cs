using UnityEngine;
using System.Collections;

public class GameWorldParameter : MonoBehaviour {

	public static GameWorldParameter instance = null;

	public float TPSPlayer_WalkSpeed = 7.0f;
	// Use this for initialization
	void Awake () {
		instance = this;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
