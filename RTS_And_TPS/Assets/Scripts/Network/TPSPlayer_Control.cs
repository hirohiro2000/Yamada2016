
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class TPSPlayer_Control : NetworkBehaviour {

    public  Camera          c_rMyCamera     =   null;
    public  GameObject      c_Commander     =   null;

    private LinkManager     m_rLinkManager  =   null;

    private bool            m_IsLock        =   true;

    private SoundController m_bgm           = null;

	// Use this for initialization
	void    Start()
    {
        m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );
        
        //  自分のキャラクターのみ処理を行う
        if( isLocalPlayer ){
            //  開始処理
            StartProc();
        }
        //  自分のキャラクター以外はカメラを使用しない
        else{
            c_rMyCamera.enabled =   false;
        }
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
            CmdChangeToCommander();
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
	}

    //  開始処理
    void    StartProc()
    {
        //  プレイヤーカメラを使用するのでメインカメラを非アクティブ化
        GameObject  rMainCamera =   GameObject.Find( "Main Camera" );
        rMainCamera.GetComponent< Camera >().enabled            =   false;
        rMainCamera.GetComponent< AudioListener >().enabled     =   false;
        
        // UI系初期化
        GameObject.Find("TPS_HUD").SetActive(true);
        UIRadar.SetPlayer(gameObject);

        // カメラのスクリプト生成
        TPS_CameraController cam = c_rMyCamera.gameObject.AddComponent<TPS_CameraController>();

        // BGM再生
        if (m_bgm == null)
        {
            m_bgm = SoundController.Create("BGM0", null);
        }
        m_bgm.Play();

    }
    //  終了処理
    void    EndProc()
    {
        //  メインカメラを復旧
        GameObject  rMainCamera =   GameObject.Find( "Main Camera" );
        rMainCamera.GetComponent< Camera >().enabled            =   true;
        rMainCamera.GetComponent< AudioListener >().enabled     =   true;

        // BGM停止
        if (m_bgm != null)
        {
            m_bgm.Stop();
        }
    }

    //  コマンダーに戻る
    [ ClientRpc ]
    public  void    RpcChangeToCommander( int _ClientID )
    {
        //  指定されたプレイヤーのみ処理を行う
        if( m_rLinkManager.m_LocalPlayerID != _ClientID )   return;

        //  終了処理
        EndProc();

        //  サーバーに折り返しコマンドを送る
        CmdChangeToCommander();
    }
    [ Command ]
    public  void    CmdChangeToCommander()
    {
        //  新しいプレイヤーオブジェクト生成
        GameObject  newPlayer   =   Instantiate( c_Commander );

        //  変身
        NetworkServer.ReplacePlayerForConnection( connectionToClient, newPlayer, 0 );

        //  オブジェクト破棄
        Destroy( gameObject );
    }
}
