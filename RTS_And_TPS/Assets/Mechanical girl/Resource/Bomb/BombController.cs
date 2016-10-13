using UnityEngine;
using System.Collections;

public class BombController : MonoBehaviour
{
	public float m_lifespan		= 1.0f;
	public float m_bombRange	= 1.0f;

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
		m_lifespan -= Time.deltaTime;

		//
		if( m_lifespan < 0 )
		{
			Destroy( gameObject );

			//GameObject ef = GameObject.Find("EnemyFactory");

			//for( int i=0; i<ef.transform.childCount; ++i )
			//{
			//	Vector3 pos		= ef.transform.GetChild(i).transform.position;
			//	float	dist	= ( transform.position - pos ).sqrMagnitude;

			//	if( dist < m_bombRange*m_bombRange )
			//	{
			//		Destroy( ef.transform.GetChild(i).gameObject );
			//	}
			//}
		}
	}
}
