
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class NetPlayer_Control : NetworkBehaviour {

    [ SyncVar ]
    public  int         c_ClientID      =   0;
    public  bool        c_IsMe          =   false;

    private LinkManager m_rLinkManager  =   null;

	// Use this for initialization
	void    Start()
    {
	    c_IsMe          =   isLocalPlayer;

        m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );
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
            m_rLinkManager.m_LocalPlayerID  =   c_ClientID;
            m_rLinkManager.m_rLocalPlayer   =   gameObject;
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
        GameObject  rObj    =   NetworkServer.objects[ _NetID ].gameObject;
        if( !rObj )     return;

        //  コンポーネント取得
        TPS_Enemy   rEnemy  =   rObj.GetComponent< TPS_Enemy >();
        if( !rEnemy )   return;

        //  ダメージを与える
        rEnemy.GiveDamage( _Damage );
    }
    //  発射コマンドを送信
    [ Command ]
    public  void    CmdFire_Client( Vector3 _Position, Vector3 _Target, int _ShooterID )
    {
        RpcFire_Client( _Position, _Target, _ShooterID );
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
