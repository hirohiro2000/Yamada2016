using UnityEngine;
using System.Collections;

public class TurretBullet : MonoBehaviour
{
	public	Vector3	m_direction;
	public	float	m_speed		= 1.0f;
	private float	m_lifespan	= 3.0f;

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.position += m_direction * m_speed * Time.deltaTime;

		//
		m_lifespan -= Time.deltaTime;
		if( m_lifespan < 0 )
			Destroy( gameObject );
	}

	void OnCollisionEnter( Collision collision )
	{
		Destroy( gameObject );
	}
}
