
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class Health : NetworkBehaviour {

    [SerializeField, HeaderAttribute("最大HP")]
    private float HP = 10.0f;

    private DamageBank  m_damage_bank   =   null;
    private LinkManager m_rLinkManager  =   null;

    void Awake()
    {
        m_damage_bank = transform.GetComponent<DamageBank>();
        m_damage_bank.AdvancedDamagedCallback   +=  DamageProc_CallBack;

        //  アクセスの取得 
        m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );
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

    }

    public  void    CorrectionHP(int level,float correcion_rate)
    {
        HP = HP * level * correcion_rate;
    }
    public  void    GiveDamage( float _Damage )
    {
        HP  -=  _Damage;
        HP  =   Mathf.Max( HP, 0.0f );
        if( HP <= 0.0f ){
            Destroy( gameObject );
        }
    }
}
