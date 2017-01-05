
using   UnityEngine;
using   System.Collections;
using   System.Collections.Generic;

public class ExpSylinder_Control : MonoBehaviour {

    //public  float   c_ExplosionForce    =   0.0f;
    public  int                 c_Score         =   0;
    public  int                 c_PartnerID     =   0;
    public  bool                c_UseLightMode  =   true;
    public  Transform           m_rTargetTrans  =   null;
    private bool                m_ForMe         =   false;

    public  float               c_WaitTime      =   1.5f;
    private float               m_WaitTimer     =   0.0f;
    private float               c_CatchInterval =   0.0f;
    private float               m_CatchTimer    =   0.0f;

    //private int                 m_NumUpdate     =   0;
    //private bool                m_Start         =   false;

    private int                 c_SavingLine    =   3;
    private ExplodedManager     m_rExpManager   =   null;
    private bool                m_Removed       =   false;
    private int                 m_NumDebris     =   0;


	// Use this for initialization
    void    Awake()
    {
        m_NumDebris =   transform.childCount - 1;
    }
	void    Start()
    {
        //  アクセスを取得
        m_rExpManager   =   FunctionManager.GetAccessComponent< ExplodedManager >( "ExplodedManager" );

        //  当たり判定の設定
        if( m_rExpManager
        &&  c_UseLightMode ){
            //  個数を見て軽量処理にするかどうかを決定
            bool    doSaving    =   ( m_rExpManager.m_rExpObjList.Count + 1 >= c_SavingLine )? true : false;

            //  軽量化
            if( doSaving ){
                for( int i = 0; i < transform.childCount; i++ ){
                    Rigidbody   rRigid      =   transform.GetChild( i ).GetComponent< Rigidbody >();
                    if( !rRigid )   continue;

                    rRigid.velocity         =   Vector3.zero;
                    rRigid.angularVelocity  =   Vector3.zero;
                    rRigid.isKinematic      =   true;
                }
            }
            else{
                //  オブジェクトを登録
                m_rExpManager.m_rExpObjList.Add( gameObject );
            }
        }

        //  目標をセット
        UpdateTargetTrans();

        //  パラメーターセット
        m_WaitTimer     =   c_WaitTime;
	}
	
	// Update is called once per frame
	void    Update()
    {
        //  ターゲット監視
	    UpdateTargetTrans();

        //  タイマー更新
        m_WaitTimer -=  Time.deltaTime;
        m_WaitTimer =   Mathf.Max( m_WaitTimer, 0.0f );
        if( m_WaitTimer > 0.0f )    return;

        //  子を移動させる
        {
            m_CatchTimer    -=  Time.deltaTime;
            m_CatchTimer    =   Mathf.Max( m_CatchTimer, 0.0f );
            if( m_CatchTimer <= 0.0f ){
                m_CatchTimer    =   c_CatchInterval;

                bool    isAllMoved  =   true;
                for( int i = 1; i < transform.childCount; i++ ){
                    Debris_Control  rControl    =   transform.GetChild( i ).GetComponent< Debris_Control >();
                    if( rControl.IsMove() ) continue;

                    rControl.SetMove( m_rTargetTrans );
                    rControl.c_TargetID =   c_PartnerID;
                    rControl.c_Score    =   ( float )c_Score / Mathf.Max( 1, m_NumDebris );
                    rControl.c_ForMe    =   m_ForMe;

                    isAllMoved          =   false;
                    break;
                }

                if( !m_Removed
                &&  isAllMoved ){
                    m_rExpManager.m_rExpObjList.Remove( gameObject );
                }
            }
        }

        //  子がいなくなったら削除
	    if( transform.childCount <= 1 ){
            Destroy( gameObject );
            return;
        }
	}
    void    UpdateTargetTrans()
    {
        if( m_rTargetTrans != null )    return;

        //Debug.Log( "Call_Start" );

        GameObject[]    objList =   GameObject.FindGameObjectsWithTag( "Player" );
        for( int i = 0; i < objList.Length; i++ ){
            NetPlayer_Control   rControl    =   objList[ i ].GetComponent< NetPlayer_Control >();
            if( rControl == null )                      continue;
            if( rControl.c_ClientID != c_PartnerID ){
                //Debug.Log( "UnMatch_ID_" + rControl.c_ClientID + "_" + c_PartnerID );
                continue;
            }

            m_rTargetTrans  =   objList[ i ].transform;
            m_ForMe         =   rControl.c_IsMe;
            //Debug.Log( "Find_OK" );
            break;
        }
    }

    public  void    SetExplosionPoint( Vector3 _Point )
    {
        for( int i = 0; i < transform.childCount; i++ ){
            Rigidbody   rRigid  =   transform.GetChild( i ).GetComponent< Rigidbody >();
            if( !rRigid )   continue;

            rRigid.velocity         =   Vector3.zero;
            rRigid.angularVelocity  =   Vector3.zero;
            rRigid.isKinematic      =   true;
        }

        //
        Transform   rTrans  =   transform.GetChild( 0 );
        //float       scale   =   transform.localScale.x * 0.02f;
        rTrans.position     =   transform.position + ( _Point - transform.position ) * 1.5f;//_Point;
        rTrans.rotation     =   transform.rotation;
    }
}
