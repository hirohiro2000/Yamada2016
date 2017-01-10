
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class PlayerCommander_Control : NetworkBehaviour {

    public  GameObject  c_TPSPlayer     =   null;
    public  GameObject  c_RTSPlayer     =   null;
    public  GameObject  c_TPSPlayerE    =   null;

    private GameManager m_rGameManager  =   null;
    private LinkManager m_rLinkManager  =   null;

    private string      m_EditName      =   "Null";

	// Use this for initialization
	void    Start()
    {
        m_rGameManager  =   FunctionManager.GetAccessComponent< GameManager >( "GameManager" );
        m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );

        if( isLocalPlayer ){
            //  カーソルを戻す
            Cursor.lockState    =   CursorLockMode.None;
            Cursor.visible      =   true;

            //  プレイヤー名取得
            m_EditName          =   PlayerPrefs.GetString( "PlayerName", "Null" );

            //  BGM再生
            //BGMManager.ChangeBGM( "BGM1", 0.5f, 0.0f, 0.0f );
            //SoundController bgm = SoundController.Create("BGM1",this.transform);
            //bgm.Play();
        }
    }
	
	// Update is called once per frame
	void    Update()
    {
	    
	}
    void    OnGUI()
    {
        //  クローンは処理を行わない
        if( !isLocalPlayer )    return;
        //  アクセスを取得
        if( !m_rGameManager )   m_rGameManager      =   FunctionManager.GetAccessComponent< GameManager >( "GameManager" );
        if( !m_rGameManager )   return;

        //  プレイヤー名入力
        {
            //  グループ開始
            GUI.BeginGroup( new Rect( 43, 158, 130, 58 ) );
            GUI.Box( new Rect( 0, 0, 130, 58 ), "Your Name" );

            //  入力欄
            m_EditName  =   GUI.TextField( new Rect( 10, 28, 110, 20 ), m_EditName );

            //  グループ終了
            GUI.EndGroup();
        }

        //  プレイヤータイプ選択
        GUI.Box( new Rect( 43, 232, 130, 123 ), "Select Type" );
        {
            //  ロボットで出撃
            if( GUI.Button( new Rect( 58, 260, 100, 20 ), "ロボット" ) ){
                if( m_rGameManager.GetState() <= GameManager.State.InGame ){
                    DetermPlayerName( m_EditName );
                    CmdLaunchTPSPlayer();
                }
            }
            //  女の子で出撃
            if( GUI.Button( new Rect( 58, 290, 100, 20 ), "女の子" ) ){
                if( m_rGameManager.GetState() <= GameManager.State.InGame ){
                    DetermPlayerName( m_EditName );
                    CmdLaunchRTSPlayer();
                }
            }
            //  敵ロボットで出撃
            if( GUI.Button( new Rect( 58, 320, 100, 20 ), "侵入" ) ){
                if( m_rGameManager.GetState() <= GameManager.State.InGame ){
                    DetermPlayerName( m_EditName );
                    CmdLaunchTPSPlayerE();
                }
            }
        }
    }

    //  プレイヤー名決定
    void    DetermPlayerName( string _PlayerName )
    {
        //  プレイヤー名保存
        PlayerPrefs.SetString( "PlayerName", _PlayerName );

        //  プレイヤー名送信
        CmdSendPlayerName( _PlayerName );
    }
    //  プレイヤー名送信
    [ Command ]
    public  void    CmdSendPlayerName( string _PlayerName )
    {
        m_rGameManager.SetToList_PlayerName( connectionToClient.connectionId, _PlayerName );
    }

    //  ＴＰＳプレイヤーで出撃
    [ Command ]
    public  void    CmdLaunchTPSPlayer()
    {
        //  カウントダウン開始
        if( m_rGameManager ){
            m_rGameManager.StartCountDown();
        }

        //  新しいプレイヤーオブジェクト生成
        GameObject  newPlayer   =   Instantiate( c_TPSPlayer );

        //  配置設定
        newPlayer.transform.position    =   new Vector3( 0.0f, 0.0f, 15.0f );
        newPlayer.transform.rotation    =   Quaternion.Euler( 0.0f, 180.0f, 0.0f );

        GameObject  rSpawnPoint         =   GameObject.Find( "Player_SpawnPoint" );
        if( rSpawnPoint ){
            newPlayer.transform.position    =   rSpawnPoint.transform.position;
            newPlayer.transform.eulerAngles =   rSpawnPoint.transform.eulerAngles;
        }

        //  変身
        NetworkServer.ReplacePlayerForConnection( connectionToClient, newPlayer, 0 );

        //  オブジェクト破棄
        Destroy( gameObject );
    }
    //  ＲＴＳプレイヤーで出撃 
    [ Command ]
    public  void    CmdLaunchRTSPlayer()
    {
        //  カウントダウン開始
        if( m_rGameManager ){
            m_rGameManager.StartCountDown();
        }

        //  新しいプレイヤーオブジェクト生成 
        GameObject  newPlayer   =   Instantiate( c_RTSPlayer );

        //  配置設定
        newPlayer.transform.position    =   new Vector3( 0.0f, 0.0f, 15.0f );
        newPlayer.transform.rotation    =   Quaternion.Euler( 0.0f, 180.0f, 0.0f );

        GameObject  rSpawnPoint         =   GameObject.Find( "Player_SpawnPoint" );
        if( rSpawnPoint ){
            newPlayer.transform.position    =   rSpawnPoint.transform.position;
            newPlayer.transform.eulerAngles =   rSpawnPoint.transform.eulerAngles;
        }

        //  変身
        NetworkServer.ReplacePlayerForConnection( connectionToClient, newPlayer, 0 );

        //  オブジェクト破棄
        Destroy( gameObject );
    }
    //  敵ＴＰＳプレイヤーで出撃
    [ Command ]
    public  void    CmdLaunchTPSPlayerE()
    {
        //  カウントダウン開始
        //if( m_rGameManager ){
        //    m_rGameManager.StartCountDown();
        //}

        //  侵入メッセージを送信
        if( m_rGameManager ){
            m_rGameManager.RpcRecordNoticeE_ToOther( "侵入者が現れました！", connectionToClient.connectionId );
        }

        //  新しいプレイヤーオブジェクト生成
        GameObject  newPlayer   =   Instantiate( c_TPSPlayerE );

        //  配置設定
        newPlayer.transform.position    =   new Vector3( 0.0f, 0.0f, 15.0f );
        newPlayer.transform.rotation    =   Quaternion.Euler( 0.0f, 180.0f, 0.0f );

        GameObject  rSpawnPoint         =   GameObject.Find( "Player_SpawnPointE" );
        if( rSpawnPoint ){
            newPlayer.transform.position    =   rSpawnPoint.transform.position;
            newPlayer.transform.eulerAngles =   rSpawnPoint.transform.eulerAngles;
        }

        //  変身
        NetworkServer.ReplacePlayerForConnection( connectionToClient, newPlayer, 0 );

        //  オブジェクト破棄
        Destroy( gameObject );
    }
}
