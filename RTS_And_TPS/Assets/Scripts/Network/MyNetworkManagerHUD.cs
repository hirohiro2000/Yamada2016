
#if ENABLE_UNET

using   System;
using   System.Net;
using   System.Net.Sockets;
using   System.Collections.Generic;
using   UnityEngine.SceneManagement;

namespace   UnityEngine.Networking
{
	[ AddComponentMenu( "Network/NetworkManagerHUD" ) ]
	[ System.ComponentModel.EditorBrowsable( System.ComponentModel.EditorBrowsableState.Never ) ]
	public  class   MyNetworkManagerHUD   :   MonoBehaviour
	{
		public  NetworkManager      m_rNetworkManager   =   null;
        public  uint                m_MatchSize         =   4;

        public  bool                showGUI             =   true;
        public  Font                c_UseFont           =   null;

        private int                 offsetX             =   0;
        private int                 offsetY             =   0;

        private string              m_RoomPass          =   "";
        private string              m_JoinPass          =   "";
        private string              m_NameFilter        =   "";

        private MyNetworkDiscovery  m_rDiscovery        =   null;

        private string              m_HostName          =   "";
        private string              m_MyIP              =   "";


		// Runtime variable
		//private bool                showServer          =   false;

		void    Awake()
		{
			m_rNetworkManager   =   GetComponent< NetworkManager >();
            m_rDiscovery        =   GetComponent< MyNetworkDiscovery >();
		}
        void    Start()
        {
            m_rNetworkManager.matchName =   "";

            //  ホスト名を取得する
            m_HostName  =   Dns.GetHostName();
            //  IPアドレスを保存
            m_MyIP      =   Network.player.ipAddress;
        }

		void    Update()
		{
			if( !showGUI )	return;

            if( !NetworkClient.active
            &&  !NetworkServer.active
            &&  m_rNetworkManager.matchMaker == null )
            {
                //if( Input.GetKeyDown( KeyCode.S ) ) m_rNetworkManager.StartServer();
                if( Input.GetKeyDown( KeyCode.H ) ){
                    //  ブロードキャスト開始 
                    m_rDiscovery.Initialize();
                    m_rDiscovery.StartAsServer();

                    //  ホスト開始
                    m_rNetworkManager.StartHost();
                }
                if( Input.GetKeyDown( KeyCode.C ) ){
                    //  ブロードキャスト開始
                    m_rDiscovery.Initialize();
                    m_rDiscovery.StartAsClient();
                }
            }
            //if( NetworkServer.active
            //&&  NetworkClient.active )
            //{
            //    if( Input.GetKeyDown( KeyCode.X ) ) m_rNetworkManager.StopHost();
            //}
		}

