
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class NetPlayer_Control : NetworkBehaviour {

    [ SyncVar ]
    public  int         c_ClientID      =   0;
    [ SyncVar ]
    public  string      c_PlayerName    =   "Null";

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
            m_rLinkManager.m_LocalPlayerID      =   c_ClientID;
        }
        if( m_rLinkManager
        &&  isLocalPlayer ){
            m_rLinkManager.m_LocalPlayerID      =   c_ClientID;
            m_rLinkManager.m_rLocalPlayer       =   gameObject;
            m_rLinkManager.m_rLocalNPControl    =   this;
        }

        if( m_rGameManager
        &&  m_rGameManager.GetNumItem_PlayerName() > c_ClientID ){
            c_PlayerName    =   m_rGameManager.GetFromList_PlayerName( c_ClientID );
        }
	}
    public  override    void    OnStartServer()
    {
        base.OnStartServer();

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
    public  void        ChangeToCommander( bool _IsFriend )
    {
        TPSPlayer_Control   rTPSControl =   GetComponent< TPSPlayer_Control >();
        RTSPlayer_Control   rRTSControl =   GetComponent< RTSPlayer_Control >();
        if( rTPSControl ){
            rTPSControl.EndProc();
            rTPSControl.CmdChangeToCommander( _IsFriend );
        }
        if( rRTSControl ){
            rRTSControl.EndProc();
            rRTSControl.CmdChangeToCommander( _IsFriend );
        }
    }
//*********************************************************************************
//      コマンド
//*********************************************************************************
    //  エネミーへのダメージを送信
    [ Command ]
    public  void    CmdSendDamageEnemy( NetworkInstanceId _NetID, float _Damage, bool _HitWeak, bool _IsTowerAttack )
    {
        //  対象オブジェクトを探す
        NetworkIdentity rIdentity   =   FunctionManager.FindIdentityAtNetID( _NetID );
        if( !rIdentity )    return;

        //  コンポーネント取得
        Health          rHaelth     =   rIdentity.GetComponent< Health >();
        if( !rHaelth )      return;

        //  ダメージを与える
        rHaelth.GiveDamage( _Damage, connectionToClient.connectionId, _HitWeak, _IsTowerAttack );
    }
    //  プレイヤーへのダメージを送信
    [ Command ]
    public  void    CmdSendDamagePlayer( NetworkInstanceId _NetID, Vector3 _HitDirection, float _Damage )
    {
        //  対象オブジェクトを探す
        NetworkIdentity     rIdentity   =   FunctionManager.FindIdentityAtNetID( _NetID );
        if( !rIdentity )    return;

        //  コンポーネント取得
        TPSPlayer_HP        rHaelth     =   rIdentity.GetComponent< TPSPlayer_HP >();
        if( !rHaelth )      return;

        //  ダメージを与える
        rHaelth.SetDamage( _Damage );

        //  ダメージをクライアントに通知
        NetPlayer_Control   rNPControl  =   rIdentity.GetComponent< NetPlayer_Control >();
        if( !rNPControl )   return;

        //  リクエスト送信
        RpcDamagedPlayer( _HitDirection, rNPControl.c_ClientID );
    }
    //  オブジェクトへのダメージを送信
    [ Command ]
    public  void    CmdSendDamageExpObj( NetworkInstanceId _NetID, float _Damage )
    {
        //  対象オブジェクトを探す
        NetworkIdentity     rIdentity   =   FunctionManager.FindIdentityAtNetID( _NetID );
        if( !rIdentity )    return;

        //  コンポーネント取得
        DetonationObject    rHaelth     =   rIdentity.GetComponent< DetonationObject >();
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
    [ Command ]
    public  void    CmdFire_Server( Vector3 _Position, Vector3 _Target, int _WeaponID, int _ChildID )
    {
        Fire_InServer( _Position, _Target, connectionToClient.connectionId, _WeaponID, _ChildID );
    }
    //  オブジェクトの破壊を通知
    [ Command ]
    public  void    CmdDestroyResourceObj( NetworkInstanceId _NetID )
    {
        //  対象オブジェクトを探す
        NetworkIdentity     rIdentity   =   FunctionManager.FindIdentityAtNetID( _NetID );
        if( !rIdentity )    return;

        //  コンポーネントを取得
        ResourceObject_Control  rControl    =   rIdentity.GetComponent< ResourceObject_Control >();
        if( !rControl )     return;

        //  破壊処理
        rControl.Destroy( connectionToClient.connectionId );
    }
    //  データ更新
    [ Command ]
    public  void    CmdSend_GMIsReady( bool _IsReady )
    {
        m_rGameManager.SetToList_IsReady( connectionToClient.connectionId, _IsReady );
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
    //  地雷を起爆
    [ Command ]
    public  void    CmdDetonationMine( NetworkInstanceId _NetID )
    {
        //  対象オブジェクトを探す
        NetworkIdentity     rIdentity   =   FunctionManager.FindIdentityAtNetID( _NetID );
        if( !rIdentity )    return;

        //  コンポーネント取得
        Mine_Control        rMine       =   rIdentity.GetComponent< Mine_Control >();
        if( !rMine )        return;

        //  爆破
        rMine.Exploding( connectionToClient.connectionId );
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
    //  ダメージ通知
    [ ClientRpc ]
    void    RpcDamagedPlayer( Vector3 _HitDirection, int _ClientID )
    {
        //  指定されたクライアントでのみ処理を行う
        if( m_rLinkManager.m_LocalPlayerID != _ClientID )   return;

        //  ダメージエフェクト
        m_rGameManager.SetDamageFilterEffect();

        //  カメラシェイク
        Shaker_Control  rShaker     =   Camera.main.GetComponent< Shaker_Control >();
        if( rShaker ){
            Transform   rCamTrans   =   rShaker.transform;
            Vector3     vShake      =   rCamTrans.InverseTransformDirection( _HitDirection );
            float       shakePower  =   0.5f;
                        shakePower  =   Mathf.Min( shakePower, 1.0f );
            
            rShaker.SetShake( vShake, 0.5f, 0.1f, shakePower );
        }
    }
//*********************************************************************************
//      サーバー処理
//*********************************************************************************
    [ Server ]
    void    Fire_InServer( Vector3 _Position, Vector3 _Target, int _ShooterID, int _WeaponID, int _ChildID )
    {
        //  アクセス取得
        TPSShotController   rShot   =   transform.FindChild( "Weapons" ).GetChild( _ChildID ).GetComponent< TPSShotController >();
        if( !rShot )                                        return;

        //  発射
        rShot.Shot_InServer( _Position, _Target, _ShooterID, _WeaponID );
    }
}
