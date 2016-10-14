using UnityEngine;
using System.Collections;

public class TurretBullet : MonoBehaviour
{
	private	Vector3	m_direction;
	private	float	m_speed		= 1.0f;
	public float	m_lifespan	= 3.0f;

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.position	+= ( m_direction * m_speed * Time.deltaTime );
		
		//
		m_lifespan -= Time.deltaTime;
		if( m_lifespan < 0 )
			Destroy( gameObject );
	}

	public void Set( Vector3 dir, float speed )
	{
		m_direction = dir;
		m_speed		= speed;
	}

	void OnCollisionEnter( Collision collision )
	{
		Destroy( gameObject );
	}
}
