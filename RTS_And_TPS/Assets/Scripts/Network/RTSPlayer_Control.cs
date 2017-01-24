
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class RTSPlayer_Control : NetworkBehaviour {

    public  GameObject          c_Commander     =   null;

    private LinkManager         m_rLinkManager  =   null;
    private ResourceCreator     m_rRCreator     =   null;

    private TPSPlayer_HP        m_rHP           =   null;

	// Use this for initialization
	void    Start()
    {
        //  アクセスの取得
        m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );
        m_rRCreator     =   FunctionManager.GetAccessComponent< ResourceCreator >( "ResourceCreator" );
        m_rHP           =   GetComponent< TPSPlayer_HP >();

        //  自分のキャラクターのみ処理を行う
	    if( isLocalPlayer ){
            //  開始処理
            StartProc();
        }

        //  出撃ボイス
        SoundController.PlayNow( "Voice_G_Launch", transform, transform.position, 0.0f, 1.0f, 1.0f, 10.0f );
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
	}

    //  開始処理
    void    StartProc()
    {
        //  ＵＩ初期化
        {
            Transform   rCanvasTrans    =   GameObject.Find( "Canvas" ).transform;
		    Transform   rRTSHUD         =   rCanvasTrans.FindChild( "RTS_HUD" );

            rRTSHUD.gameObject.SetActive( true );

            //rCanvasTrans.FindChild( "TPS_HUD" ).gameObject.SetActive( true );
            rCanvasTrans.FindChild( "DyingMessage" ).gameObject.SetActive( true );
            rCanvasTrans.FindChild( "RevivalGage" ).gameObject.SetActive( true );

            UIRadar.SetPlayer( gameObject );
        }

        //  リソース情報有効化
        ResourceInformation rResourceInfo   =   GameObject.Find( "ResourceInformation" ).GetComponent< ResourceInformation >();
        rResourceInfo.enabled   =   true;
        rResourceInfo.m_gridSplitSpacePlaneTargetTransform
                                =   transform;

        //  ガイドが既に生成されている場合は有効にする
        GameObject  rSplitGuide =   GameObject.Find( "SplitGuidePlane(Clone)" );
        if( rSplitGuide ){
            rSplitGuide.GetComponent< MeshRenderer >().enabled  =   true;
        }
        
        //  プレイヤーカメラを使用するのでメインカメラを非アクティブ化
        GameObject  rMainCamera =   GameObject.Find( "Main Camera" );
        rMainCamera.GetComponent< Camera >().enabled            =   false;
        rMainCamera.GetComponent< AudioListener >().enabled     =   false;
        
        //  ＲＴＳカメラ有効化
        GameObject  rRTSCamera      =   GameObject.Find( "RTSCamera" );
        GameObject  rRTSCameraAdd   =   GameObject.Find( "RTSCamera_Add" );
        Transform   rRTSCParent     =   rRTSCamera.transform.parent;
        rRTSCamera.GetComponent< Camera >().enabled         =   true;
        rRTSCamera.GetComponent< AudioListener >().enabled  =   true;
        rRTSCParent.GetComponent< RTSCamera >().enabled     =   true;
        rRTSCParent.GetComponent< RTSCamera >().m_target    =   transform;
        rRTSCParent.GetComponent< RTSCamera >().ForcusOnPlayer();
        if( rRTSCameraAdd ){
            rRTSCameraAdd.GetComponent< Camera >().enabled  =   true;
        }
    }
    //  終了処理
    public  void    EndProc()
    {
        //  ＵＩ無効化
        Transform   rCanvasTrans    =   GameObject.Find( "Canvas" ).transform;
        Transform   rRTSHUD         =   rCanvasTrans.FindChild( "RTS_HUD" );

        rRTSHUD.gameObject.SetActive( false );

        //rCanvasTrans.FindChild( "TPS_HUD" ).gameObject.SetActive( false );
        rCanvasTrans.FindChild( "DyingMessage" ).gameObject.SetActive( false );
        rCanvasTrans.FindChild( "RevivalGage" ).gameObject.SetActive( false );

        UIRadar.SetPlayer( gameObject );

        //  メインカメラを復旧
        GameObject  rMainCamera     =   GameObject.Find( "Main Camera" );
        rMainCamera.GetComponent< Camera >().enabled            =   true;
        rMainCamera.GetComponent< AudioListener >().enabled     =   true;

        //  ＲＴＳカメラ無効化
        GameObject  rRTSCamera      =   GameObject.Find( "RTSCamera" );
        GameObject  rRTSCameraAdd   =   GameObject.Find( "RTSCamera_Add" );
        Transform   rRTSCParent     =   rRTSCamera.transform.parent;
        rRTSCamera.GetComponent< Camera >().enabled         =   false;
        rRTSCamera.GetComponent< AudioListener >().enabled  =   false;
        rRTSCParent.GetComponent< RTSCamera >().enabled     =   false;
        if( rRTSCameraAdd ){
            rRTSCameraAdd.GetComponent< Camera >().enabled  =   false;
        }

        //  リソース情報無効化
        GameObject.Find( "ResourceInformation" )
            .GetComponent< ResourceInformation >().enabled  =   false;

        //  ガイドを非表示
        GameObject  rSplitGuide =   GameObject.Find( "SplitGuidePlane(Clone)" );
        if( rSplitGuide ){
            rSplitGuide.GetComponent< MeshRenderer >().enabled  =   false;
        }
        //  ボタンを非表示
        GetComponent< GirlController >().SetActiveButton( false );
    }

    //  死亡通知
    public  void    Death_InLocal( bool _IsFriend )
    {
        //  終了処理
        EndProc();

        //  サーバーに折り返しコマンドを送る
        CmdChangeToCommander( _IsFriend );
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

//================================================================================================
//      外部からの操作
//================================================================================================
    [ Command ]
	public  void    CmdAddResource( int resourceID, Vector3 _Position, Quaternion _Rotation )
	{
        m_rRCreator.AddResource_CallByCommand( resourceID, _Position, _Rotation, connectionToClient.connectionId );
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
}
