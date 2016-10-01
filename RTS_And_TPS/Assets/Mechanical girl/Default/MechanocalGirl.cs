using UnityEngine;
using System.Collections;

public class MechanocalGirl : MonoBehaviour 
{
	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		IsMovedByKey ();
	}

	void IsMovedByKey()
	{		
		float v = Input.GetAxis ("Vertical");
		float h = Input.GetAxis ("Horizontal");

		Vector3 cameraForward 	= Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
		Vector3 direction 		= cameraForward * v + Camera.main.transform.right * h;
		direction.Normalize();

		float axis				= ( Mathf.Abs(v) > Mathf.Abs(h) )? Mathf.Abs(v) : Mathf.Abs(h);
		//axis					= Mathf.Clamp( axis, 0.5f, 1.0f );
		float moveSpeed 		= 30.0f;
		transform.position 		+= direction * axis * moveSpeed * Time.deltaTime;


		Vector3 animDir 		= direction;
		animDir.y 				= 0;

		if ( animDir.sqrMagnitude > 0.001f )
		{
			Vector3 newDir 		= Vector3.RotateTowards( transform.forward, animDir, 10.0f*Time.deltaTime, 0f );
			transform.rotation 	= Quaternion.LookRotation( newDir );
		}
	}
}