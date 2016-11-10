
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class RTSPlayer_Control : NetworkBehaviour {

    public  GameObject          c_Commander     =   null;

    private LinkManager         m_rLinkManager  =   null;
    private ResourceCreator     m_rRCreator     =   null;

	// Use this for initialization
	void    Start()
    {
        //  アクセスの取得
        m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );
        m_rRCreator     =   FunctionManager.GetAccessComponent< ResourceCreator >( "ResourceCreator" );

        //  自分のキャラクターのみ処理を行う
	    if( isLocalPlayer ){
            //  開始処理
            StartProc();
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
	}

    //  開始処理
    void    StartProc()
    {
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
        GameObject  rRTSCamera  =   GameObject.Find( "RTSCamera" );
        rRTSCamera.GetComponent< Camera >().enabled         =   true;
        rRTSCamera.GetComponent< AudioListener >().enabled  =   true;
        rRTSCamera.GetComponent< RTSCamera >().enabled      =   true;
        rRTSCamera.GetComponent< RTSCamera >().m_target     =   transform;
    }
    //  終了処理
    void    EndProc()
    {
        //  メインカメラを復旧
        GameObject  rMainCamera =   GameObject.Find( "Main Camera" );
        rMainCamera.GetComponent< Camera >().enabled            =   true;
        rMainCamera.GetComponent< AudioListener >().enabled     =   true;

        //  ＲＴＳカメラ無効化
        GameObject  rRTSCamera  =   GameObject.Find( "RTSCamera" );
        rRTSCamera.GetComponent< Camera >().enabled         =   false;
        rRTSCamera.GetComponent< AudioListener >().enabled  =   false;
        rRTSCamera.GetComponent< RTSCamera >().enabled      =   false;

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

//================================================================================================
//      外部からの操作
//================================================================================================
    [ Command ]
	public  void    CmdAddResource( int resourceID, Vector3 _Position, Quaternion _Rotation )
	{
        m_rRCreator.AddResource_CallByCommand( resourceID, _Position, _Rotation );
    }
}
