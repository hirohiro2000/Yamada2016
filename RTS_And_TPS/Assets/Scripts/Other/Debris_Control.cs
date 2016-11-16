using UnityEngine;
using System.Collections;

public class Debris_Control : MonoBehaviour {

    enum    State{
        Stay,
        Move,
    };

    //public  Transform   c_Target        =   null;
    public  int             c_TargetID      =   0;
    public  float           c_Score         =   0.0f;
    public  bool            c_ForMe         =   false;

    private float           c_MoveTime      =   1.0f;
    private State           m_State         =   State.Stay;

    //  移動関連
    private Vector3         m_StartPoint    =   Vector3.zero;
    private float           m_StartScale    =   1.0f;
    private float           m_StartScaleW   =   1.0f;
    private float           m_MoveTimer     =   0.0f;

    private float[]         c_MoveCurve     =   new float[]{    0.5f,   0.475f,  0.5f,   1.0f   };//new float[]{    0.5f,   0.35f,   0.5f,   1.0f    };
    private float[]         c_UpCurve       =   new float[]{    0.5f,   1.0f,   0.5f,   0.5f    };
    public  Transform       m_rTarget       =   null;

    private float           c_BirthTime     =   1.5f;
    private float           m_BearthTimer   =   0.0f;

    //  アクセス
    private LinkManager     m_rLinkManager  =   null;
    //private Flag_Control    m_rCentral      =   null;

	// Use this for initialization
	void    Start()
    {
        //  アクセス取得
        m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );
        //m_rCentral      =   FunctionManager.GetAccessComponent< Flag_Control >( "Central_Flag" );

        m_BearthTimer   =   c_BirthTime;
        m_StartScale    =   transform.localScale.x;
        m_StartScaleW   =   transform.lossyScale.x;
	}
	
	// Update is called once per frame
	void    Update()
    {
        //  アクセス取得
        if( !m_rLinkManager )   m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );

        //  タイマー更新
        m_BearthTimer   -=  Time.deltaTime;
        m_BearthTimer   =   Mathf.Max( m_BearthTimer, 0.0f );

        //  状態ごとの処理
	    switch( m_State ){
            case    State.Stay: Update_Stay();  break;
            case    State.Move: Update_Move();  break;
        }
	}
	void    Update_Stay()
    {

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
                //  スコア獲得（自分のプレイヤーだけ処理）
                if( m_rLinkManager ){

                    if( c_TargetID >= 0 ){

                    }
                }

                //  削除
                Destroy( gameObject );
                return;
            }
        }
    }
    public  void    SetMove( Transform _rTrans )
    {
        m_State         =   State.Move;
        m_StartPoint    =   transform.position;
        m_rTarget       =   _rTrans;
        
        //  コンポーネント削除
        Destroy( GetComponent< Rigidbody >() );
        Destroy( GetComponent< Collider >() );
    }
    public  bool    IsMove()
    {
        return  m_State == State.Move;
    }
}
