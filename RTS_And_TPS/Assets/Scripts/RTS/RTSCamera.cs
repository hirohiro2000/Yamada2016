using UnityEngine;
using System.Collections;

public class RTSCamera : MonoBehaviour 
{
	public Transform	m_target = null;
    public  Vector3     m_dir;

	[SerializeField, Range(5, 100)]
    public  float       m_targetDistance = 20.0f;

	// Use this for initialization
	void Start ()
	{
	}

	
	// Update is called once per frame
	void Update () 
	{
		Vector3 dir	= m_dir.normalized * m_targetDistance;

		if( m_target == null )
		{
			transform.position	= Vector3.zero + dir;
			transform.LookAt( Vector3.zero );
			return;
		}

		transform.position	= m_target.position + dir;
		transform.LookAt( m_target.position );
	}
}
