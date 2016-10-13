using UnityEngine;
using System.Collections;

public class Tragetor : MonoBehaviour
{
	private GameObject	m_enemyFactory;
	public	float		m_forcusRange		= 30.0f;
	public	float		m_targettingSpeed	= 0.1f;

	// Use this for initialization
	void Start()
	{
		m_enemyFactory = GameObject.Find("EnemyFactory");
	}
	
	// Update is called once per frame
	void Update ()
	{
		int		nearID	= -1;
		float	near	= m_forcusRange*m_forcusRange;//float.MaxValue;

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

			transform.rotation = Quaternion.Slerp ( transform.rotation, Quaternion.LookRotation( forward ), m_targettingSpeed );
		}
	}
}
