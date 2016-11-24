
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class PlayerCommander_Control : NetworkBehaviour {

    public  GameObject  c_TPSPlayer     =   null;
    public  GameObject  c_RTSPlayer     =   null;

    private GameManager m_rGameManager  =   null;

	// Use this for initialization
	void    Start()
    {
        m_rGameManager      =   FunctionManager.GetAccessComponent< GameManager >( "GameManager" );

        if( isLocalPlayer ){
            //  カーソルを戻す
            Cursor.lockState    =   CursorLockMode.None;
            Cursor.visible      =   true;

            // BGM再生
            SoundController bgm = SoundController.Create("BGM1",this.transform);
            bgm.Play();
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

        //  フレーム
        GUI.Box( new Rect( 43, 212, 130, 123 ), "Select Type" );

        //  プレイヤータイプ選択
        if( GUI.Button( new Rect( 58, 240, 100, 20 ), "ロボット" ) ){
            //  ロボットに変身
            if( m_rGameManager.GetState() <= GameManager.State.InGame ) CmdLunchTPSPlayer();
        }
        if( GUI.Button( new Rect( 58, 270, 100, 20 ), "女の子" ) ){
            //  女の子に変身
            if( m_rGameManager.GetState() <= GameManager.State.InGame ) CmdLunchRTSPlayer();
        }
    }

    //  ＴＰＳプレイヤーで出撃
    [ Command ]
    public  void    CmdLunchTPSPlayer()
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
    public  void    CmdLunchRTSPlayer()
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
}
