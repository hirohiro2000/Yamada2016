using UnityEngine;
using System.Collections;

public class TurretBullet : MonoBehaviour
{
	private	float		m_speed			= 1.0f;
	public float		m_lifespan		= 3.0f;
	public float		m_chaseSpeed	= 1.0f;

	private EnemyFactory	m_factory;
	private Transform		m_target;

	// Use this for initialization
	void Start ()
	{
		m_factory = GameObject.Find("EnemyFactory").GetComponent<EnemyFactory>();
		m_factory.GetNearEnemyTransform( ref m_target, transform.position, float.MaxValue );
	}
	
	// Update is called once per frame
	void Update ()
	{
		if( m_target == null )
		{
			Destroy( gameObject );
			return;
		}


		//
		Vector3 vec = m_target.position - transform.position;
		vec.Normalize();

		transform.rotation = Quaternion.Slerp ( transform.rotation, Quaternion.LookRotation( vec ), m_chaseSpeed*Time.deltaTime );
		transform.position	+= ( transform.forward * m_speed * Time.deltaTime );


		//
		m_lifespan -= Time.deltaTime;
		if( m_lifespan < 0 )
			Destroy( gameObject );
	}

	public void Set( Quaternion _Rotation, float speed )
	{
		transform.rotation	= _Rotation;
		m_speed				= speed;
	}

	void OnCollisionEnter( Collision collision )
	{
		Destroy( gameObject );
	}
	void OnTriggerEnter( Collider collision )
	{
		Destroy( gameObject );
	}
}
