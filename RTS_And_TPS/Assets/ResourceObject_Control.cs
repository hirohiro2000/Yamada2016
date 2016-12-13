﻿
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class ResourceObject_Control : NetworkBehaviour {

    public  GameObject  c_ExplodedObj   =   null;
    public  GameObject  c_BreakEffect   =   null;

    [ SyncVar ]
    public  int         m_Resource      =   0;
    [ SyncVar ]
    private int         m_KillerID      =   -1;

    private bool        m_IsGameQuit    =   false;
    private GameManager m_rGameManager  =   null;
    private LinkManager m_rLinkManager  =   null;

	// Use this for initialization
	void    Start()
    {
	    //  アクセスの取得 
        m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );
        m_rGameManager  =   FunctionManager.GetAccessComponent< GameManager >( "GameManager" );

        //  ダメージ処理を登録
        GetComponent< DamageBank >().AdvancedDamagedCallback    +=  DamageProc_CallBack;
	}
    void    OnApplicationQuit()
    {
        m_IsGameQuit    =   true;
    }
    public  override    void    OnNetworkDestroy()
    {
        base.OnNetworkDestroy();

        //  ゲーム終了時は処理を行わない
        if( m_IsGameQuit )                                              return;
        //  ゲーム中以外は処理を行わない
        if( !m_rGameManager )                                           return;
        if( m_rGameManager.GetState() >= GameManager.State.GameOver )   return;

        //  破砕エフェクト
        if( c_BreakEffect ){
            Instantiate( c_BreakEffect, transform.position, transform.rotation );
        }

        //  破砕オブジェクト生成
        if( c_ExplodedObj
        &&  Camera.main ){
            GameObject  rObj    =   Instantiate( c_ExplodedObj );
            Transform   rTrans  =   rObj.transform;

            rTrans.position     =   transform.position;
            rTrans.localScale   =   transform.localScale;
            rTrans.rotation     =   transform.rotation;

            //  パラメータ設定
            ExpSylinder_Control rControl    =   rObj.GetComponent< ExpSylinder_Control >();
            GameObject          rPlayer     =   m_rLinkManager.m_rLocalPlayer;
            if( rPlayer ){
                rControl.c_PartnerID        =   m_KillerID;
                rControl.c_Score            =   m_Resource;
            }
        }

        //  カメラシェイク（撃破したプレイヤーのクライアントだけ）
        //if( m_KillerID == m_rLinkManager.m_LocalPlayerID ){
        //    Camera          rCamera     =   Camera.main;
        //    Shaker_Control  rShaker     =   ( rCamera )? rCamera.GetComponent< Shaker_Control >() : null;
        //    if( rShaker ){
        //        Transform   rCamTrans   =   rShaker.transform;
        //        Vector3     vToCamera   =   ( Camera.main.transform.position - transform.position );
        //        Vector3     vShake      =   vToCamera.normalized;
        //                    vShake      =   rCamTrans.InverseTransformDirection( vShake );
        //        float       shakePower  =   10 / vToCamera.magnitude;
        //                    shakePower  =   Mathf.Min( shakePower, 1.0f );
                
        //        rShaker.SetShake( vShake, 3.0f, 0.2f, shakePower );
        //    }
        //}
	}

    //  ダメージ処理
    void    DamageProc_CallBack( DamageResult _rDamageResult, CollisionInfo _rInfo )
    {
        //  ＴＰＳプレイヤーとのダメージ処理 
        DamageProc_WidthTPSPlayer( _rDamageResult, _rInfo );
    }
    void    DamageProc_WidthTPSPlayer( DamageResult _rDamageResult, CollisionInfo _rInfo )
    {
        Transform       rAttacker   =   _rInfo.attackedObject;
        if( !rAttacker )                                                return;

        TPSAttack_Net   rTPSATK     =   rAttacker.GetComponent< TPSAttack_Net >();

        //  TPSプレイヤーの攻撃以外は無視する 
        if( !rTPSATK )                                                  return;
        //  発射したプレイヤーのクライアント以外では処理を行わない 
        if( rTPSATK.c_AttackerID != m_rLinkManager.m_LocalPlayerID )    return;

        //  サーバーに破壊を通知する
        m_rLinkManager.m_rLocalNPControl.CmdDestroyResourceObj( netId );
    }
	
    //  当たり判定
    void    OnCollisionEnter( Collision _rCollision )
    {
        //  衝突したオブジェクトがプレイヤーかどうかチェック 
        GameObject  rHitObj =   _rCollision.gameObject;
        if( rHitObj.tag != "Player" )                                   return;

        //  ネットコントロールへのアクセスを取得
        NetPlayer_Control   rNetPlayer  =   rHitObj.GetComponentInParent< NetPlayer_Control >();
        if( !rNetPlayer )                                               return;

        //  衝突したのがほかのプレイヤーなら無視する
        if( rNetPlayer.c_ClientID != m_rLinkManager.m_LocalPlayerID )   return;

        //  サーバーに破壊を通知する
        m_rLinkManager.m_rLocalNPControl.CmdDestroyResourceObj( netId );
    }
    void    OnTriggerEnter( Collider _rCollider )
    {
        //  衝突したオブジェクトがプレイヤーかどうかチェック 
        GameObject  rHitObj =   _rCollider.gameObject;
        if( rHitObj.tag != "Player" )                                   return;

        //  ネットコントロールへのアクセスを取得
        NetPlayer_Control   rNetPlayer  =   rHitObj.GetComponentInParent< NetPlayer_Control >();
        if( !rNetPlayer )                                               return;

        //  衝突したのがほかのプレイヤーなら無視する
        if( rNetPlayer.c_ClientID != m_rLinkManager.m_LocalPlayerID )   return;

        //  サーバーに破壊を通知する
        m_rLinkManager.m_rLocalNPControl.CmdDestroyResourceObj( netId );
    }

    //  破壊
    public  void    Destroy( int _DestroyerID )
    {
        m_KillerID  =   _DestroyerID;

        Destroy( gameObject );
    }
}