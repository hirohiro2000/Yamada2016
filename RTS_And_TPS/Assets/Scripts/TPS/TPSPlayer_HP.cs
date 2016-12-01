
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

[ RequireComponent( typeof( DamageBank ) ) ]
public class TPSPlayer_HP : NetworkBehaviour {

    public  bool                    m_IsEnemy       =   false;

    [ SyncVar ]
	public  float                   m_MaxHP         =   10.0f;
    [ SyncVar ]
	public  float                   m_CurHP         =   0.0f;
    [ SyncVar ]
    public  bool                    m_IsDying       =   false;

    private DamageBank              m_rDamageBank   =   null;
    private TPSPlayer_Control       m_rTPSControl   =   null;
    private RTSPlayer_Control       m_rRTSControl   =   null;
    private NetPlayer_Control       m_rNetPlayer    =   null;

    //  落下ダメージ用パラメータ
    private float                   c_DamageHeight  =   10.0f;
    private float                   c_HDRatio       =   1.0f;

    private CharacterController     m_rCharaCtrl    =   null;
    private bool                    m_PrevGrounded  =   false;
    private float                   m_LastHeight    =   0.0f;

    //  蘇生用パラメータ 
    private float                   c_RevivalTime   =   3.0f;
    private float                   c_RevivalRange  =   3.0f;
    private float                   c_DeathTime     =   60.0f;//90.0f;

    [ SyncVar ]
    private float                   m_RevivalTimer  =   0.0f;
    [ SyncVar ]
    private float                   m_DeathTimer    =   0.0f;
    
    //  外部へのアクセス
    private DamageFilter_Control    m_rDFCtrl       =   null;
    private DyingMessage_Control    m_rDMControl    =   null;
    private GageControl             m_rRevivalGage  =   null;
    private LinkManager             m_rLinkManager  =   null;
    private GameManager             m_rGameManager  =   null;

	//  Use this for initialization
	void    Start()
    {
        //  外部へのアクセス
        m_rDFCtrl       =   FunctionManager.GetAccessComponent< DamageFilter_Control >( "Canvas/DamageFilter" );
        m_rDMControl    =   FunctionManager.GetAccessComponent< DyingMessage_Control >( "Canvas/DyingMessage" );
        m_rRevivalGage  =   GameObject.Find( "Canvas" ).transform.FindChild( "RevivalGage" ).GetComponent< GageControl >();
        m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );
        m_rGameManager  =   FunctionManager.GetAccessComponent< GameManager >( "GameManager" );

        //  アクセスの取得
	    m_rDamageBank   =   GetComponent< DamageBank >();
        m_rCharaCtrl    =   GetComponent< CharacterController >();
        m_rTPSControl   =   GetComponent< TPSPlayer_Control >();
        m_rRTSControl   =   GetComponent< RTSPlayer_Control >();
        m_rNetPlayer    =   GetComponent< NetPlayer_Control >();

        //  パラメータ初期化
        m_CurHP         =   m_MaxHP;

        //  ローカルでのみ処理を行う
        if( isLocalPlayer ){
            //  ダメージ処理をセット
		    m_rDamageBank.AdvancedDamagedCallback   +=  DamageProc_CallBack;

            //  ＨＰバー初期化
            TPSHpBar.Initialize( m_MaxHP );
        }
        //  クローン側では敵プレイヤーからの攻撃のみ受け付ける
        else{
            //  ダメージ処理をセット
		    m_rDamageBank.AdvancedDamagedCallback   +=  DamageProc_CallBackEnemy;
        }

