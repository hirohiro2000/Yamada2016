
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class TPSPlayer_Control : NetworkBehaviour {

    public  Camera              c_rMyCamera         =   null;
    public  GameObject          c_Commander         =   null;

    public  TPSShotController[] c_ShotController    =   null;

    private LinkManager         m_rLinkManager      =   null;

    private bool                m_IsLock            =   true;

    private TPSPlayer_HP        m_rHP               =   null;
    private GameObject          m_rRideButton       =   null;
    private GameObject          m_rGetOffButton     =   null;

	// Use this for initialization
	void    Start()
    {
        m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );
        m_rHP           =   GetComponent< TPSPlayer_HP >();
        
        //  自分のキャラクターのみ処理を行う
        if( isLocalPlayer ){
            //  開始処理
            StartProc();
        }
        //  自分のキャラクター以外
        else{
            c_rMyCamera.enabled =   false;

            //  リジッドボディ無効化
            GetComponent< Rigidbody >().isKinematic =   true;
        }

        //  出撃ボイス
        SoundController.PlayNow( "Voice_R_Launch", transform, transform.position, 0.0f, 1.0f, 1.0f, 10.0f );
	}
	
	// Update is called once per frame
	void    Update()
    {
        //  自分のキャラクターのみ処理を行う
        if( !isLocalPlayer )    return;
        
        //  コマンダーに戻る（デバッグ用）
        if( Input.GetKeyDown( KeyCode.Delete ) ){
            //  終了処理
            EndProc();
            //  コマンダーに戻る
            CmdChangeToCommander( false );
            return;
        }

        //  ロックフラグ切り替え
        if( Input.GetKeyDown( KeyCode.Escape ) )    m_IsLock    =   !m_IsLock;
        
        //  マウスカーソルをウィンドウに固定する
        if( m_IsLock ){
            Cursor.lockState    =   CursorLockMode.Locked;
            Cursor.visible      =   false;
        }
        else{
            Cursor.lockState    =   CursorLockMode.None;
            Cursor.visible      =   true;
        }

        //  搭乗関係の処理
        {
            //  搭乗チェック、接近チェック
            GameObject  rAroundGirl =   FindAroundGirl( 4.5f );
            bool        rideGirl    =   CheckWhetherIsBoardingGirl();
            bool        aroundGirl  =   rAroundGirl;
            bool        isAlive     =   m_rHP.m_CurHP > 0;

            //  ボタンの表示
            m_rRideButton.SetActive( !rideGirl && aroundGirl && isAlive );
            m_rGetOffButton.SetActive( rideGirl && isAlive );

            //  搭乗
            if( Input.GetKeyDown( KeyCode.V )
            &&  isAlive ){
                        if( aroundGirl && !rideGirl )   CmdRideOnGirl( true );
                else    if( rideGirl )                  CmdRideOnGirl( false );
            }
        }
	}

    //  開始処理
    void    StartProc()
    {
        //  プレイヤーカメラを使用するのでメインカメラを非アクティブ化
        GameObject  rMainCamera =   GameObject.Find( "Main Camera" );
        rMainCamera.GetComponent< Camera >().enabled            =   false;
        rMainCamera.GetComponent< AudioListener >().enabled     =   false;
        
        //  ＵＩ初期化
        {
            Transform   rCanvasTrans    =   GameObject.Find( "Canvas" ).transform;
            rCanvasTrans.FindChild( "TPS_HUD" ).gameObject.SetActive( true );
            rCanvasTrans.FindChild( "DyingMessage" ).gameObject.SetActive( true );
            rCanvasTrans.FindChild( "RevivalGage" ).gameObject.SetActive( true );

            UIRadar.SetPlayer( gameObject );
            UICompasCore.SetPlayer( gameObject ); 

            m_rRideButton   =   rCanvasTrans.FindChild( "TPS_HUD" ).FindChild( "RideButton" ).gameObject;
            m_rGetOffButton =   rCanvasTrans.FindChild( "TPS_HUD" ).FindChild( "GetOffButton" ).gameObject;
        }

        //  カメラのスクリプト生成  
        c_rMyCamera.transform.parent.gameObject.AddComponent<TPS_CameraController>();
        //  リスナーを有効化
        c_rMyCamera.GetComponent< AudioListener >().enabled     =   true;

        //  BGM再生
        //BGMManager.ChangeBGM( "BGM0", 0.5f, 1.0f );
    }
    //  終了処理
    public  void    EndProc()
    {
        //  カーソルを戻す
        Cursor.lockState    =   CursorLockMode.None;
        Cursor.visible      =   true;

        //  ＵＩ無効化
        Transform   rCanvasTrans    =   GameObject.Find( "Canvas" ).transform;
        rCanvasTrans.FindChild( "TPS_HUD" ).gameObject.SetActive( false );
        rCanvasTrans.FindChild( "DyingMessage" ).gameObject.SetActive( false );
        rCanvasTrans.FindChild( "RevivalGage" ).gameObject.SetActive( false );

        //  メインカメラを復旧
        GameObject  rMainCamera =   GameObject.Find( "Main Camera" );
        rMainCamera.GetComponent< Camera >().enabled            =   true;
        rMainCamera.GetComponent< AudioListener >().enabled     =   true;

        //  BGM停止
        //BGMManager.StopBGM();
    }

    //  弾薬を充填する
    public  void    SupplyAmmo()
    {
        if( c_ShotController == null )  return;

        for( int i = 0; i < c_ShotController.Length; i++ ){
            c_ShotController[ i ].SupplyAmmo();
        }
    }
    //  弾薬が満タンかどうか
    public  bool    CheckWhetherIsFullAmmo()
    {
        if( c_ShotController == null )                  return  true;

        for( int i = 0; i < c_ShotController.Length; i++ ){
            if( !c_ShotController[ i ].IsFullAmmo() )   return  false;
        }

        return  true;
    }

    //  死亡通知
    public  void    Death_InLocal( bool _IsFriend )
    {
        //  終了処理
        EndProc();

        //  サーバーに折り返しコマンドを送る
        CmdChangeToCommander( _IsFriend );
    }
