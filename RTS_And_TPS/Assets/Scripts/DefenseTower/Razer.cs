using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Razer : NetworkBehaviour
{
	public  float	m_interval	=   1.0f;
    public  float   m_RazerTime =   1.0f;
    public  float   m_fireRange =   30.0f;
	private int		m_lazerID	=   1;

    private EnemyShell_Control  m_rEnemyShell		=   null;
    private Tragetor            m_rTragetor         =   null;
    private float               m_IntervalTimer     =   0.0f;
    private float               m_RazerTimer        =   0.0f;

	// Use this for initialization
	void Start ()
	{
        m_rEnemyShell   =   GameObject.Find( "Enemy_Shell" ).GetComponent< EnemyShell_Control >();
        m_rTragetor     =   GetComponent< Tragetor >();
	}
	
	// Update is called once per frame
	void Update ()
	{
        Transform   rRazerTrans =   transform.GetChild( m_lazerID );
        GameObject  rRazer      =   rRazerTrans.gameObject;

        //  タイマー更新
        {
            if( m_RazerTimer <= 0.0f )  m_IntervalTimer =   Mathf.Max( m_IntervalTimer - Time.deltaTime, 0.0f );
            else                        m_RazerTimer    =   Mathf.Max( m_RazerTimer    - Time.deltaTime, 0.0f );
        }

        //  当たり判定を行うのは１フレームだけ
        rRazer.GetComponent< Collider >().enabled   =   false;

        //  当たり判定
        if( isServer
        &&  m_IntervalTimer <= 0.0f
        &&  m_rEnemyShell.IsExistEnemy() && m_rEnemyShell.CheckWhetherWithinTheRange( transform.position, m_fireRange ) ){
            m_rTragetor.UpdateRotation();

            rRazer.SetActive( true );
            rRazer.GetComponent< Collider >().enabled   =   true;

            m_IntervalTimer =   m_interval;
            m_RazerTimer    =   m_RazerTime;

            //  クライアント側でも処理
            RpcSpawnRazer( transform.rotation );
        }
        //  放射終了
        if( m_RazerTimer <= 0.0f ){
            rRazer.SetActive( false );
        }
	}

    //  リクエスト
    [ ClientRpc ]
    void    RpcSpawnRazer( Quaternion _Rotation )
    {
        if( isServer )      return;

        GameObject  rRazer  =   transform.GetChild( m_lazerID ).gameObject;

        transform.rotation  =   _Rotation;

        rRazer.SetActive( true );

        m_RazerTimer    =   m_RazerTime;
    }
}
