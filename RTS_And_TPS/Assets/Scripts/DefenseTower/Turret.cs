using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Turret : NetworkBehaviour
{
	public GameObject		m_bullet;
	public float			m_fireInterval		= 1.0f;
	public float			m_fireRange			= 30.0f;
	public float			m_bulletSpeed		= 1.0f;

	private float			m_initFireInterval	= 0;

	private CollisionParam	m_param				= null;
	private EnemyFactory	m_factory			= null;
    private Tragetor        m_rTragetor         = null;

    private float           m_IntervalTimer     =   0.0f;

	// Use this for initialization
	void Start ()
	{
		m_initFireInterval	= m_fireInterval;
		m_param				= GetComponent<CollisionParam>();
        m_rTragetor         = GetComponent<Tragetor>();

		m_factory			= GameObject.Find("EnemyFactory").GetComponent<EnemyFactory>();

        //  サーバーでのみ処理を行う
        //if( isServer ){
        //    StartCoroutine( Spawn() );
        //}
	}
	
	// Update is called once per frame
	void Update ()
	{
        //  サーバーでのみ処理を行う
        if( !isServer ) return;

		m_fireInterval = m_initFireInterval / m_param.m_level;

        //  タイマー更新
        m_IntervalTimer +=  Time.deltaTime;
        m_IntervalTimer =   Mathf.Min( m_IntervalTimer, m_fireInterval );
        if( m_IntervalTimer >= m_fireInterval ){
            //  
            if( m_factory.IsExistEnemy() && m_factory.CheckWhetherWithinTheRange( transform.position, m_fireRange ) ){
                m_rTragetor.UpdateRotation();

				GameObject g			= Instantiate( m_bullet );
				g.transform.position	= transform.position;
				g.GetComponent<TurretBullet>().Set( transform.rotation, m_bulletSpeed );
				g.AddComponent<CollisionParam>().Copy( m_param );

                //  クライアントでも弾が見えるようにする
                RpcSpawnBullet( transform.rotation, transform.position, m_bulletSpeed );

                //  インターバルリセット
                m_IntervalTimer =   0.0f;
            }
        }
	}

	IEnumerator Spawn()
    {
        while( true )
        {
			if( m_factory.IsExistEnemy() && m_factory.CheckWhetherWithinTheRange( transform.position, m_fireRange ))
			{
                m_rTragetor.UpdateRotation();

				GameObject g			= Instantiate( m_bullet );
				g.transform.position	= transform.position;
				g.GetComponent<TurretBullet>().Set( transform.rotation, m_bulletSpeed );
				g.AddComponent<CollisionParam>().Copy( m_param );

                //  クライアントでも弾が見えるようにする
                RpcSpawnBullet( transform.rotation, transform.position, m_bulletSpeed );
			}
			
			yield return new WaitForSeconds( m_fireInterval );
		}
	}

    //  リクエスト
    [ ClientRpc ]
    void    RpcSpawnBullet( Quaternion _Rotation, Vector3 _Position, float _BulletSpeed )
    {
        if( isServer )      return;

        transform.rotation  =   _Rotation;

        GameObject  g			= Instantiate( m_bullet );
		g.transform.position	= _Position;
		g.GetComponent<TurretBullet>().Set( _Rotation, _BulletSpeed );
		g.AddComponent<CollisionParam>().Copy( m_param );
    }
}
