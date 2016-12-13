
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class TurretBullet : MonoBehaviour
{
	public float				m_speed			= 1.0f;
	public float				m_chaseSpeed	= 1.0f;

	private ReferenceWrapper	m_rEnemyShell   =   null;
    //private LinkManager         m_rLinkManager  =   null;
	private Transform		    m_target        =   null;

	// Use this for initialization
	void Start ()
	{
		m_rEnemyShell   =   GameObject.Find( "EnemySpawnRoot" ).GetComponent< ReferenceWrapper >();
        //m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );
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
		transform.position	+= ( transform.forward * m_speed * Time.deltaTime );		
	}

	void    OnTriggerEnter( Collider _Collider )
	{
        Transform   rTrans  =   _Collider.transform;
        if( rTrans.tag != "Enemy" )     return;

        TPS_Enemy   rEnemy  =   rTrans.GetComponent< TPS_Enemy >();
        if( !rEnemy )   rEnemy  =   rTrans.GetComponentInParent< TPS_Enemy >();
        if( !rEnemy )                   return;

        ResourceParameter  rParam  =   GetComponent< ResourceParameter >();
        if( !rParam )                   return;

        //  ダメージを与える（サーバーのみ）
        if( NetworkServer.active ){
            rEnemy.GiveDamage( rParam.GetCurLevelParam().power );
        }

        //  オブジェクトを破棄
        Destroy( gameObject );
	}

}
