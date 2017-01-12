
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections;

public class MyNetworkManager : NetworkManager {

	// Use this for initialization 
	void    Start()
    {
	    
	}
	
	// Update is called once per frame
	void    Update()
    {
	    
	}

    public  override    void    OnServerSceneChanged( string _SceneName )
    {
        Debug.Log( "Call_OnServerSceeneChanged" );

        for( int i = 0; i < NetworkServer.connections.Count; i++ ){
            NetworkConnection   rConn   =   NetworkServer.connections[ i ];
            for( int p = 0; p < rConn.playerControllers.Count; p++ ){
                PlayerController    rPCtrl  =   rConn.playerControllers[ p ];

                //  プレイヤーオブジェクトが存在する場合は削除
                if( rPCtrl.gameObject ){
                    Destroy( rPCtrl.gameObject );
                    rPCtrl.gameObject   =   null;

                    Debug.Log( "Destroy_PCtrl_Gameobject_OK" );
                }
            }
        }

        for( int i = 0; i < NetworkServer.connections.Count; i++ ){
            NetworkConnection   rConn   =   NetworkServer.connections[ i ];
            for( short p = 0; p < rConn.playerControllers.Count; p++ ){
                //  新しくプレイヤーオブジェクトを生成
                GameObject  rObj        =   Instantiate( playerPrefab );

                //  新しいゲームプレイヤーを追加 
                if( NetworkServer.AddPlayerForConnection( rConn, rObj, p ) ){
                    Debug.Log( "AddPlayerForConnection_OK" );
                }
                else{
                    Debug.Log( "AddPlayerForConnection_NG" );
                }
            }
        }
    }
    public  override    void    OnClientSceneChanged( NetworkConnection conn )
    {
        if( !ClientScene.ready ){
            ClientScene.Ready( conn );
        }

        bool addPlayer = (ClientScene.localPlayers.Count == 0);
        bool foundPlayer = false;
        foreach (var playerController in ClientScene.localPlayers)
        {
            if (playerController.gameObject != null)
            {
                foundPlayer = true;
                break;
            }
        }
        if (!foundPlayer)
        {
            // there are players, but their game objects have all been deleted
            addPlayer = true;
        }
        if (addPlayer)
        {
            ClientScene.AddPlayer(0);
        }
    }
}
