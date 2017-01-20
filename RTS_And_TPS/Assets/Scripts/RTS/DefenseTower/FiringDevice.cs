using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;


public class FiringDevice : NetworkBehaviour
{
	[SerializeField]
	private GameObject		        m_bullet				= null;

	[SerializeField]
	private List< Transform >		m_firePointTransforms	= null;

	[SerializeField]
	private Transform		        m_orientatedTransform	= null;

	private ReferenceWrapper        m_rEnemyShell			= null;
	private	ResourceParameter	    m_resourceParam			= null;
    private RTSResourece_Control    m_RTSResControl			= null;
	private ResourceAppear			m_resourceAppear		= null;

    private float                   m_IntervalTimer			= 0.0f;

    public  GameObject              c_ShotSound             = null;

	// Use this for initialization
	void Start ()
	{
		m_rEnemyShell       = GameObject.Find( "EnemySpawnRoot" ).GetComponent< ReferenceWrapper >();
		m_resourceParam		= GetComponent<ResourceParameter>();
        m_RTSResControl     = GetComponent<RTSResourece_Control>();
		m_resourceAppear	= GetComponent<ResourceAppear>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		//	生えてるときは動かない
		if( !m_resourceAppear.IsEnd() )
			return;
       
     
		//  タイマー更新
        m_IntervalTimer +=  Time.deltaTime;
        m_IntervalTimer =   Mathf.Min( m_IntervalTimer, m_resourceParam.GetCurLevelParam().interval );

	
		//	敵の存在確認
		if( !m_rEnemyShell.IsExistEnemy() )
			return;

		if( !m_rEnemyShell.CheckWhetherWithinTheRange( transform.position, m_resourceParam.GetCurLevelParam().range ))
			return;


		//	ターゲットに向く
		Tragetting();
		
	
		//	壁判定
		{
			var		target		= m_rEnemyShell.GetNearEnemyTransform( transform.position, m_resourceParam.GetCurLevelParam().range ).FindChild("Eye");
			var		vector		= target.position - m_orientatedTransform.position;
			Vector3	dir			= transform.TransformDirection( vector.normalized );
			int     layerMask   = LayerMask.GetMask( "Field" );

			if( Physics.Raycast( m_orientatedTransform.position, dir, vector.magnitude, layerMask ))
				return;
		}


		//	発射
		if ( m_IntervalTimer >= m_resourceParam.GetCurLevelParam().interval )
		{
			foreach( var fires in m_firePointTransforms )
			{
				Fire( fires, m_orientatedTransform );
			}

			//  音再生
            if( c_ShotSound ){
                GameObject  rObj    =   Instantiate( c_ShotSound );
                Transform   rTrans  =   rObj.transform;
                rTrans.position     =   transform.position;
            }

			//  インターバルリセット
			m_IntervalTimer =   0.0f;
		}   	 
	}


	//
	void Fire( Transform fire, Transform orien )
	{
		var instance				=	Instantiate( m_bullet );
		instance.transform.position	=	fire.position;
		instance.transform.rotation	=	orien.rotation;

		//
		AttackPointList rATKPL		=   instance.GetComponentInChildren< AttackPointList >();
		GameObject      rObj		=   rATKPL.gameObject;
		rATKPL.baseAttackPoint		*=  m_resourceParam.GetCurLevelParam().power;

		rObj.AddComponent< ResourceParameter >().Copy( m_resourceParam );

		//  オーナー設定
        RTSAttack_Net   rRTSAttack  =   rObj.GetComponent< RTSAttack_Net >();
        BombExplosion   rBomb       =   rObj.GetComponent< BombExplosion >();
        if( rRTSAttack )    rRTSAttack.c_AttackerID =   m_RTSResControl.c_OwnerID;
        if( rBomb )         rBomb.c_AttackerID      =   m_RTSResControl.c_OwnerID;

        //  榴弾の場合は火力も設定
        if( rBomb ){
            rBomb.c_ATK     =   m_resourceParam.GetCurLevelParam().power;
        }
	}


	//
	void Tragetting()
    {
		var trs = m_rEnemyShell.GetNearEnemyTransform( m_orientatedTransform.position, m_resourceParam.GetCurLevelParam().range );
        if( trs )
        {
			trs	= trs.FindChild("Eye");

            Vector3 forward = trs.position - m_orientatedTransform.position;

            //forward.y   =   0.0f;
            forward.Normalize();
        
			//	即時
			//m_orientatedTransform.rotation	=   Quaternion.LookRotation( forward );

			//	補間
			m_orientatedTransform.rotation	=	Quaternion.Slerp( m_orientatedTransform.rotation, Quaternion.LookRotation( forward ), Time.deltaTime*2.0f );
        }
    }


    //  リクエスト
    [ ClientRpc ]
    void    RpcSpawnBullet( Quaternion _Rotation, Vector3 _Position )
    {
        if( NetworkServer.active )      return;

		m_orientatedTransform.rotation	=   _Rotation;

        GameObject  g			= Instantiate( m_bullet );
		g.transform.position	= _Position;
		g.transform.rotation	= _Rotation;
		g.AddComponent<ResourceParameter>().Copy( m_resourceParam );
    }
}
