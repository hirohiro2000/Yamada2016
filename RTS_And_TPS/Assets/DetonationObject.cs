
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class DetonationObject : NetworkBehaviour {

    [ SyncVar ]
    public  float           c_MaxHP         =   10.0f;
    [ SyncVar ]
    public  float           m_CurHP         =   0.0f;
    [ SyncVar ]
    public  int             m_DestroyerID   =   0;

    private float           c_ExpDelay      =   0.07f;//0.35f;
    private float           m_ExpTimer      =   0.0f;
    private bool            m_Destroyed     =   false;

    private DamageBank      m_rDamageBack   =   null;
    private BombExplosion   m_rBomb         =   null;

    private LinkManager     m_rLinkManager  =   null;

	// Use this for initialization
	void    Start()
    {
        //  アクセスの取得
        m_rDamageBack   =   GetComponent< DamageBank >();
        m_rBomb         =   GetComponent< BombExplosion >();
        m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );

        //  ダメージ処理を設定
	    m_rDamageBack.AdvancedDamagedCallback   +=  DamageProc_CallBack;

        //  パラメータを初期化
        m_CurHP         =   c_MaxHP;
	}
    public  override    void    OnNetworkDestroy()
    {
        base.OnNetworkDestroy();

        if( m_DestroyerID != -1 ){
            m_rBomb.c_AttackerID    =   m_DestroyerID;
            m_rBomb.Explosion();
        }
    }

    //  ダメージ処理 
	void    DamageProc_CallBack( DamageResult _rDamageResult, CollisionInfo _rInfo )
    {
        //  受けたダメージ
        float   damage  =   _rDamageResult.GetTotalDamage();

        //  既に力尽きている場合は処理を行わない
        if( m_CurHP <= 0.0f )                                           return;
        //  ダメージがなければ終了
        if( damage <= 0.0f )                                            return;
        
        //  アタッカーへのアクセス
        Transform       rAttacker   =   _rInfo.attackedObject;
        if( !rAttacker )                                                return;

        //  ＴＰＳプレイヤーの攻撃コンポーネント
        TPSAttack_Net   rTPSATK     =   rAttacker.GetComponent< TPSAttack_Net >();

        //  TPSプレイヤーの攻撃以外は無視する 
        if( !rTPSATK )                                                  return;

        //  発射したプレイヤーのクライアント以外では処理を行わない 
        if( rTPSATK.c_AttackerID != m_rLinkManager.m_LocalPlayerID )    return;

        //  ダメージをサーバーに送信 
        if( m_rLinkManager.m_rLocalNPControl ){
            //  送信
            m_rLinkManager.m_rLocalNPControl.CmdSendDamageExpObj( netId, damage );
        }
    }

	// Update is called once per frame
	void    Update()
    {
        //  アクセスの取得
        if( !m_rLinkManager )   m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );

        //  オーナー設定
        //m_rBomb.c_AttackerID    =   m_DestroyerID;

        //  削除準備中は非アクティブ化
        if( m_DestroyerID == -1 )   gameObject.SetActive( false );

        //  サーバーでの更新処理
        if( NetworkServer.active )  Update_InServer();
	}
    void    Update_InServer()
    {
        //  既に破壊されている場合
        if( m_Destroyed ){
            m_ExpTimer  -=  Time.deltaTime;
            m_ExpTimer  =   Mathf.Max( 0.0f, m_ExpTimer );
            if( m_ExpTimer <= 0.0f ){
                //  削除
                Destroy( gameObject );
            }
        }
    }
    //  爆破
    void    Explosion()
    {
        //  破壊者設定
        BombExplosion   rBE =   GetComponent< BombExplosion >();
        rBE.c_AttackerID    =   m_DestroyerID;

        //  オブジェクトを削除
        Destroy( gameObject );
    }

    //  アクセス
    [ Server ]
    public  void    GiveDamage( float _Damage, int _AttackerID )
    {
        //  耐久力が尽きている場合は処理を行わない
        if( m_CurHP <= 0.0f )   return;

        //  ダメージを受ける
        m_CurHP     -=  _Damage;
        m_CurHP     =   Mathf.Max( m_CurHP, 0.0f );

        //  死亡
        if( m_CurHP <= 0.0f ){
            //  爆破タイマーセット
            m_ExpTimer      =   c_ExpDelay;
            m_Destroyed     =   true;

            //  攻撃者を保存
            m_DestroyerID   =   _AttackerID;
        }
    }
}
