using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class FiringDevice : NetworkBehaviour
{
	public GameObject		        m_bullet;

	private ReferenceWrapper        m_rEnemyShell		= null;
	private	ResourceParameter	    m_resourceParam		= null;
    private RTSResourece_Control    m_RTSResControl     = null;

    private float                   m_IntervalTimer     = 0.0f;

	// Use this for initialization
	void Start ()
	{
		m_rEnemyShell       = GameObject.Find( "EnemySpawnRoot" ).GetComponent< ReferenceWrapper >();
		m_resourceParam		= GetComponent<ResourceParameter>();
        m_RTSResControl     = GetComponent<RTSResourece_Control>();
	}
	
	// Update is called once per frame
	void Update ()
	{
        //  サーバーでのみ処理を行う 
        //if( !isServer ) return;

        //  タイマー更新
        m_IntervalTimer +=  Time.deltaTime;
        m_IntervalTimer =   Mathf.Min( m_IntervalTimer, m_resourceParam.GetCurLevelParam().interval );

		if ( m_IntervalTimer >= m_resourceParam.GetCurLevelParam().interval )
		{
            if( m_rEnemyShell.IsExistEnemy()
            &&  m_rEnemyShell.CheckWhetherWithinTheRange( transform.position, m_resourceParam.GetCurLevelParam().range ) )
			{
                UpdateRotation();

				GameObject g			= Instantiate( m_bullet );
				g.transform.position	= transform.position;
				g.transform.rotation	= transform.rotation;

                AttackPointList rATKPL  =   g.GetComponentInChildren< AttackPointList >();
                GameObject      rObj    =   rATKPL.gameObject;
                rATKPL.baseAttackPoint  *=  m_resourceParam.GetCurLevelParam().power;

                rObj.AddComponent< ResourceParameter >().Copy( m_resourceParam );

                //  オーナー設定
                rObj.GetComponent< RTSAttack_Net >().c_AttackerID   =   m_RTSResControl.c_OwnerID;

                //  クライアントでも弾が見えるようにする
                //RpcSpawnBullet( transform.rotation, transform.position );

                //  インターバルリセット
                m_IntervalTimer =   0.0f;
            }
        }
	}

	//
	void UpdateRotation()
    {
        Transform trs = m_rEnemyShell.GetNearEnemyTransform( transform.position, m_resourceParam.GetCurLevelParam().range );
        if( trs )
        {
            Vector3 forward = trs.position - transform.position;

            forward.y   =   0.0f;
            forward.Normalize();
        
            transform.rotation  =   Quaternion.LookRotation( forward );
        }
    }


    //  リクエスト
    [ ClientRpc ]
    void    RpcSpawnBullet( Quaternion _Rotation, Vector3 _Position )
    {
        if( NetworkServer.active )      return;

        transform.rotation  =   _Rotation;

        GameObject  g			= Instantiate( m_bullet );
		g.transform.position	= _Position;
		g.transform.rotation	= _Rotation;
		g.AddComponent<ResourceParameter>().Copy( m_resourceParam );
    }
}
