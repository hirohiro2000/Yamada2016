
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class Health : NetworkBehaviour {

    //  ダメージタイプ
    public  enum    DamageType{
        TPSAttack,
        RTSAttack_Gun,
        RTSAttack_Rifle,
        RTSAttack_Laser,
        RTSAttack_Bomb,
        RTSAttack_Missile,
    }

    public  GameObject      c_ExplodedObj   =   null;
    public  GameObject[]    c_DeathEmission =   null;

    public  GameObject      c_HitEmission   =   null;
    public  GameObject      c_WeakEmission  =   null;
    public  GameObject      c_THitEmission  =   null;

    [ SyncVar ]
    public  int         Resource        =   0;
    public  int         Score           =   0;

    [ SerializeField, HeaderAttribute( "最大HP" ) ]
    private float       DefaultHP       =   10.0f;
    [ SyncVar ]
    private float       MaxHP           =   10.0f;
    [ SyncVar ]
    private float       HP              =   0.0f;
	[ SyncVar ]
    private int         Level           =   0;

    [ SyncVar ]
    private int         KillerID        =   0;

    private DamageBank  m_damage_bank   =   null;
    private LinkManager m_rLinkManager  =   null;
    private GameManager m_rGameManager  =   null;

    private bool        m_IsGameQuit    =   false;

    void    Awake()
    {
        m_damage_bank = transform.GetComponent<DamageBank>();
        m_damage_bank.AdvancedDamagedCallback   +=  DamageProc_CallBack;

        //  アクセスの取得 
        m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );
        m_rGameManager  =   FunctionManager.GetAccessComponent< GameManager >( "GameManager" );
    }
    void    OnApplicationQuit()
    {
        m_IsGameQuit    =   true;
    }
    public  override    void    OnNetworkDestroy()
    {
        base.OnNetworkDestroy();

        //  ゲーム終了時は処理を行わない
        if( m_IsGameQuit )                                          return;
        //  ゲーム中以外は処理を行わない
        if( !m_rGameManager )                                       return;
        if( m_rGameManager.GetState() != GameManager.State.InGame ) return;

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
                rControl.c_PartnerID        =   KillerID;
                rControl.c_Score            =   Resource;// + Mathf.Max( Level - 1, 0 );
            }
        }

        //  死亡時の生成オブジェクト 
        if( c_DeathEmission != null ){
            for( int i = 0; i < c_DeathEmission.Length; i++ ){
                if( !c_DeathEmission[ i ] ) continue;

                //  生成
                GameObject  rObj    =   Instantiate( c_DeathEmission[ i ] );
                Transform   rTrans  =   rObj.transform;
                rTrans.position     =   transform.position;
            }
        }

        //  カメラシェイク（撃破したプレイヤーのクライアントだけ）
        if( KillerID == m_rLinkManager.m_LocalPlayerID ){
            Camera          rCamera     =   Camera.main;
            Shaker_Control  rShaker     =   ( rCamera )? rCamera.GetComponent< Shaker_Control >() : null;
            if( rShaker ){
                Transform   rCamTrans   =   rShaker.transform;
                Vector3     vToCamera   =   ( Camera.main.transform.position - transform.position );
                Vector3     vShake      =   vToCamera.normalized;
                            vShake      =   rCamTrans.InverseTransformDirection( vShake );
                float       shakePower  =   10 / vToCamera.magnitude;
                            shakePower  =   Mathf.Min( shakePower, 1.0f );
                
                rShaker.SetShake( vShake, 3.0f, 0.2f, shakePower );
            }
        }
	}


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        //if( Input.GetKeyDown( KeyCode.B ) ){
        //    GiveDamage( 1000.0f, 0 );
        //}
	}

    //  ダメージ処理
    void    DamageProc_CallBack( DamageResult _rDamageResult, CollisionInfo _rInfo )
    {
        //  既に死亡している場合は処理を行わない
        if( HP <= 0.0f )    return;

        //  ヒット音 
        {
            bool    weakHit =   _rDamageResult.GetTotalDamage() > _rDamageResult.GetBaseDamage();
            bool    isRTS   =   _rInfo.attackedObject.GetComponent< RTSAttack_Net >();

            GameObject  rEmit   =   ( weakHit )? c_WeakEmission : c_HitEmission;
                        rEmit   =   ( isRTS )?   c_THitEmission : rEmit;

            if( rEmit ){
                GameObject  rObj    =   Instantiate( rEmit );
                Transform   rTrans  =   rObj.transform;
                rTrans.position     =   _rInfo.contactPoint;
            }
        }

        //  ＴＰＳプレイヤーとのダメージ処理 
        DamageProc_WidthTPSPlayer( _rDamageResult, _rInfo );
        //  タワーとのダメージ処理
        DamageProc_WidthTower( _rDamageResult, _rInfo );
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

        //  ダメージをサーバーに送信 
        if( m_rLinkManager.m_rLocalNPControl ){
            //  弱点に当たったかどうか
            bool    weakHit =   _rDamageResult.GetTotalDamage() > _rDamageResult.GetBaseDamage()
                            &&  _rDamageResult.GetTotalDamage() / _rDamageResult.GetBaseDamage() < 6.0f;
            //  弱点なら割合ダメージ 
            float   myRatio =   10 / DefaultHP;
            float   damage  =   ( weakHit )? MaxHP * myRatio * 0.2f * _rDamageResult.GetBaseDamage(): _rDamageResult.GetTotalDamage();

            //  送信
            m_rLinkManager.m_rLocalNPControl.CmdSendDamageEnemy( netId, damage, weakHit, false );
        }
    }
    void    DamageProc_WidthTower( DamageResult _rDamageResult, CollisionInfo _rInfo )
    {
        Transform       rAttacker   =   _rInfo.attackedObject;
        if( !rAttacker )                                                return;

        RTSAttack_Net   rRTSATK     =   rAttacker.GetComponent< RTSAttack_Net >();

        //  タワーの攻撃以外は無視する 
        if( !rRTSATK )                                                  return;
        //  設置したプレイヤーのクライアント以外では処理を行わない 
        if( rRTSATK.c_AttackerID != m_rLinkManager.m_LocalPlayerID )    return;

        //  ダメージをサーバーに送信
        if( m_rLinkManager.m_rLocalNPControl ){
            m_rLinkManager.m_rLocalNPControl.CmdSendDamageEnemy( netId, _rDamageResult.GetTotalDamage(), false, true );
        }
    }

    public  void    CorrectionHP(int level,float correcion_rate)
    {
        MaxHP   =   DefaultHP + (level * correcion_rate);
        HP      =   MaxHP;
        Level   =   level;
    }
    public  void    GiveDamage( float _Damage, int _AttackerID, bool _HitWeak, bool _IsTowerAttack )
    {
        //  既に死んでいる場合は処理を行わない
        if( HP <= 0.0f )    return;

        //  攻撃者のＩＤを保存
        KillerID    =   _AttackerID;

        //  ダメージを与える
        HP  -=  _Damage;
        HP  =   Mathf.Max( HP, 0.0f );
        if( HP <= 0.0f ){
            //  スコアを獲得
            m_rGameManager.AddGlobalScore( Score, KillerID );

            //  スコア獲得通知
            int score   =   ( int )( Score + Score * 0.2f * Mathf.Max( 0, Level - 1 ) );
            if( NetworkServer.active )  m_rGameManager.SetAcqScoreNotrice( score, KillerID );
                                        m_rGameManager.RpcGetScoreNotice( score, KillerID );
            //  ヘッドショット
            if( _HitWeak ){
                //  レコードを通知
                if( NetworkServer.active )  m_rGameManager.SetAcqRecord     ( "ヘッドショットキル！ + 20", KillerID );
                                            m_rGameManager.RpcRecordNotice  ( "ヘッドショットキル！ + 20", KillerID );
                //  スコアを獲得
                m_rGameManager.AddGlobalScore( 20, KillerID );
                //  獲得リソース増加
                Resource    +=  2;

                //  ヘッドショットキルを記録
                m_rGameManager.SetToList_HSKill( KillerID, 1 );
            }

            //  キルを通知（RTS） 
            if( _IsTowerAttack ){
                if( NetworkServer.active )  m_rGameManager.SetAcqRecord     ( "あなたの防衛施設が敵を撃破しました！", KillerID );
                                            m_rGameManager.RpcRecordNotice  ( "あなたの防衛施設が敵を撃破しました！", KillerID );
            }

            //  キルを記録
            m_rGameManager.SetToList_Kill( KillerID, 1 );

            //  オブジェクトを破棄
            Destroy( gameObject );
        }
    }
    
    public float GetHP()    { return HP;    }
    public float GetMaxHP() { return MaxHP; }
    public float GetLevel() { return Level; }


//====================================================================================
//      リクエスト
//====================================================================================
}
