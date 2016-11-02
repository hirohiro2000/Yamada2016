
using UnityEngine;
using System.Collections;

public class TurretBullet : MonoBehaviour
{
	private	float		m_speed			= 1.0f;
	public float		m_lifespan		= 3.0f;
	public float		m_chaseSpeed	= 1.0f;

	private EnemyShell_Control  m_rEnemyShell   =   null;
    private LinkManager         m_rLinkManager  =   null;
	private Transform		    m_target        =   null;

	// Use this for initialization
	void Start ()
	{
		m_rEnemyShell   =   GameObject.Find( "Enemy_Shell" ).GetComponent< EnemyShell_Control >();
        m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );
		m_rEnemyShell.GetNearEnemyTransform( ref m_target, transform.position, float.MaxValue );
	}
	
	// Update is called once per frame
	void Update ()
	{
		if( m_target == null )
		{
			Destroy( gameObject );
			return;
		}


		//
		Vector3 vec = m_target.position - transform.position;
		vec.Normalize();

		transform.rotation = Quaternion.Slerp ( transform.rotation, Quaternion.LookRotation( vec ), m_chaseSpeed*Time.deltaTime );
		transform.position	+= ( transform.forward * m_speed * Time.deltaTime );


		//
		m_lifespan -= Time.deltaTime;
		if( m_lifespan < 0 )
			Destroy( gameObject );
	}

	public void Set( Quaternion _Rotation, float speed )
	{
		transform.rotation	= _Rotation;
		m_speed				= speed;
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
        if( m_rLinkManager.isServer ){
            rEnemy.GiveDamage( rParam.GetCurLevelParam().power );
        }

        //  オブジェクトを破棄
        Destroy( gameObject );
	}

}
