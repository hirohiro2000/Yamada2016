
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
//      アクセス
//*********************************************************************************
    //  コマンダーに戻る
    public  void        ChangeToCommander()
    {
        TPSPlayer_Control   rTPSControl =   GetComponent< TPSPlayer_Control >();
        RTSPlayer_Control   rRTSControl =   GetComponent< RTSPlayer_Control >();
        if( rTPSControl ){
            rTPSControl.EndProc();
            rTPSControl.CmdChangeToCommander();
        }
        if( rRTSControl ){
            rRTSControl.EndProc();
            rRTSControl.CmdChangeToCommander();
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
        if( !rIdentity )    return;

        //  コンポーネント取得
        Health          rHaelth     =   rIdentity.GetComponent< Health >();
        if( !rHaelth )      return;

        //  ダメージを与える
        rHaelth.GiveDamage( _Damage, connectionToClient.connectionId );
    }
    //  発射コマンドを送信
    [ Command ]
    public  void    CmdFire_Client( Vector3 _Position, Vector3 _Target, int _WeaponID, int _ChildID )
    {
        RpcFire_Client( _Position, _Target, connectionToClient.connectionId, _WeaponID, _ChildID );
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
    //  スコア加算
    [ Command ]
    public  void    CmdAddResource( float _AddResource )
    {
        m_rGameManager.AddResource( _AddResource );
    }
//*********************************************************************************
//      リクエスト
//*********************************************************************************
    //  発射リクエスト
    [ ClientRpc ]
    void    RpcFire_Client( Vector3 _Position, Vector3 _Target, int _ShooterID, int _WeaponID, int _ChildID )
    {
        //  発射元のクライアントでは発射しない
        if( _ShooterID == m_rLinkManager.m_LocalPlayerID )  return;
        
        //  アクセス取得
        TPSShotController   rShot   =   transform.FindChild( "Weapons" ).GetChild( _ChildID ).GetComponent< TPSShotController >();
        if( !rShot )                                        return;

        //  発射
        rShot.Shot_ByRequest( _Position, _Target, _ShooterID, _WeaponID );
    }
}
