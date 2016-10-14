using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour
{
	public GameObject	m_bullet;
	public float		m_fireInterval		= 1.0f;
	public float		m_bulletSpeed		= 1.0f;

	private float		m_initFireInterval	= 0;

	private CollisionParam m_param = null;

	// Use this for initialization
	void Start ()
	{
		m_initFireInterval	= m_fireInterval;
		m_param				= GetComponent<CollisionParam>();

		StartCoroutine( Spawn() );
	}
	
	// Update is called once per frame
	void Update ()
	{
		m_fireInterval = m_initFireInterval / m_param.m_level;
	}

	IEnumerator Spawn()
    {
        while( true )
        {
			GameObject g			= Instantiate( m_bullet );
			g.transform.position	= transform.position;
			g.GetComponent<TurretBullet>().Set( transform.forward, m_bulletSpeed );
			g.AddComponent<CollisionParam>().Copy( m_param );
			
			yield return new WaitForSeconds( m_fireInterval );
		}
	}
}
