using UnityEngine;
using System.Collections;

public class Debris_Control : MonoBehaviour {

    enum    State{
        Stay,
        Move,
    };

    //public  Transform   c_Target        =   null;
    public  GameObject      c_EndEmission   =   null;
    public  GameObject      c_EndEmission2  =   null;
    public  GameObject      c_HitEmission   =   null;
    public  GameObject      c_MoveEmission  =   null;

    public  int             c_TargetID      =   0;
    public  float           c_Score         =   0.0f;
    public  bool            c_ForMe         =   false;

    
    public  bool            c_UseSpeedLimit =   false;
    public  float           c_SpeedLimit    =   0.0f;
    public  float           c_LimitTime     =   0.0f;

    private float           c_MoveTime      =   1.0f;
    private State           m_State         =   State.Stay;

    //  移動関連
    private Vector3         m_StartPoint    =   Vector3.zero;
    //private float           m_StartScale    =   1.0f;
    //private float           m_StartScaleW   =   1.0f;
    private float           m_MoveTimer     =   0.0f;

    private float[]         c_MoveCurve     =   new float[]{    0.5f,   0.475f,  0.5f,   1.0f   };//new float[]{    0.5f,   0.35f,   0.5f,   1.0f    };
    private float[]         c_UpCurve       =   new float[]{    0.5f,   1.0f,   0.5f,   0.5f    };
    public  Transform       m_rTarget       =   null;

    private float           c_BirthTime     =   1.5f;
    private float           m_BearthTimer   =   0.0f;

    private float           m_Timer         =   0.0f;

	private bool			m_Saving		=	false;

    //  アクセス
    private LinkManager     m_rLinkManager  =   null;
    private GameManager     m_rGameManager  =   null;
    //private Flag_Control    m_rCentral      =   null;
    private Rigidbody       m_rRigid        =   null;
    private Vector3         m_ChargeVector  =   Vector3.zero;

	// Use this for initialization
	void    Start()
    {
        //  アクセス取得
        m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );
        m_rGameManager  =   FunctionManager.GetAccessComponent< GameManager >( "GameManager" );
        //m_rCentral      =   FunctionManager.GetAccessComponent< Flag_Control >( "Central_Flag" );

        m_rRigid        =   GetComponent< Rigidbody >();

