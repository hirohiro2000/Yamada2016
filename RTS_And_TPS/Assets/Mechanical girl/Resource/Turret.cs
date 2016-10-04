using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour
{
	public GameObject	m_bullet;
	public int			m_fireInterval		= 1;

	private int			m_timer				= 0;
	private Transform	m_bulletsParent;
	public GameObject	m_enemyFactory;

	// Use this for initialization
	void Start ()
	{
		m_bulletsParent = GameObject.Find("BulletsParent").transform;
		m_enemyFactory	= GameObject.Find("EnemyFactory");
	}
	
	// Update is called once per frame
	void Update ()
	{
		//	targetting
		{
			int		nearID	= -1;
			float	near	= float.MaxValue;

			for( int i=0; i<m_enemyFactory.transform.childCount; ++i )
			{
				Vector3 pos		= m_enemyFactory.transform.GetChild(i).transform.position;
				float	dist	= ( transform.position - pos ).sqrMagnitude;

				if( dist < near )
				{
					near	= dist;
					nearID	= i;
				}
			}

			if( nearID != -1 )
			{
				Vector3 pos		= m_enemyFactory.transform.GetChild( nearID ).transform.position;
				Vector3 forward = pos - transform.position;

				transform.rotation = Quaternion.Slerp ( transform.rotation, Quaternion.LookRotation( forward ), 0.1f );
			}
		}

		//	fire
		if( m_timer++ > m_fireInterval )
		{
			GameObject g			= Instantiate( m_bullet );
			g.transform.parent		= m_bulletsParent;
			g.transform.position	= transform.position;
			g.GetComponent<TurretBullet>().m_direction = transform.forward;
			m_timer = 0;
		}
	}
}
