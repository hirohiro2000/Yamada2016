using UnityEngine;
using System.Collections;

public class EnemyFactory : MonoBehaviour
{
	public	GameObject	m_enemy;
	public	int			m_max		= 300;
	public	int			m_interval	= 30;
	private int			m_timer		= 0;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if( transform.childCount >= m_max )
			return;

		if( m_timer++ > m_interval )
		{
			GameObject	e			= Instantiate( m_enemy );
			
            //Vector2		r			= Random.insideUnitCircle * 50;
            Vector2 rnd             = new Vector2( Random.Range( -10.0f, 10.0f ), Random.Range( -10.0f, 10.0f ) );
			rnd.Normalize();
			rnd						*= Random.Range( 40.0f, 80.0f );

			e.transform.parent		= transform;
			e.transform.position	= new Vector3( rnd.x, 0.0f, rnd.y );

			m_timer					= 0;
		}
	}
}
