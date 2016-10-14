using UnityEngine;
using System.Collections;

public class EnemyFactory : MonoBehaviour
{
	public	GameObject	m_enemy;
	public	int			m_max			= 1;
	public	float		m_eachInterval	= 1.0f;
	public	float		m_waveInterval	= 30.0f;

	private bool		m_running		= false;
	private	float		m_waveCounter	= 0.0f;
	private int			m_curWave		= 0;

	// Use this for initialization
	void Start ()
	{
		StartCoroutine( Spawn() );
	}
	
	// Update is called once per frame
	void Update ()
	{
		m_waveCounter += Time.deltaTime;

		if( m_waveCounter > m_waveInterval )
		{
			m_running		= !m_running;
			m_waveCounter	= 0;

			//	up wave
			if( m_running )
			{
				m_curWave++;
			}
		}
	}

	IEnumerator Spawn()
    {
        while( true )
        {
			if( m_running )
			{
				GameObject	e			= Instantiate( m_enemy );

				//	set wave information
				int attack				= ( m_curWave / 3 ) + 1;
				int defense				= ( m_curWave / 5 );
				int hp					= ( m_curWave / 2 ) * 10 + 10;
				e.GetComponent<CollisionParam>().SetInitParam( attack, defense, hp );
				e.GetComponent<NavMeshAgent>().speed = ( m_curWave / 2 ) + 5;

				//	set spawned information
				Vector2 rnd             = new Vector2( Random.Range( -10.0f, 10.0f ), Random.Range( -10.0f, 10.0f ) );
				rnd.Normalize();
				rnd						*= Random.Range( 40.0f, 80.0f );

				e.transform.parent		= transform;
				e.transform.position	= new Vector3( rnd.x, 0.0f, rnd.y );
			}
           
            yield return new WaitForSeconds( m_eachInterval );
        }     
    }


	void OnGUI ()
	{
		GUIStyle		style = new GUIStyle();
		style.alignment = TextAnchor.MiddleLeft;

		GUIStyleState	state = new GUIStyleState();
		state.textColor = new Color( 1,1,1,1 );
		
		style.normal = state;
		
		GUI.Label ( new Rect (700,200,200,100), 
			"",
			"box");

        GUI.TextArea ( new Rect (700,200,200,100), 
			"　ウェーブレベル　" + m_curWave.ToString() +
			"\n　稼働　　　　　　" + m_running.ToString() +
			"\n　タイマー　　　　" + (m_waveInterval-m_waveCounter).ToString() + "秒",
			style );
    }
}
