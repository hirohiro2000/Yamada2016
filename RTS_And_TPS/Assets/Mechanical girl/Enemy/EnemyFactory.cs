using UnityEngine;
using System.Collections;

public class EnemyFactory : MonoBehaviour
{
	public	GameObject	m_enemy;
	public	int			m_max		= 1;
	public	float		m_interval	= 1.0f;

	// Use this for initialization
	void Start ()
	{
		StartCoroutine( Sporn() );
	}
	
	// Update is called once per frame
	void Update ()
	{
		if( transform.childCount >= m_max )
			return;
	}

	IEnumerator Sporn()
    {
        while( true )
        {
            GameObject	e			= Instantiate( m_enemy );

			Vector2 rnd             = new Vector2( Random.Range( -10.0f, 10.0f ), Random.Range( -10.0f, 10.0f ) );
			rnd.Normalize();
			rnd						*= Random.Range( 40.0f, 80.0f );

			e.transform.parent		= transform;
			e.transform.position	= new Vector3( rnd.x, 0.0f, rnd.y );

            yield return new WaitForSeconds( m_interval );
        }     
    }
}
