
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class LinkManager : NetworkBehaviour {

    public  int                 m_LocalPlayerID     =   0;
    public  GameObject          m_rLocalPlayer      =   null;
    public  NetPlayer_Control   m_rLocalNPControl   =   null;

    private NetworkManager      m_rNetManager       =   null;

	// Use this for initialization
	void    Start()
    {
	    //  アクセスの取得
        if( !m_rNetManager )    m_rNetManager   =   FunctionManager.GetAccessComponent< NetworkManager >( "NetworkManager" );
	}
	
	// Update is called once per frame
	void    Update()
    {
	    //  クライアントでは更新処理を行わない
        if( !NetworkServer.active ) return;

	    //  アクセス更新
        if( !m_rNetManager )        m_rNetManager   =   FunctionManager.GetAccessComponent< NetworkManager >( "NetworkManager" );
        if( !m_rNetManager )        return;


	}

    //  指定されたＩＤのクライアントがアクティブかどうか調べる
    public  bool        CheckActiveClient( int _ClientID )
    {
        GameObject[]    objList =   GameObject.FindGameObjectsWithTag( "Player" );
        for( int i = 0; i < objList.Length; i++ ){
            NetPlayer_Control   rControl    =   objList[ i ].GetComponent< NetPlayer_Control >();
            if( !rControl )                         continue;
            if( rControl.c_ClientID == _ClientID )  return  true;
        }

        return  false;
    }
    //  自分のプレイヤーオブジェクトへのアクセスを取得
    public  GameObject  FindPlayerObject( int _ClientID )
    {
        GameObject[]    playerList  =   GameObject.FindGameObjectsWithTag( "Player" );
        for( int i = 0; i < playerList.Length; i++ ){
            NetPlayer_Control   rContrl =   playerList[ i ].GetComponent< NetPlayer_Control >();
            if( !rContrl )                          continue;
            if( rContrl.c_ClientID == _ClientID )   return  playerList[ i ];
        }

        return  null;
    }
    public  int         CheckNumPlayer()
    {
        return  m_rNetManager.numPlayers;
    }
}
