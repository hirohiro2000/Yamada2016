﻿using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour
{
	public GameObject	m_bullet;
	public int			m_fireInterval		= 1;

	private int			m_timer				= 0;
	private Transform	m_bulletsParent;

	// Use this for initialization
	void Start ()
	{
		m_bulletsParent = GameObject.Find("BulletsParent").transform;
	}
	
	// Update is called once per frame
	void Update ()
	{
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