//=================================================================================
//      雑多
//=================================================================================
    //  女の子が搭乗しているかどうかを調べる
    public  bool    CheckWhetherIsBoardingGirl()
    {
        GameObject[]    playerList  =   GameObject.FindGameObjectsWithTag( "Player" );
        for( int i = 0; i < playerList.Length; i++ ){
            GirlController  rGirl   =   playerList[ i ].GetComponent< GirlController >();
            if( !rGirl )            continue;
            if( !rGirl.IsRide() )   continue;

            return  true;
        }

        return  false;
    }
    //  周囲に乗せられる女の子が居るかどうか
    GameObject  FindAroundGirl( float _CheckDistance )
    {
        float           nearDistanceSq  =   _CheckDistance;
        GameObject[]    playerList      =   GameObject.FindGameObjectsWithTag( "Player" );
        for (int i = 0; i < playerList.Length; i++)
        {               
            if (this.gameObject == playerList[i])                           continue;
            if (playerList[i].GetComponent< RTSPlayer_Control >() == null)  continue;
        
            float distanceSq = ( transform.position - playerList[i].transform.position ).sqrMagnitude;
            if ( distanceSq > nearDistanceSq )    continue; 
        
            return  playerList[ i ];
        }

        return  null;
    }
//=================================================================================
//      通信
//=================================================================================
    //  女の子を乗せる
    [ Command ]
    public  void    CmdRideOnGirl( bool _IsEnable )
    {
        GameObject[]    players =   GameObject.FindGameObjectsWithTag( "Player" );
        for( int i = 0; i < players.Length; i++ ){
            if( players[ i ] == gameObject )    continue;

            GirlController  rGirl   =   players[ i ].GetComponent< GirlController >();
            if( !rGirl )                        continue;

            rGirl.RpcRidingVehicle( rGirl.netId, netId, _IsEnable );
            return;
        }
    }
    //  ダメージ送信
    [ Command ]
    public  void    CmdSendDamage( float _Damage )
    {
        //  ダメージを受ける
        if( _Damage > 0.0f )    m_rHP.SetDamage( _Damage );
        //  回復する
        else                    m_rHP.SetRecovery( -_Damage );
    }
    //  コマンダーに戻る
    [ ClientRpc ]
    public  void    RpcChangeToCommander( int _ClientID, bool _IsFriend )
    {
        //  指定されたプレイヤーのみ処理を行う
        if( m_rLinkManager.m_LocalPlayerID != _ClientID )   return;

        //  終了処理
        EndProc();

        //  サーバーに折り返しコマンドを送る
        CmdChangeToCommander( _IsFriend );
    }
    [ Command ]
    public  void    CmdChangeToCommander( bool _IsFriend )
    {
        //  侵入者が死亡
        if( !_IsFriend ){
            GameManager rGameManager    =   FunctionManager.GetAccessComponent< GameManager >( "GameManager" );
            if( rGameManager ){
                rGameManager.RpcRecordNotice_ToOther( "侵入者が撃退されました！", connectionToClient.connectionId );
            }
        }
        //  ゲームオーバー
        if( _IsFriend ){
            GameManager rGameManager    =   FunctionManager.GetAccessComponent< GameManager >( "GameManager" );
            if( rGameManager ){
                rGameManager.GameOver();
            }
        }

        //  新しいプレイヤーオブジェクト生成
        GameObject  newPlayer   =   Instantiate( c_Commander );

        //  変身
        NetworkServer.ReplacePlayerForConnection( connectionToClient, newPlayer, 0 );

        //  オブジェクト破棄
        Destroy( gameObject );
    }
}
