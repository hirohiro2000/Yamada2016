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
    public  float                   c_BulletSpeed           = 1.0f;
    public  bool                    c_UseNewExpect          = false;

    public  float                   c_AngleSpeed            = 0.0f;
    public  bool                    c_UseNewTargetting      = false;

    public  float                   c_ShotAngleThreshold    = 0.0f;
    public  bool                    c_ShotAngleCheckHorizon = false;

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
        		
        //  ターゲット
        Transform           rTarget     =   m_rEnemyShell.GetNearEnemyTransform_CheckWall( m_orientatedTransform.position, m_resourceParam.GetCurLevelParam().range );
        if( !rTarget )
            return;

        Transform           rEye        =   rTarget.FindChild( "Eye" );
        float               distance    =   ( rEye.position - m_firePointTransforms[ 0 ].position ).magnitude;

        Vector3             targetPoint =   ( c_UseNewExpect )? rTarget.GetComponent< DeviationCalculator >().GetCorrectionPoint( c_BulletSpeed, distance )
                                                            :   rTarget.GetComponent< DeviationCalculator >().Get();

        //	ターゲットに向く
		Targetting( targetPoint );
	
		//	壁判定
		{
			var		vector		= targetPoint - m_orientatedTransform.position;
			Vector3	dir			= transform.TransformDirection( vector.normalized );
			int     layerMask   = LayerMask.GetMask( "Field" );

            RaycastHit  hitInfo;
            if( Physics.Raycast( m_orientatedTransform.position, dir, out hitInfo, vector.magnitude, layerMask ) ){
                //  床は無視
                if( Vector3.Angle( Vector3.up, hitInfo.normal ) < 30.0f ){}
                //  壁はだめ
                else                                                        return;
            }
		}

        //  照準判定
        if( !CheckAiming( targetPoint, c_ShotAngleThreshold, c_ShotAngleCheckHorizon ) ){
            //Debug.Log( "Aiming_False" );
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
	void    Targetting( Vector3 _TargetPoint )
    {
        //var trs = m_rEnemyShell.GetNearEnemyTransform( m_orientatedTransform.position, m_resourceParam.GetCurLevelParam().range );

        //if( trs == null )
        //{
        //    Debug.Log("enemy transform is null");
        //    return;
        //}

        //if( trs.GetComponent<DeviationCalculator>() == null )
        //{
        //    Debug.Log("enemy DeviationCalculator is null");
        //    return;
        //}
        
		//
		//Vector3 forward = trs.GetComponent<DeviationCalculator>().Get() - m_orientatedTransform.position;
        Vector3 forward     =   _TargetPoint - m_orientatedTransform.position;
		forward.Normalize();
        
		//	即時
		//m_orientatedTransform.rotation	=   Quaternion.LookRotation( forward );

        //  速度制限ありの即時回転
        if( c_UseNewTargetting ){
            m_orientatedTransform.rotation  =   Quaternion.RotateTowards( m_orientatedTransform.rotation, Quaternion.LookRotation( forward ), c_AngleSpeed * Time.deltaTime * 60.0f );
        }
        //  
        else{
		    //	補間
		    m_orientatedTransform.rotation	=	Quaternion.Slerp( m_orientatedTransform.rotation, Quaternion.LookRotation( forward ), Time.deltaTime*5.0f );
        }
    }

    //  振り向ききってるかどうかチェック
    bool    CheckAiming( Vector3 _TargetPoint, float _AngleThreshold, bool _HorizontalOnly )
    {
        Vector3 vBarrel     =   m_orientatedTransform.forward.normalized;
        Vector3 toTarget    =   ( _TargetPoint - m_orientatedTransform.position ).normalized;

        if( _HorizontalOnly ){
            vBarrel.y       =   0.0f;
            toTarget.y      =   0.0f;
        }

        float   angleDist   =   Vector3.Angle( vBarrel, toTarget );

        return  angleDist <= _AngleThreshold;
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
