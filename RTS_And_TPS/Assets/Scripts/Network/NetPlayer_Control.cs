
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class NetPlayer_Control : NetworkBehaviour {

    [ SyncVar ]
    public  int         c_ClientID      =   0;
    public  bool        c_IsMe          =   false;

    private LinkManager m_rLinkManager  =   null;
    private GameManager m_rGameManager  =   null;

	// Use this for initialization
	void    Start()
    {
	    c_IsMe          =   isLocalPlayer;

        m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );
        m_rGameManager  =   FunctionManager.GetAccessComponent< GameManager >( "GameManager" );
        if( m_rLinkManager ){
            m_rLinkManager.m_LocalPlayerID  =   c_ClientID;
        }
        if( m_rLinkManager
        &&  isLocalPlayer ){
            m_rLinkManager.m_LocalPlayerID  =   c_ClientID;
            m_rLinkManager.m_rLocalPlayer   =   gameObject;
        }
	}
    public  override    void    OnStartServer()
    {
        c_ClientID      =   connectionToClient.connectionId;
    }
	
	// Update is called once per frame
	void    Update()
    {
	    if( !m_rLinkManager )   m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );
        if( !m_rLinkManager )   return;

        if( isLocalPlayer ){
            m_rLinkManager.m_LocalPlayerID      =   c_ClientID;
            m_rLinkManager.m_rLocalPlayer       =   gameObject;
            m_rLinkManager.m_rLocalNPControl    =   this;
        }
	}

//*********************************************************************************
//      コマンド
//*********************************************************************************
    //  エネミーへのダメージを送信
    [ Command ]
    public  void    CmdSendDamageEnemy( NetworkInstanceId _NetID, float _Damage )
    {
        //  対象オブジェクトを探す
        NetworkIdentity rIdentity   =   FunctionManager.FindIdentityAtNetID( _NetID );
        if( !rIdentity )  return;

        //  コンポーネント取得
        TPS_Enemy       rEnemy      =   rIdentity.GetComponent< TPS_Enemy >();
        if( !rEnemy )   return;

        //  ダメージを与える
        rEnemy.GiveDamage( _Damage );
    }
    [ Command ]
    public  void    CmdSendDamageEnemy_RTS( NetworkInstanceId _NetID, float _Damage )
    {
        //  対象オブジェクトを探す
        NetworkIdentity rIdentity   =   FunctionManager.FindIdentityAtNetID( _NetID );
        if( !rIdentity )    return;

        //  コンポーネント取得
        CollisionParam  rParam  =   rIdentity.GetComponent< CollisionParam >();
        if( !rParam )   return;

        //  ダメージを与える
        rParam.m_hp     -=  ( int )_Damage;
        if( rParam.m_hp <= 0.0f ){
            RTSEnemy    rRTSEnemy   =   rParam.GetComponent< RTSEnemy >();
            rRTSEnemy.Death_Proc();
        }
    }
    //  発射コマンドを送信
    [ Command ]
    public  void    CmdFire_Client( Vector3 _Position, Vector3 _Target, int _ShooterID )
    {
        RpcFire_Client( _Position, _Target, _ShooterID );
    }
    //  データ更新
    [ Command ]
    public  void    CmdSend_GMIsReady( bool _IsReady )
    {
        m_rGameManager.SetIsReady( connectionToClient.connectionId, _IsReady );
    }
    //  ゲームスピード更新
    [ Command ]
    public  void    CmdChange_GameSpeed( float _GameSpeed )
    {
        m_rGameManager.m_GameSpeed  =   _GameSpeed;
    }
//*********************************************************************************
//      リクエスト
//*********************************************************************************
    //  発射リクエスト
    [ ClientRpc ]
    void    RpcFire_Client( Vector3 _Position, Vector3 _Target, int _ShooterID )
    {
        //  発射元のクライアントでは発射しない
        if( _ShooterID == m_rLinkManager.m_LocalPlayerID )  return;
        
        //  アクセス取得
        TPSShotController   rShot   =   transform.FindChild( "Weapons" ).FindChild( "NormalGun" ).GetComponent< TPSShotController >();
        if( !rShot )                                        return;

        //  発射
        rShot.Shot_ByRequest( _Position, _Target, _ShooterID );
    }
}
