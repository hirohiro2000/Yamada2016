using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class FiringDevice : NetworkBehaviour
{
	public GameObject		        m_bullet;
	public Transform		        m_firePointTransform	= null;
	public Transform		        m_orientatedTransform	= null;

	private ReferenceWrapper        m_rEnemyShell			= null;
	private	ResourceParameter	    m_resourceParam			= null;
    private RTSResourece_Control    m_RTSResControl			= null;

    private float                   m_IntervalTimer			= 0.0f;

    [SerializeField]
    private string                  m_seName            = "";
    private SoundController         m_se                = null;

	// Use this for initialization
	void Start ()
	{
		m_rEnemyShell       = GameObject.Find( "EnemySpawnRoot" ).GetComponent< ReferenceWrapper >();
		m_resourceParam		= GetComponent<ResourceParameter>();
        m_RTSResControl     = GetComponent<RTSResourece_Control>();

        m_se                = SoundController.Create(m_seName, this.transform);
	}
	
	// Update is called once per frame
	void Update ()
	{
        //  サーバーでのみ処理を行う 
        //if( !isServer ) return;

        //  タイマー更新
        m_IntervalTimer +=  Time.deltaTime;
        m_IntervalTimer =   Mathf.Min( m_IntervalTimer, m_resourceParam.GetCurLevelParam().interval );

		//
		UpdateRotation();

		if ( m_IntervalTimer >= m_resourceParam.GetCurLevelParam().interval )
		{

            if( m_rEnemyShell.IsExistEnemy()
            &&  m_rEnemyShell.CheckWhetherWithinTheRange( transform.position, m_resourceParam.GetCurLevelParam().range ) )
			{
				GameObject g			= Instantiate( m_bullet );

				//	射出位置を決める
				g.transform.position	= m_firePointTransform.position;
				g.transform.rotation	= m_orientatedTransform.rotation;
			
				//
                AttackPointList rATKPL  =   g.GetComponentInChildren< AttackPointList >();
                GameObject      rObj    =   rATKPL.gameObject;
                rATKPL.baseAttackPoint  *=  m_resourceParam.GetCurLevelParam().power;

                rObj.AddComponent< ResourceParameter >().Copy( m_resourceParam );

                //  オーナー設定
                rObj.GetComponent< RTSAttack_Net >().c_AttackerID   =   m_RTSResControl.c_OwnerID;

                //  クライアントでも弾が見えるようにする
                //RpcSpawnBullet( transform.rotation, transform.position );

                // 音再生
                m_se.PlayOneShot();

                //  インターバルリセット
                m_IntervalTimer =   0.0f;
            }
        }
	}


	//
	void UpdateRotation()
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
			m_orientatedTransform.rotation	=	Quaternion.Slerp( m_orientatedTransform.rotation, Quaternion.LookRotation( forward ), 0.2f );
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
