using UnityEngine;
using System.Collections;

public class CameraGroundStation : MonoBehaviour
{

	private GameObject  m_mechanicalGirl = null;
    
    public  float       m_targetDistance = 100.0f;

	// Use this for initialization
	void Start ()
	{
		m_mechanicalGirl = GameObject.Find ("MechanicalGirl");
	}
	
	// Update is called once per frame
	void Update () 
	{
		ChaseTarget ();
	}

	void ChaseTarget()
	{
		Vector3 dir	= Vector3.up * m_targetDistance;

		if( m_mechanicalGirl == null )
		{
			transform.position	= Vector3.zero + dir;
			transform.LookAt( Vector3.zero );
			return;
		}

		transform.position	= m_mechanicalGirl.transform.position + dir;
		transform.LookAt( m_mechanicalGirl.transform.position );
	}
}