		void    OnGUI()
		{
			if( !showGUI )  return;

			int xpos = 10 + offsetX;
			int ypos = 40 + offsetY;
			int spacing = 24;

			if( !NetworkClient.active
            &&  !NetworkServer.active
            &&  m_rNetworkManager.matchMaker == null
            &&  !m_rDiscovery.running
            &&  ( !NetworkTransport.IsStarted || !NetworkTransport.IsBroadcastDiscoveryRunning() ) )
			{
				if( GUI.Button( new Rect( xpos, ypos, 200, 20 ), "部屋を立てる" ) ){
                    //  ブロードキャスト開始  　
                    m_rDiscovery.Initialize();
                    if( m_rDiscovery.StartAsServer() ){
                        //  ホスト開始 
                        m_rNetworkManager.StartHost();
                    }
                }
				ypos    +=  spacing;

				if( GUI.Button( new Rect( xpos, ypos, 200, 20 ), "部屋を探す" ) ){
                    //  ブロードキャスト開始　 
                   m_rDiscovery.Initialize();
                   m_rDiscovery.StartAsClient();
                }
                //ypos    +=  spacing;
                ypos    +=  ( int )( spacing * 1.5f );

                if( GUI.Button( new Rect( xpos, ypos, 200, 20 ), "オフラインで遊ぶ" ) ){
                    //  ホスト開始 
                    m_rNetworkManager.StartHost();
                }
                ypos    +=  spacing;
                
                if( GUI.Button( new Rect( xpos, ypos, 106, 20 ), "IPで参加" ) ){
                    //  クライアント開始     
                    m_rNetworkManager.StartClient();
                }
                m_rNetworkManager.networkAddress
                    =   GUI.TextField( new Rect( xpos + 110, ypos, 90, 20 ), m_rNetworkManager.networkAddress );
                ypos    +=  ( int )( spacing * 1.5f );
                
                //  ＩＰアドレス表示 
                {
                    float   myLeft  =   10.0f;
                    GUI.Box( new Rect( xpos + 210 + myLeft, 40 + offsetY + 8, 164, 22 ), "" );
                    GUI.Box( new Rect( xpos + 210 + myLeft, 40 + offsetY + 8, 164, 84 ), "あなたの情報" );
                    GUI.Label( new Rect( xpos + 210 + myLeft + 14, 40 + offsetY + 8 + 29, 300, 20 ), "ホスト名" );
                    GUI.Label( new Rect( xpos + 210 + myLeft+ 14, 40 + offsetY + 8 + 54, 300, 20 ), "IP");
                    FunctionManager.GUILabel( FunctionManager.AR_TYPE.TOP_LEFT, new Vector2( xpos + 210 + myLeft+ 164 - 14, -40 - offsetY - 8 - 29 ), m_HostName, new Vector2( 1.0f, 1.0f ) );
                    FunctionManager.GUILabel( FunctionManager.AR_TYPE.TOP_LEFT, new Vector2( xpos + 210 + myLeft+ 164 - 14, -40 - offsetY - 8 - 54 ), m_MyIP, new Vector2( 1.0f, 1.0f ) );

                }
                //if( GUI.Button( new Rect( xpos, ypos, 200, 20 ), "LAN Server Only(S)" ) ){
                //    //  ブロードキャスト開始 
                //    m_rDiscovery.Initialize();
                //    m_rDiscovery.StartAsServer();

                //    //  サーバー起動
                //    m_rNetworkManager.StartServer();
                //}
                //ypos    +=  spacing;
			}
			else
			{
				if( NetworkServer.active ){
					GUI.Label( new Rect( xpos, ypos, 300, 20 ), "Server: port=" + m_rNetworkManager.networkPort );
					ypos    +=  spacing;
				}
				if( NetworkClient.active ){
					GUI.Label( new Rect( xpos, ypos, 300, 20 ), "Client: address=" + m_rNetworkManager.networkAddress + " port=" + m_rNetworkManager.networkPort );
					ypos    +=  spacing;
				}
			}

            if( !NetworkServer.active
            &&  !NetworkClient.active
            &&  m_rDiscovery.running ){
                //  サーバーが見つかった
                if( m_rDiscovery.broadcastsReceived            != null
                &&  m_rDiscovery.broadcastsReceived.Keys.Count >  0    ){ 
                    foreach( var key in m_rDiscovery.broadcastsReceived.Keys ){
                        var value       =   m_rDiscovery.broadcastsReceived[ key ];
                        var dataString  =   MyNetworkDiscovery.BytesToString( value.broadcastData );
                        var items       =   dataString.Split( ':' );

                        if( items.Length == 3 && items[ 0 ] == "NetworkManager" ){
                            if( NetworkManager.singleton != null && NetworkManager.singleton.client == null ){
                                //  クライアント開始
                                NetworkManager.singleton.networkAddress =   items[ 1 ];
                                NetworkManager.singleton.networkPort    =   Convert.ToInt32( items[ 2 ] );
                                NetworkManager.singleton.StartClient();

                                //  ブロードキャスト終了
                                m_rDiscovery.StopBroadcast();
                            }
                        }

                        //  最初に見つかったアドレス以外は無視
                        break;
                    }
                }
                //  検索中
                else{
                    GUIContent  content =   new GUIContent( "___  Searching Host  ___" );
                    GUIStyle    style   =   new GUIStyle( GUI.skin.label );
                    Vector2     size    =   style.CalcSize( content );
                    Rect        rect    =   FunctionManager.AdjustRectCanvasToGUI(
                        FunctionManager.AR_TYPE.MIDDLE_CENTER,
                        new Rect( 0, 0, size.x, size.y ),
                        new Vector2( 0.5f, 0.5f ),
                        new Vector2( 200, 20 )
                    );
                    GUI.Label( new Rect( xpos + rect.x, ypos, 300, 20 ), content );
                    ypos += spacing; 
                }
            }

			if( NetworkClient.active
            &&  !ClientScene.ready )
			{ 
				if( GUI.Button( new Rect( xpos, ypos, 200, 20 ), "Client Ready" ) )
				{
					ClientScene.Ready( m_rNetworkManager.client.connection );
				
					if( ClientScene.localPlayers.Count == 0 ){
						ClientScene.AddPlayer( 0 );
					}
				}
				ypos += spacing;
			}

			if( NetworkServer.active
            ||  NetworkClient.active
            ||  m_rDiscovery.running )
			{
				if( GUI.Button( new Rect( xpos, ypos, 200, 20 ), "Stop" ) )
				{
					m_rNetworkManager.StopHost();
                    if( m_rDiscovery.running )          m_rDiscovery.StopBroadcast();
                    if( NetworkTransport.IsStarted )    NetworkTransport.Shutdown();

                    //  シーンをリロード
                    SceneManager.LoadScene( SceneManager.GetActiveScene().name );
				}
				ypos += spacing;
			}

			if( !NetworkServer.active
            &&  !NetworkClient.active
            &&  !m_rDiscovery.running )
			{
				ypos += 10;

				if( m_rNetworkManager.matchMaker == null )
				{
                    //if( GUI.Button( new Rect( xpos, ypos, 200, 20 ), "Enable Match Maker" ) )
                    //{
                    //    m_rNetworkManager.StartMatchMaker();
                    //}
                    //ypos += spacing;
				}
				else
				{
					if( m_rNetworkManager.matchInfo == null )
					{
						if( m_rNetworkManager.matches == null )
						{
							if( GUI.Button( new Rect( xpos, ypos, 200, 20 ), "Create Internet Match" ) )
							{
								m_rNetworkManager.matchMaker.CreateMatch( m_rNetworkManager.matchName, m_MatchSize, true, m_RoomPass, m_rNetworkManager.OnMatchCreate );
							}
							ypos += spacing;

							GUI.Label( new Rect( xpos, ypos, 100, 20 ), "Room Name :  " );
							m_rNetworkManager.matchName =   GUI.TextField( new Rect( xpos+100, ypos, 100, 20 ), m_rNetworkManager.matchName );
							ypos    +=  spacing;
                            GUI.Label( new Rect( xpos, ypos, 100, 20 ), "Room Pass :  " );
							m_RoomPass  =   GUI.TextField( new Rect( xpos+100, ypos, 100, 20 ), m_RoomPass );
							ypos    +=  spacing;
							ypos    +=  10;

							if( GUI.Button( new Rect( xpos, ypos, 200, 20 ), "Find Internet Match" ) )
							{
								m_rNetworkManager.matchMaker.ListMatches( 0, 20, m_NameFilter, m_rNetworkManager.OnMatchList );
							}
							ypos += spacing;

                            GUI.Label( new Rect( xpos, ypos, 100, 20 ), "Name Filter :  " );
							m_NameFilter    =   GUI.TextField( new Rect( xpos+100, ypos, 100, 20 ), m_NameFilter );

                            ypos += spacing;
                            ypos += 10;
						}
						else
						{
                            //  リロード
                            if( GUI.Button( new Rect( xpos, ypos, 200, 20 ), "Reload" ) )
							{
								m_rNetworkManager.matchMaker.ListMatches( 0, 20, m_NameFilter, m_rNetworkManager.OnMatchList );
							}
							ypos += spacing;
                            ypos += 10;

                            //  部屋リスト表示
							foreach( var match in m_rNetworkManager.matches )
							{
                                //  部屋に参加
								if( GUI.Button( new Rect( xpos, ypos, 200, 20 ), "Join Match :  " + match.name ) )
								{
									m_rNetworkManager.matchName = match.name;
									m_rNetworkManager.matchSize = (uint)match.currentSize;
									m_rNetworkManager.matchMaker.JoinMatch(match.networkId, m_JoinPass, m_rNetworkManager.OnMatchJoined );
								}
								ypos += spacing;
							}

                            //  参加パスワードを設定
                            if( m_rNetworkManager.matches.Count > 0 ){
                                GUI.Label( new Rect( xpos, ypos, 100, 20 ), "Join Pass :  " );
							    m_JoinPass  =   GUI.TextField( new Rect( xpos+100, ypos, 100, 20 ), m_JoinPass );
                                ypos += spacing;
                            }
                            //  部屋が見つからなかった 
                            else{
                                GUIContent  content =   new GUIContent( "___  Room Not Found  ___" );
                                GUIStyle    style   =   new GUIStyle( GUI.skin.label );
                                Vector2     size    =   style.CalcSize( content );
                                Rect        rect    =   FunctionManager.AdjustRectCanvasToGUI(
                                    FunctionManager.AR_TYPE.MIDDLE_CENTER,
                                    new Rect( 0, 0, size.x, size.y ),
                                    new Vector2( 0.5f, 0.5f ),
                                    new Vector2( 200, 20 )
                                );
                                GUI.Label( new Rect( xpos + rect.x, ypos, 300, 20 ), content );
                                ypos += spacing; 
                            }
                            ypos += 10;
                            ypos += 10;
						}
					}

					if( GUI.Button( new Rect( xpos, ypos, 200, 20 ), "Disable Match Maker" ) )
					{
						m_rNetworkManager.StopMatchMaker();
					}
					ypos += spacing;
				}
			}
		}

        //  アクセス
        public  void    Stop()
        {
            m_rNetworkManager.StopHost();
            if( m_rDiscovery.running )          m_rDiscovery.StopBroadcast();
            if( NetworkTransport.IsStarted )    NetworkTransport.Shutdown();
        }

        //  コールバック
        static  void    OnMatchJoinRequest_CallBack( Match.JoinMatchResponse _Response )
        {

        }
	}
};
#endif //ENABLE_UNET
