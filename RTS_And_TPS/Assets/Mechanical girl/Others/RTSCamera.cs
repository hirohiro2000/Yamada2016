using UnityEngine;
using System.Collections;

public class RTSCamera : MonoBehaviour 
{
	private GameObject  m_mechanicalGirl = null;

	[SerializeField, Range(5, 100)]
    public  float       m_targetDistance = 20.0f;

    public  Vector3     m_dir;

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
		Vector3 dir	= m_dir.normalized * m_targetDistance;

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
