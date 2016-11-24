
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class TPSPlayer_Control : NetworkBehaviour {

    public  Camera              c_rMyCamera         =   null;
    public  GameObject          c_Commander         =   null;

    public  TPSShotController[] c_ShotController    =   null;

    private LinkManager         m_rLinkManager      =   null;

    private bool                m_IsLock            =   true;

    private SoundController     m_bgm               =   null;
    private TPSPlayer_HP        m_rHP               =   null;

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
        
        //  UI系初期化
        GameObject  rCanvas         =   GameObject.Find( "Canvas" );
        if( rCanvas ){
            Transform   rTPSHUD     =   rCanvas.transform.FindChild( "TPS_HUD" );
            if( rTPSHUD )   rTPSHUD.gameObject.SetActive( true );
        }
        UIRadar.SetPlayer( gameObject );

        //  カメラのスクリプト生成 
        TPS_CameraController cam = c_rMyCamera.transform.parent.gameObject.AddComponent<TPS_CameraController>();
        //  リスナーを有効化
        c_rMyCamera.GetComponent< AudioListener >().enabled     =   true;

        // BGM再生
        if (m_bgm == null)
        {
            Transform   rPTTrans    =   FunctionManager.GetAccessComponent< Transform >( "PlayTest_Shell" );
            m_bgm = SoundController.Create("BGM0", rPTTrans);
        }
        m_bgm.Play();

    }
    //  終了処理
    public  void    EndProc()
    {
        //  カーソルを戻す
        Cursor.lockState    =   CursorLockMode.None;
        Cursor.visible      =   true;

        //  ＵＩ無効化
        GameObject.Find( "Canvas" ).transform
            .FindChild( "TPS_HUD" ).gameObject.SetActive( false );

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

//=================================================================================
//      通信
//=================================================================================
    //  ダメージ送信
    [ Command ]
    public  void    CmdSendDamage( float _Damage )
    {
        //  ダメージを受ける
        m_rHP.SetDamage( _Damage );
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
        //  ゲームオーバー
        GameManager rGameManager    =   FunctionManager.GetAccessComponent< GameManager >( "GameManager" );
        if( rGameManager ){
            rGameManager.GameOver();
        }

        //  新しいプレイヤーオブジェクト生成
        GameObject  newPlayer   =   Instantiate( c_Commander );

        //  変身
        NetworkServer.ReplacePlayerForConnection( connectionToClient, newPlayer, 0 );

        //  オブジェクト破棄
        Destroy( gameObject );
    }
}
