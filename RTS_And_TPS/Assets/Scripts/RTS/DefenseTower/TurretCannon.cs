using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class TurretCannon : MonoBehaviour
{
	public float				m_bulletSpeed		= 1.0f;
	public float				m_chaseSpeed		= 1.0f;

	private ReferenceWrapper	m_rEnemyShell   = null;
	private Transform		    m_target        = null;

	// Use this for initialization
	void Start ()
	{
		m_rEnemyShell   =   GameObject.Find( "EnemySpawnRoot" ).GetComponent< ReferenceWrapper >();
		m_target		=	m_rEnemyShell.GetNearEnemyTransform( transform.position, float.MaxValue );

		if( m_target != null )
		{
			if( m_target.FindChild("Eye") )
			{
				m_target = m_target.FindChild("Eye");
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		//	対象がないと消す
		if( m_target == null )
		{
			Destroy( gameObject );
			return;
		}

		//	跳ぶ
		Vector3 vec = m_target.position - transform.position;
		vec.Normalize();

		transform.rotation	= Quaternion.Slerp ( transform.rotation, Quaternion.LookRotation( vec ), m_chaseSpeed*Time.deltaTime );
		transform.position	+= ( transform.forward * m_bulletSpeed * Time.deltaTime );		
	}
}