        m_BearthTimer   =   c_BirthTime;
        //m_StartScale    =   transform.localScale.x;
        //m_StartScaleW   =   transform.lossyScale.x;
	}
	
	// Update is called once per frame
	void    Update()
    {
        //  アクセス取得
        //if( !m_rLinkManager )   m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );

        //  タイマー更新
        m_BearthTimer   -=  Time.deltaTime;
        m_BearthTimer   =   Mathf.Max( m_BearthTimer, 0.0f );

        if( m_Timer < c_LimitTime ){
            m_Timer     +=  Time.deltaTime;
            m_Timer     =   Mathf.Min( m_Timer, c_LimitTime );
            if( m_Timer >= c_LimitTime ){
                //m_rRigid.velocity   =   m_ChargeVector * 2.0f;
            }
        }

		//Saving状態なら一定時間後軽量レイヤーへ切り替え
		{
			if(m_Timer > 0.2f)
			{
				if (m_Saving == true)
					transform.gameObject.layer = LayerMask.NameToLayer("Debris_Light");
			}
		}

        //  状態ごとの処理
	    switch( m_State ){
            case    State.Stay: Update_Stay();  break;
            case    State.Move: Update_Move();  break;
        }
	}
	void    Update_Stay()
    {
		if(m_rRigid)
		{
			if(m_Timer > 0.1f)
				if(m_rRigid.velocity.magnitude < 0.0001f && m_rRigid.angularVelocity.magnitude < 0.0001f)
				{
					if (m_rRigid)
						Destroy(m_rRigid);
				}

		}
    }
    void    Update_Move()
    {
        //  時間を進める
        m_MoveTimer +=  Time.deltaTime;
        m_MoveTimer =   Mathf.Min( m_MoveTimer, c_MoveTime );

        //  移動処理
        {
            Vector3 vMove       =   ( ( m_rTarget == null )? Vector3.zero : m_rTarget.position + Vector3.up * 1.0f ) - m_StartPoint;
            float   timeRate    =   m_MoveTimer / c_MoveTime;
            float   moveRate    =   ( FunctionManager.CalcBezie( c_MoveCurve, timeRate ) - 0.5f ) * 2.0f;
            float   upRate      =   ( FunctionManager.CalcBezie( c_UpCurve, timeRate )   - 0.5f ) * 2.0f;

            transform.position  =   m_StartPoint + vMove * moveRate + Vector3.up * upRate * 2.0f;

            //  終了処理
            if( timeRate >= 1.0f ){ 
                //  リソース獲得（自分のプレイヤーだけ処理）
                if( m_rLinkManager ){
                    if( c_TargetID == m_rLinkManager.m_LocalPlayerID ){
                        if( m_rLinkManager.m_rLocalNPControl )  m_rLinkManager.m_rLocalNPControl.CmdAddResource( c_Score );
                        if( m_rGameManager )                    m_rGameManager.SetAcqResource( c_Score );
                    }
                }

                //  獲得エフェクト  
                if( c_EndEmission ){
                    GameObject  rObj    =   Instantiate( c_EndEmission );
                    Transform   rTrans  =   rObj.transform;
                    rTrans.position     =   transform.position;//m_rTarget.position + Vector3.up * 1.0f;
                    rTrans.parent       =   m_rTarget;
                }
                if( c_EndEmission2
                //&&  Random.Range( 0, 6 ) == 0 
                    ){
                    GameObject  rObj    =   Instantiate( c_EndEmission2 );
                    Transform   rTrans  =   rObj.transform;
                    rTrans.position     =   transform.position;
                    rTrans.parent       =   m_rTarget;
                }
                if( c_HitEmission
                &&  Random.Range( 0, 3 ) == 0 
                    ){
                    GameObject  rObj    =   Instantiate( c_HitEmission );
                    Transform   rTrans  =   rObj.transform;
                    rTrans.position     =   transform.position;

                    SoundPlay_Control   rSound  =   rObj.GetComponent< SoundPlay_Control >();
                    rSound.c_PitchRatio     =   Random.Range( 0.5f, 1.5f );
                    rSound.c_VolumeRatio    =   Random.Range( 1.0f, 2.0f );
                    rSound.c_LifeTime       =   rSound.c_LifeTime / rSound.c_PitchRatio;
                }

                //  削除 
                Destroy( gameObject );
                return;
            }
        }
    }

    void    FixedUpdate()
    {
        if( !c_UseSpeedLimit )  return;
        if( !m_rRigid ){
            c_UseSpeedLimit =   false;
            return;
        }
        //  一定時間経過
        if( m_Timer >= c_LimitTime ){
            c_UseSpeedLimit =   false;
            return;
        }

        //  速度制限 
        Vector3 velocity    =   m_rRigid.velocity;
        float   curSpeed    =   velocity.magnitude;
        if( curSpeed > c_SpeedLimit ){
            //m_rRigid.velocity   =   velocity.normalized * c_SpeedLimit;
            m_rRigid.velocity   =   Vector3.zero;
            if( curSpeed > m_ChargeVector.magnitude ){
                m_ChargeVector  =   velocity;
            }
        }
    }

    void    OnCollisionEnter( Collision _rCollision )
    {
        if( m_State != State.Stay )                                             return;
        if( _rCollision.gameObject.layer == LayerMask.NameToLayer( "Debris" ) ) return;

        //  音を再生  
        if( c_HitEmission
        &&  Random.Range( 0, 5 ) == 0 ){
            GameObject  rObj    =   Instantiate( c_HitEmission );
            Transform   rTrans  =   rObj.transform;
            rTrans.position     =   transform.position;

            SoundPlay_Control   rSound  =   rObj.GetComponent< SoundPlay_Control >();
            rSound.c_PitchRatio     =   Random.Range( 0.5f, 1.5f );
            rSound.c_VolumeRatio    =   Random.Range( 1.0f, 2.0f );
            rSound.c_LifeTime       =   rSound.c_LifeTime / rSound.c_PitchRatio;
        
            //  次は再生しない  
            //c_HitEmission       =   null;
        }
    }

    public  void    SetMove( Transform _rTrans, float _MoveTime )
    {
        c_MoveTime      =   _MoveTime;

        m_State         =   State.Move;
        m_StartPoint    =   transform.position;
        m_rTarget       =   _rTrans;
        
        //  コンポーネント削除
        Rigidbody   rRigid  =   GetComponent< Rigidbody >();
        Collider    rCol    =   GetComponent< Collider >();
        if( rRigid )    Destroy( rRigid );
        if( rCol )      Destroy( rCol );

        //  効果音再生 
        if( c_MoveEmission
        //&&  Random.Range( 0, 3 ) == 0  
            ){
            GameObject  rObj    =   Instantiate( c_MoveEmission );
            Transform   rTrans  =   rObj.transform;
            rTrans.position     =   transform.position;
            rTrans.parent       =   transform;

            SoundPlay_Control   rSound  =   rObj.GetComponent< SoundPlay_Control >();
            rSound.c_PitchRatio     =   Random.Range( 0.5f, 1.5f );
            rSound.c_VolumeRatio    =   Random.Range( 0.25f, 0.5f );
        }
    }
    public  bool    IsMove()
    {
        return  m_State == State.Move;
    }

	public void		ActiveSaving()
	{
		m_Saving = true;
	}
}
