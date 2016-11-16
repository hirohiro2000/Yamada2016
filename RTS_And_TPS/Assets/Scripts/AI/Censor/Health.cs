
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class Health : NetworkBehaviour {

    public  GameObject  c_ExplodedObj   =   null;
	public	GameObject	c_Explosion		=	null;

    public  int         Resource        =   0;
    public  int         Score           =   0;

    [ SerializeField, HeaderAttribute("最大HP"), SyncVar ]
    private float       MaxHP           =   10.0f;
    [ SyncVar ]
    private float       HP              =   0.0f;
	[ SyncVar ]
    private int         Level           =   0;

    private DamageBank  m_damage_bank   =   null;
    private LinkManager m_rLinkManager  =   null;
    private GameManager m_rGameManager  =   null;

    void Awake()
    {
        m_damage_bank = transform.GetComponent<DamageBank>();
        m_damage_bank.AdvancedDamagedCallback   +=  DamageProc_CallBack;

        //  アクセスの取得 
        m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );
        m_rGameManager  =   FunctionManager.GetAccessComponent< GameManager >( "GameManager" );
    }
    public  override    void    OnNetworkDestroy()
    {
        base.OnNetworkDestroy();

        //  破砕オブジェクト生成
        if( c_ExplodedObj ){
            GameObject  rObj    =   Instantiate( c_ExplodedObj );
            Transform   rTrans  =   rObj.transform;

            rTrans.position     =   transform.position;
            rTrans.localScale   =   transform.localScale;
            rTrans.rotation     =   transform.rotation;

            //  パラメータ設定
            ExpSylinder_Control rControl    =   rObj.GetComponent< ExpSylinder_Control >();
            GameObject          rPlayer     =   m_rLinkManager.m_rLocalPlayer;
            if( rPlayer ){
                rControl.m_rTargetTrans     =   rPlayer.transform;
            }
        }

		//ここで爆発
		Instantiate(c_Explosion, transform.position, Quaternion.identity);

	}


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //  ダメージ処理
    void    DamageProc_CallBack( DamageResult _rDamageResult, CollisionInfo _rInfo )
    {
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
        m_rLinkManager.m_rLocalNPControl.CmdSendDamageEnemy( netId, _rDamageResult.GetTotalDamage() );
    }
    void    DamageProc_WidthTower( DamageResult _rDamageResult, CollisionInfo _rInfo )
    {
        //  サーバーでのみ処理を行う
        if( !NetworkServer.active ) return;

        //  ダメージを受ける
        GiveDamage( _rDamageResult.GetTotalDamage() );
    }

    public  void    CorrectionHP(int level,float correcion_rate)
    {
        MaxHP   =   MaxHP * level * correcion_rate;
        HP      =   MaxHP;
        Level   =   level;
    }
    public  void    GiveDamage( float _Damage )
    {
        HP  -=  _Damage;
        HP  =   Mathf.Max( HP, 0.0f );
        if( HP <= 0.0f ){
            //  スコアとリソースを獲得
            m_rGameManager.AddResource( Resource + Level );
            m_rGameManager.AddGlobalScore( Score + Level );

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