        //  敵プレイヤーの場合はエネミーリストに登録
        if( m_IsEnemy ){
            EnemyGenerator  rGenerator  =   GameObject.Find( "EnemySpawnRoot" ).GetComponent< EnemyGenerator >();
            rGenerator.GetCurrentHierachyList().Add( gameObject );
        }
	}
	void    DamageProc_CallBack( DamageResult _rDamageResult, CollisionInfo _rInfo )
    {
        //  受けたダメージ
        float   damage  =   _rDamageResult.GetTotalDamage();

        //  既に力尽きている場合は処理を行わない
        if( m_CurHP <= 0.0f )                                           return;
        //  ダメージがなければ終了
        if( damage <= 0.0f )                                            return;

        //  攻撃者へのアクセス
        Transform       rAttacker   =   _rInfo.attackedObject;
        if( !rAttacker )                                                return;

        //  ＴＰＳプレイヤーの攻撃へのアクセス
        TPSAttack_Net   rTPSATK     =   rAttacker.GetComponent< TPSAttack_Net >();
            //  ＴＰＳプレイヤーの攻撃は攻撃側のクライアントで処理する
            if( rTPSATK )                                               return;
        
        //  受けたダメージをサーバーに送信
        if( damage > 0.0f ){
            if( m_rRTSControl ) m_rRTSControl.CmdSendDamage( damage );
            if( m_rTPSControl ) m_rTPSControl.CmdSendDamage( damage );
        }

        //  カメラをシェイク
        {
            Shaker_Control  rShaker     =   Camera.main.GetComponent< Shaker_Control >();
            if( rShaker ){
                Vector3     atkPos      =   ( _rInfo.attackedObject )? _rInfo.attackedObject.position : Vector3.zero;
                Vector3     dfePos      =   ( _rInfo.damagedObject  )? _rInfo.damagedObject.position  : Vector3.zero;

                Transform   rCamTrans   =   rShaker.transform;
                Vector3     vShake      =   ( dfePos - atkPos ).normalized;
                            vShake      =   rCamTrans.InverseTransformDirection( vShake );
                float       shakePower  =   0.5f;
                            shakePower  =   Mathf.Min( shakePower, 1.0f );
                
                rShaker.SetShake( vShake, 0.5f, 0.1f, shakePower );
            }
        }
        //  ダメージエフェクト
        if( m_rDFCtrl ){
            m_rDFCtrl.SetEffect( 0.6f, 1.0f );
        }
    }
    void    DamageProc_CallBackEnemy( DamageResult _rDamageResult, CollisionInfo _rInfo )
    {
        //  既に力尽きている場合は処理を行わない
        if( m_CurHP <= 0.0f )                                           return;

        //  攻撃者へのアクセス
        Transform       rAttacker   =   _rInfo.attackedObject;
        if( !rAttacker )                                                return;

        //  ＴＰＳプレイヤーの攻撃へのアクセス
        TPSAttack_Net   rTPSATK     =   rAttacker.GetComponent< TPSAttack_Net >();

        //  ＴＰＳプレイヤーの攻撃以外は無視する 
        if( !rTPSATK )                                                  return;
        //  発射したプレイヤーのクライアント以外では処理を行わない 
        if( rTPSATK.c_AttackerID != m_rLinkManager.m_LocalPlayerID )    return;

        //  ダメージをサーバーに送信 
        if( m_rLinkManager.m_rLocalNPControl ){
            //  弱点に当たったかどうか
            bool    weakHit =   _rDamageResult.GetTotalDamage() > _rDamageResult.GetBaseDamage();
            //  弱点なら割合ダメージ
            float   myRatio =   1.0f;
            float   damage  =   ( weakHit )? m_MaxHP * myRatio * 0.2f * _rDamageResult.GetBaseDamage() : _rDamageResult.GetTotalDamage();

            //  送信
            {
                //  ダメージ方向計算
                Vector3 atkPos  =   ( _rInfo.attackedObject )? _rInfo.attackedObject.position : Vector3.zero;
                Vector3 dfePos  =   ( _rInfo.damagedObject  )? _rInfo.damagedObject.position  : Vector3.zero;
                Vector3 hitDir  =   ( dfePos - atkPos ).normalized;

                //  送信
                m_rLinkManager.m_rLocalNPControl.CmdSendDamagePlayer( netId, hitDir, damage );
            }
        }
    }

	//  Update is called once per frame
	void    Update()
    {
        //  サーバー側での処理
        if( NetworkServer.active )  Update_InServer();
	    //  ローカルでの処理（サーバーと重複する）
        if( isLocalPlayer )         Update_InLocal();
	}
    void    Update_InLocal()
    {
        //  落下ダメージ処理
        FallDamageProc();
        
        //  瀕死時の処理
        DyingProc();

        //  ＨＰバー更新
        TPSHpBar.SetHP( m_CurHP );
    }
    void    Update_InServer()
    {
        //  瀕死時のみ処理を行う
        if( !m_IsDying )    return;

        //  蘇生されている
        GameObject  rRivivalFriend  =   CheckResuscitatedByFriend();
        if( rRivivalFriend ){
            //  蘇生中
            m_RevivalTimer  +=  Time.deltaTime;
            m_RevivalTimer  =   Mathf.Min( m_RevivalTimer, c_RevivalTime );
            //  蘇生完了チェック
            if( m_RevivalTimer >= c_RevivalTime ){
                //  蘇生を記録
                m_rGameManager.SetToList_Rivival( rRivivalFriend.GetComponent< NetPlayer_Control >().c_ClientID, 1 );

                //  復活
                SetRecovery( m_MaxHP * 0.41f );

                //  タイマーリセット
                m_RevivalTimer  =   0.0f;
            }
        }
        //  蘇生されていない
        else{
            //  蘇生タイマーリセット
            m_RevivalTimer  =   0.0f;

            //  死亡タイマー更新
            m_DeathTimer    -=  Time.deltaTime;
            m_DeathTimer    =   Mathf.Max( m_DeathTimer, 0.0f );
            //  死亡チェック（一定時間経過または全滅した場合）
            if( m_DeathTimer <= 0.0f
            ||  CheckWhetherAliveFriend() == false ){
                //  死亡
                if( m_rRTSControl ) m_rRTSControl.RpcChangeToCommander( m_rNetPlayer.c_ClientID, !m_IsEnemy );
                if( m_rTPSControl ) m_rTPSControl.RpcChangeToCommander( m_rNetPlayer.c_ClientID, !m_IsEnemy );

                //  スクリプト制御停止
                this.enabled    =   false;
            }
        }
    }

    void    FallDamageProc()
    {
        if( !m_rCharaCtrl ) return;
        
        //  落下判定
        if( m_rCharaCtrl.isGrounded
        &&  m_PrevGrounded == false ){
            //  落下ダメージ計算
            float   damage  =   Mathf.Max( 0, ( m_LastHeight - transform.position.y ) - c_DamageHeight );
                    damage  =   damage * c_HDRatio;

            //  ダメージチェック
            if( damage > 0.0f ){
                //  ダメージを送信
                if( m_rRTSControl ) m_rRTSControl.CmdSendDamage( damage );
                if( m_rTPSControl ) m_rTPSControl.CmdSendDamage( damage );
                //  ダメージエフェクト
                if( m_rDFCtrl )     m_rDFCtrl.SetEffect( 0.6f, 1.5f );
            }

            //  カメラシェイク
            if( damage > 0.0f ){
                Shaker_Control  rShaker     =   Camera.main.GetComponent< Shaker_Control >();
                if( rShaker ){
                    Transform   rCamTrans   =   rShaker.transform;
                    Vector3     vShake      =   Vector3.up.normalized * damage;
                                vShake      =   rCamTrans.InverseTransformDirection( vShake );
                    float       shakePower  =   12.0f;//5 / vShake.magnitude;
                                //shakePower  =   Mathf.Min( shakePower, 1.0f );
            
                    rShaker.SetShake( vShake.normalized, 0.5f, 0.2f, shakePower );
                }
            }
        }

        //  ダメージ用の高さを更新（滞空中は最大高度のみ更新）
        if( m_rCharaCtrl.isGrounded )   m_LastHeight    =   transform.position.y;
        else                            m_LastHeight    =   Mathf.Max( m_LastHeight, transform.position.y );
        //  設置判定を保存
        m_PrevGrounded      =   m_rCharaCtrl.isGrounded;
    }
    void    DyingProc()
    {
        //  瀕死時の処理
        if( m_IsDying ){
            //  瀕死ＵＩ更新
            m_rDMControl.SetActive( true );
            m_rDMControl.SetTimeLimit( m_DeathTimer );

            //  蘇生ＵＩ更新
            m_rRevivalGage.SetGage( m_RevivalTimer / c_RevivalTime );
            m_rRevivalGage.gameObject.SetActive( m_RevivalTimer > 0.0f );
        }
        //  生存時の処理
        else{
            //  味方の蘇生中ならゲージを表示
            TPSPlayer_HP    rRevivalHP  =   CheckResuscitate();
            if( rRevivalHP )    m_rRevivalGage.SetGage( rRevivalHP.m_RevivalTimer / c_RevivalTime );
            
            //  アクティブ状態更新
            m_rDMControl.SetActive( false );
            m_rRevivalGage.gameObject.SetActive( rRevivalHP );
        }
    }

    //  仲間を蘇生しているかどうか
    TPSPlayer_HP    CheckResuscitate()
    {
        //  周辺のプレイヤーをチェック
        GameObject[]    rPlayers    =   GameObject.FindGameObjectsWithTag( "Player" );
        for( int i = 0; i < rPlayers.Length; i++ ){
            //  プレイヤーへのアクセス
            GameObject  rObj    =   rPlayers[ i ];
                //  自分は除外
                if( rObj == gameObject )    continue;

            //  距離をチェック
            Vector3     vToMe   =   transform.position - rObj.transform.position;
            float       dist    =   vToMe.magnitude;
                //  範囲外なら無効
                if( dist > c_RevivalRange ) continue;

            //  制御へのアクセス（元気な味方と敵は除外）
            TPSPlayer_HP    rHealth =   rObj.GetComponent< TPSPlayer_HP >();
                if( !rHealth )              continue;
                if( !rHealth.m_IsDying )    continue;
                if( rHealth.m_IsEnemy )     continue;

            //  蘇生中
            return  rHealth;
        }

        //  蘇生していない
        return  null;
    }
    //  仲間が蘇生しているかどうか
    GameObject      CheckResuscitatedByFriend()
    {
        //  周辺のプレイヤーをチェック
        GameObject[]    rPlayers    =   GameObject.FindGameObjectsWithTag( "Player" );
        for( int i = 0; i < rPlayers.Length; i++ ){
            //  プレイヤーへのアクセス
            GameObject  rObj    =   rPlayers[ i ];
                //  自分は除外
                if( rObj == gameObject )    continue;

            //  距離をチェック
            Vector3     vToMe   =   transform.position - rObj.transform.position;
            float       dist    =   vToMe.magnitude;
                //  範囲外なら無効
                if( dist > c_RevivalRange ) continue;

            //  制御へのアクセス（瀕死状態の味方と敵は除外）
            TPSPlayer_HP    rHealth =   rObj.GetComponent< TPSPlayer_HP >();
                if( !rHealth )              continue;
                if( rHealth.m_IsDying )     continue;
                if( rHealth.m_IsEnemy )     continue;

            //  蘇生可能
            return  rObj;
        }

        //  蘇生されていない
        return  null;
    }
    //  生存している味方がいるかどうか
    bool            CheckWhetherAliveFriend()
    {
        //  全てのプレイヤーへのアクセス
        GameObject[]    rPlayers    =   GameObject.FindGameObjectsWithTag( "Player" );
        for( int i = 0; i < rPlayers.Length; i++ ){
            //  プレイヤーへのアクセス
            GameObject  rObj    =   rPlayers[ i ];
                //  自分は除外
                if( rObj == gameObject )    continue;

            //  制御へのアクセス（瀕死状態の味方と敵は除外）
            TPSPlayer_HP    rHealth =   rObj.GetComponent< TPSPlayer_HP >();
                if( !rHealth )              continue;
                if( rHealth.m_IsDying )     continue;
                if( rHealth.m_IsEnemy )     continue;

            //  生存者発見
            return  true;
        }

        //  見つからなかった
        return  false;
    }

    //  外部からの操作
    public  void    SetDamage( float _Damage )
    {
        //  瀕死状態ではダメージを受けない
        if( m_IsDying ) return;

        //  ダメージを受ける 
        m_CurHP -=  _Damage;
        m_CurHP =   Mathf.Max( m_CurHP, 0.0f );
        m_CurHP =   Mathf.Min( m_CurHP, m_MaxHP );

        //  ダメージを記録
        m_rGameManager.SetToList_Damage( m_rNetPlayer.c_ClientID, Mathf.Max( _Damage, 0 ) );

        //  死亡チェック
        if( m_CurHP == 0.0f ){
            //  死亡をカウント
            m_rGameManager.SetToList_Death( m_rNetPlayer.c_ClientID, 1 );

            //  敵は死んだらすぐにコマンダーに戻る
            if( m_IsEnemy ){
                //  コマンダーに戻る
                if( m_rTPSControl ) m_rTPSControl.RpcChangeToCommander( m_rNetPlayer.c_ClientID, !m_IsEnemy );
                if( m_rRTSControl ) m_rRTSControl.RpcChangeToCommander( m_rNetPlayer.c_ClientID, !m_IsEnemy );
            }
            //  味方プレイヤーの場合
            else{
                //  生存者がいる場合は瀕死状態へ
                if( CheckWhetherAliveFriend() ){
                    m_IsDying       =   true;
                    m_DeathTimer    =   c_DeathTime;
                    m_RevivalTimer  =   0.0f;

                    //  他のプレイヤーに通知
                    m_rGameManager.RpcRecordNoticeE_ToOther( "仲間が救助を求めています！", m_rNetPlayer.c_ClientID );
                }
                //  生存者がいない場合はゲームオーバー
                else{
                    //  コマンダーに戻る
                    if( m_rTPSControl ) m_rTPSControl.RpcChangeToCommander( m_rNetPlayer.c_ClientID, !m_IsEnemy );
                    if( m_rRTSControl ) m_rRTSControl.RpcChangeToCommander( m_rNetPlayer.c_ClientID, !m_IsEnemy );
                }
            }
        }
    }
    public  void    SetRecovery( float _Recovery )
    {
        //  回復
        m_CurHP     +=  _Recovery;
        m_CurHP     =   Mathf.Max( m_CurHP, 0.0f );
        m_CurHP     =   Mathf.Min( m_CurHP, m_MaxHP );

        //  復活判定
        m_IsDying   =   m_CurHP == 0.0f;
    }

    //  アクセス
    public  float   GetDeathTimer()
    {
        return  m_DeathTimer;
    }
}
