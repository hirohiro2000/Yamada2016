using UnityEngine;
using System.Collections;

public class EnemyFactory : MonoBehaviour
{
	public GameObject	m_enemy;
	private int			m_timer = 0;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if( m_timer++ > 30 )
		{
			GameObject	e			= Instantiate( m_enemy );
			Vector2		r			= Random.insideUnitCircle * 50;

			e.transform.parent		= transform;
			e.transform.position	= new Vector3( r.x, 0.0f, r.y );

			m_timer					= 0;
		}
	}
}
