using UnityEngine;
using System.Collections;

public class DroneCamera : MonoBehaviour
{
	public Transform m_target = null;

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
		if( m_target == null )
			return;

		Vector3 forward = new Vector3( m_target.forward.x, 0.0f, m_target.forward.z ).normalized;
		Vector3 back	= forward*5.0f;
		Vector3 up		= new Vector3( 0, 5, 0 );

		transform.position = m_target.position - back + up;
		transform.LookAt( m_target.position );
	}
}
