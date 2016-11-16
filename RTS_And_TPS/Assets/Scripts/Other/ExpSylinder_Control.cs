
using   UnityEngine;
using   System.Collections;
using   System.Collections.Generic;

public class ExpSylinder_Control : MonoBehaviour {

    //public  float   c_ExplosionForce    =   0.0f;
    public  int                 c_Score         =   0;
    public  int                 c_PartnerID     =   0;
    public  Transform           m_rTargetTrans  =   null;
    private bool                m_ForMe         =   false;

    private float               c_WaitTime      =   1.5f;
    private float               m_WaitTimer     =   0.0f;
    private float               c_CatchInterval =   0.0f;
    private float               m_CatchTimer    =   0.0f;

    private int                 m_NumUpdate     =   0;
    private bool                m_Start         =   false;

    private int                 c_SavingLine    =   4;


	// Use this for initialization
	void    Start()
    {
        UpdateTargetTrans();

        //  パラメーターセット
        m_WaitTimer     =   c_WaitTime;

        //for( int i = 0; i < transform.childCount; i++ ){
        //    Rigidbody   rRigid  =   transform.GetChild( i ).GetComponent< Rigidbody >();
        //    if( !rRigid )   continue;

        //    rRigid.velocity         =   Vector3.zero;
        //    rRigid.angularVelocity  =   Vector3.zero;
        //    rRigid.isKinematic      =   false;
        //}
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

                for( int i = 1; i < transform.childCount; i++ ){
                    Debris_Control  rControl    =   transform.GetChild( i ).GetComponent< Debris_Control >();
                    if( rControl.IsMove() ) continue;

                    rControl.SetMove( m_rTargetTrans );
                    rControl.c_TargetID =   c_PartnerID;
                    rControl.c_Score    =   ( float )c_Score / Mathf.Max( 1, ( transform.childCount - 1 ) );
                    rControl.c_ForMe    =   m_ForMe;
                    //Debug.Log( "MoveStart" );
                    break;
                }
            }
        }

        //  子がいなくなったら削除
	    if( transform.childCount <= 1 ){
            Destroy( gameObject );
            return;
        }
	}
    void    FixedUpdate()
    {
        if( !m_Start ){
            if( m_NumUpdate++ == 3 ){
                m_Start =   true;

                for( int i = 0; i < transform.childCount; i++ ){
                    Rigidbody   rRigid  =   transform.GetChild( i ).GetComponent< Rigidbody >();
                    if( !rRigid )   continue;

                    rRigid.velocity         =   Vector3.zero;
                    rRigid.angularVelocity  =   Vector3.zero;
                    rRigid.isKinematic      =   false;
                }
            }
        }
    }
    void    UpdateTargetTrans()
    {
        if( m_rTargetTrans != null )    return;

        //Debug.Log( "Call_Start" );

        //GameObject[]    objList =   GameObject.FindGameObjectsWithTag( "Player" );
        //for( int i = 0; i < objList.Length; i++ ){
        //    ClientID_Control    rControl    =   objList[ i ].GetComponent< ClientID_Control >();
        //    if( rControl == null )                      continue;
        //    if( rControl.c_ClientID != c_PartnerID ){
        //        //Debug.Log( "UnMatch_ID_" + rControl.c_ClientID + "_" + c_PartnerID );
        //        continue;
        //    }

        //    m_rTargetTrans  =   objList[ i ].transform;
        //    m_ForMe         =   rControl.c_IsMe;
        //    //Debug.Log( "Find_OK" );
        //    break;
        //}
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

        //  当たり判定の設定
        //{
        //    LinkManager         rLinkManager    =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );
        //    List< Collider >    rColList        =   rLinkManager.GetDebrisList();

        //    //  個数を見て軽量処理にするかどうかを決定
        //    bool    doSaving    =   ( rLinkManager.GetExpObjList().Count + 1 >= c_SavingLine )? true : false;

        //    //  軽量化
        //    if( doSaving ){
        //        //  レイヤー変更
        //        for( int i = 0; i < transform.childCount; i++ ){
        //            transform.GetChild( i ).gameObject.layer    =   LayerMask.NameToLayer( "Debris_Light" );
        //        }
        //    }
        //    else{
        //        //  オブジェクトを登録
        //        rLinkManager.GetExpObjList().Add( gameObject );

        //        //  ほかのオブジェクトの破片とは当たらないようにする
        //        for( int i = 1; i < transform.childCount; i++ ){
        //            Collider    rCollider   =   transform.GetChild( i ).GetComponent< Collider >();
        //            //  
        //            for( int n = 0; n < rColList.Count; n++ ){
        //                if( !rColList[ n ] ) continue;

        //                Physics.IgnoreCollision( rCollider, rColList[ n ] );
        //            }
        //        }
        //        //  破片を登録
        //        for( int i = 1; i < transform.childCount; i++ ){
        //            Collider    rCollider   =   transform.GetChild( i ).GetComponent< Collider >();

        //            rColList.Remove( rCollider );
        //            rColList.Add( rCollider );
        //        }
        //    }
        //}

        //
        Transform   rTrans  =   transform.GetChild( 0 );
        float       scale   =   transform.localScale.x * 0.02f;
        rTrans.position     =   transform.position + ( _Point - transform.position ) * 1.5f;//_Point;
        rTrans.rotation     =   transform.rotation;
    }
}
