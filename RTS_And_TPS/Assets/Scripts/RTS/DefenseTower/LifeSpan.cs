using UnityEngine;
using System.Collections;

public class LifeSpan : MonoBehaviour
{
	public float m_lifespan		=	1.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		//	寿命更新
		m_lifespan -= Time.deltaTime;

		if( m_lifespan < 0 )
		{
			Destroy( gameObject );
			return;
		}
	}
}
