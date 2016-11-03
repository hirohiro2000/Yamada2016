using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour
{
	public float m_speed = 0.0f;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.Rotate( new Vector3( 0, m_speed*Time.deltaTime, 0 ));
	}
}
