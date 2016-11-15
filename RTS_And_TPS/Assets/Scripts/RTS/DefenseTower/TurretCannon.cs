using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class TurretCannon : MonoBehaviour
{
	public float				m_bulletSpeed		= 1.0f;
	public float				m_chaseSpeed		= 1.0f;
	public float				m_explodeRange		= 1.0f;
	public GameObject			m_explosionEffect	= null;

	private ReferenceWrapper	m_rEnemyShell   = null;
    private LinkManager         m_rLinkManager  = null;
	private Transform		    m_target        = null;

	// Use this for initialization
	void Start ()
	{
		m_rEnemyShell   =   GameObject.Find( "EnemySpawnRoot" ).GetComponent< ReferenceWrapper >();
        m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );
		m_target		=	m_rEnemyShell.GetNearEnemyTransform( transform.position, float.MaxValue );
	}
	
	// Update is called once per frame
	void Update ()
	{
		//	対象がないと消す
		if( m_target == null )
		{
			Destroy( gameObject );
			return;
		}

		//	跳ぶ
		Vector3 vec = m_target.position - transform.position;
		vec.Normalize();

		transform.rotation = Quaternion.Slerp ( transform.rotation, Quaternion.LookRotation( vec ), m_chaseSpeed*Time.deltaTime );
		transform.position	+= ( transform.forward * m_bulletSpeed * Time.deltaTime );		
	}

	void    OnTriggerEnter( Collider _Collider )
	{
        if( _Collider.transform.tag != "Enemy" )
			return;

		//	範囲内の敵取得
		var list = m_rEnemyShell.GetNearEnemyTransforms( transform.position, m_explodeRange );

		for( int i=0; i<list.Count; ++i )
		{
			TPS_Enemy		rEnemy  =   list[i].GetComponent< TPS_Enemy >();
			if( !rEnemy )   rEnemy  =   list[i].GetComponentInParent< TPS_Enemy >();
			if( !rEnemy )                   continue;

			ResourceParameter  rParam  =   GetComponent< ResourceParameter >();
			if( !rParam )                   continue;

			//  ダメージを与える（サーバーのみ）
			if( NetworkServer.active ){
				rEnemy.GiveDamage( rParam.GetCurLevelParam().power );
			}
		}

		//	爆発エフェクト
		{
			var explosion = Instantiate( m_explosionEffect );
			explosion.transform.position = transform.position;
		}

        //  オブジェクトを破棄
        Destroy( gameObject );
	}
}
