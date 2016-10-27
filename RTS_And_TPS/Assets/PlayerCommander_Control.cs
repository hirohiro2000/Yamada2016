
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class PlayerCommander_Control : NetworkBehaviour {

    public  GameObject  c_TPSPlayer =   null;
    public  GameObject  c_RTSPlayer =   null;

	// Use this for initialization
	void    Start()
    {
        if( isLocalPlayer ){
            //  カーソルを戻す
            Cursor.lockState    =   CursorLockMode.None;
            Cursor.visible      =   true;
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

        //  フレーム
        GUI.Box( new Rect( 43, 212, 130, 123 ), "Select Type" );

        //  プレイヤータイプ選択
        if( GUI.Button( new Rect( 58, 240, 100, 20 ), "ロボット" ) ){
            Camera.main.GetComponent< Camera >().enabled    =   false;
            //  ロボットに変身
            CmdLunchTPSPlayer();
        }
        if( GUI.Button( new Rect( 58, 270, 100, 20 ), "女の子" ) ){
            //  女の子に変身
            CmdLunchRTSPlayer();
        }
    }

    //  ＴＰＳプレイヤーで出撃
    [ Command ]
    public  void    CmdLunchTPSPlayer()
    {
        //  新しいプレイヤーオブジェクト生成
        GameObject  newPlayer   =   Instantiate( c_TPSPlayer );

        //  配置設定
        newPlayer.transform.position    =   new Vector3( 0.0f, 0.0f, 15.0f );
        newPlayer.transform.rotation    =   Quaternion.Euler( 0.0f, 180.0f, 0.0f );

        //  変身
        NetworkServer.ReplacePlayerForConnection( connectionToClient, newPlayer, 0 );

        //  オブジェクト破棄
        Destroy( gameObject );
    }
    //  ＲＴＳプレイヤーで出撃
    [ Command ]
    public  void    CmdLunchRTSPlayer()
    {
        //  新しいプレイヤーオブジェクト生成
        GameObject  newPlayer   =   Instantiate( c_RTSPlayer );

        //  配置設定
        newPlayer.transform.position    =   new Vector3( 0.0f, 0.0f, 15.0f );
        newPlayer.transform.rotation    =   Quaternion.Euler( 0.0f, 180.0f, 0.0f );

        //  変身
        NetworkServer.ReplacePlayerForConnection( connectionToClient, newPlayer, 0 );

        //  オブジェクト破棄
        Destroy( gameObject );
    }
}
