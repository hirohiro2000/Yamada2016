using UnityEngine;
using System.Collections;

public class PhotoCamera : MonoBehaviour
{
	[SerializeField]
	private  Transform	m_target = null;
	
	[SerializeField]
    private  Vector3	m_dir;

    [SerializeField, Range(5, 100)]
    private  float		m_targetDistance = 20.0f;

	// Use this for initialization
	void Start ()
	{	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if( m_target == null )
		{
			transform.position = Vector3.zero + m_dir * m_targetDistance;
			transform.LookAt( Vector3.zero );
		}
		else
		{
			Vector3 dir = m_dir.normalized * m_targetDistance;
			transform.position = m_target.position + dir;
			transform.LookAt( transform.position - m_dir );
		}
	}
}
