using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class ShotInitVelocity : MonoBehaviour {

	public Vector3 velocity;
	public bool isWorldSpace;

	// Use this for initialization
	void Awake () {
		Rigidbody body = GetComponent<Rigidbody>();

		if(isWorldSpace == false)//ローカル
		{
			Vector3 convert = transform.rotation * velocity;
			body.velocity = convert;
		}
		else
		{
			body.velocity = velocity;
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
