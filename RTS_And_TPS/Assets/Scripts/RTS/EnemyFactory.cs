//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Networking;
//using System.Collections;

//public class EnemyFactory : NetworkBehaviour
//{
//	public	GameObject	m_enemy;
//	public	int			m_max			= 1;
//	public	float		m_eachInterval	= 1.0f;
//	public	float		m_waveInterval	= 30.0f;

//	private bool		m_running		= false;
//	private	float		m_waveCounter	= 0.0f;

//    [ SyncVar ]
//	private int			m_curWave		= 0;

//	// Use this for initialization
//	void Start ()
//	{
//        if( isServer ){
//		    StartCoroutine( Spawn() );
//        }
//	}
	
//	// Update is called once per frame
//	void Update ()
//	{
//        if( isServer ){
//		    m_waveCounter += Time.deltaTime;

//		    if( m_waveCounter > m_waveInterval )
//		    {
//			    m_running		= !m_running;
//			    m_waveCounter	= 0;

//			    //	up wave
//			    if( m_running )
//			    {
//				    m_curWave++;
//			    }
//		    }
//        }

//		GameObject.Find("Canvas").transform.FindChild("Column_Wave").GetChild(0).GetComponent<Text>().text = "Wave  " + m_curWave.ToString();
//	}

//	IEnumerator Spawn()
//    {
//        while( true )
//        {
//			if( m_running )
//			{
//				GameObject	e			= Instantiate( m_enemy );

//				//	set wave information
//				int attack				= ( m_curWave / 3 ) + 1;
//				int defense				= ( m_curWave / 5 );
//				int hp					= ( m_curWave / 2 ) * 10 + 10;
//				e.GetComponent<ResourceParameter>().SetInitParam( attack, defense, hp );
//				e.GetComponent<NavMeshAgent>().speed = ( m_curWave / 2 ) + 5;

//				//	set spawned information
//				Vector2 rnd             = new Vector2( Random.Range( -10.0f, 10.0f ), Random.Range( -10.0f, 10.0f ) );
//				rnd.Normalize();
//				rnd						*= Random.Range( 40.0f, 80.0f );

//				e.transform.parent		= transform;
//				e.transform.position	= new Vector3( rnd.x, 0.0f, rnd.y );

//                //  ネットワーク上で生成
//                NetworkServer.Spawn( e );
//			}
           
//            yield return new WaitForSeconds( m_eachInterval );
//        }     
//    }

//	public bool IsExistEnemy()
//	{
//		return transform.childCount > 0;
//	}
//	public bool CheckWhetherWithinTheRange( Vector3 target, float rangeDist )
//	{
//		float	near	= rangeDist*rangeDist;

//		for( int i=0; i<transform.childCount; ++i )
//		{
//			Vector3 pos		= transform.GetChild(i).transform.position;
//			float	dist	= ( target - pos ).sqrMagnitude;

//			if( dist < near )
//			{
//				return true;
//			}
//		}

//		return false;
//	}
//	public bool GetNearEnemyTransform( ref Transform trans, Vector3 target, float maxDist )
//	{
//		int		nearID	= -1;
//		float	near	= maxDist*maxDist;

//		for( int i=0; i<transform.childCount; ++i )
//		{
//			Vector3 pos		= transform.GetChild(i).transform.position;
//			float	dist	= ( target - pos ).sqrMagnitude;

//			if( dist < near )
//			{
//				near	= dist;
//				nearID	= i;
//			}
//		}

//		if( nearID != -1 )
//		{
//			trans = transform.GetChild( nearID ).transform;
//			return true;
//		}

//		trans = null;
//		return false;
//	}

//	void OnGUI ()
//	{
//    }
//}
