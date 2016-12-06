using UnityEngine;
using System.Collections;

public class DroneController : MonoBehaviour
{
	Vector3	m_acceleration		= new Vector3( 0,0,0 );
	float	m_buoyancy			= 0.0f;

	[SerializeField]
	float	m_buoyancyUp		= 0.0f;

	[SerializeField]
	float	m_buoyancyDown		= 0.0f;

	[SerializeField]
	float	m_moveSpeed			= 100.0f;

	[SerializeField]
	float	m_rotateSpeed		= 1.0f;

	[SerializeField]
	float	m_orientationLimitAngle	= 25.0f;

	[SerializeField]
	private KeyCode m_flyKey = KeyCode.I;

	[SerializeField]
	private KeyCode m_rightKey = KeyCode.J;

	[SerializeField]
	private KeyCode m_leftKey = KeyCode.L;

	// Use this for initialization
	void Start ()
	{		
	}
	

	// Update is called once per frame
	void Update ()
	{
		Move();
		Rotate();
		Orientation();
		Hover();
		
		GetComponent<Rigidbody>().AddForce( m_acceleration, ForceMode.Acceleration );
		m_acceleration = Vector3.zero;
	}


	void Move()
	{
		float v = Input.GetAxis ("Vertical");
		float h = Input.GetAxis ("Horizontal");

		Vector3 cameraForward 	= Vector3.Scale( Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
		Vector3 direction 		= cameraForward * v + Camera.main.transform.right * h;

		direction.Normalize();

		float axis				= ( Mathf.Abs(v) > Mathf.Abs(h) )? Mathf.Abs(v) : Mathf.Abs(h);
		m_acceleration			+= direction * axis * m_moveSpeed;
	}
	void Rotate()
	{
		if( Input.GetKey( m_leftKey ))	transform.Rotate( transform.up, m_rotateSpeed );
		if( Input.GetKey( m_rightKey ))	transform.Rotate( transform.up, -m_rotateSpeed );
	}
	void Orientation()
	{ 
		float slerpT		= 0.02f;

		Vector3 axis		= new Vector3( Input.GetAxis ("Horizontal"), 0.0f, Input.GetAxis ("Vertical") );
		Vector3 vec			= new Vector3( axis.z, 0.0f, -axis.x );

		Quaternion curY		= new Quaternion( 0.0f, transform.rotation.y, 0.0f, transform.rotation.w );
		Quaternion target	= curY * Quaternion.AngleAxis( m_orientationLimitAngle, vec.normalized );
		transform.rotation	= Quaternion.Slerp( transform.rotation, target, slerpT );
	}
	void Hover()
	{
		m_buoyancy			+= Input.GetKey( m_flyKey )? m_buoyancyUp : -m_buoyancyDown;
		m_buoyancy			= Mathf.Abs( m_buoyancy );
		m_acceleration.y	= m_buoyancy;
	}
}
