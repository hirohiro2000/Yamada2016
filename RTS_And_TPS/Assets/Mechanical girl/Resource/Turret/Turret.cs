using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour
{
	public GameObject	m_bullet;
	public int			m_fireInterval		= 1;

	private int			m_initFireInterval	= 0;
	private int			m_timer				= 0;

	private CollisionParam m_param = null;

	// Use this for initialization
	void Start ()
	{
		m_initFireInterval	= m_fireInterval;
		m_param				= GetComponent<CollisionParam>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		//	fire
		if( m_timer++ > m_fireInterval )
		{
			GameObject g			= Instantiate( m_bullet );
			g.transform.position	= transform.position;
			g.GetComponent<TurretBullet>().m_direction = transform.forward;
			g.AddComponent<CollisionParam>().Copy( m_param );

			m_timer = 0;	
		}

		m_fireInterval = m_initFireInterval / m_param.m_level;
	}
}
