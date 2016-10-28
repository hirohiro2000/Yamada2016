
#if ENABLE_UNET

namespace   UnityEngine.Networking
{
	[ AddComponentMenu( "Network/NetworkManagerHUD" ) ]
	[ System.ComponentModel.EditorBrowsable( System.ComponentModel.EditorBrowsableState.Never ) ]
	public  class   NetworkManagerHUD   :   MonoBehaviour
	{
		public  NetworkManager  m_rNetworkManager   =   null;
        public  uint            m_MatchSize         =   4;

        public  bool            showGUI             =   true;

        private int             offsetX             =   0;
        private int             offsetY             =   0;

        private string          m_RoomPass          =   "";
        private string          m_JoinPass          =   "";
        private string          m_NameFilter        =   "";

		// Runtime variable
		private bool            showServer          =   false;

		void    Awake()
		{
			m_rNetworkManager   =   GetComponent< NetworkManager >();
		}

		void    Update()
		{
			if( !showGUI )	return;

			if( !NetworkClient.active
            &&  !NetworkServer.active
            &&  m_rNetworkManager.matchMaker == null )
			{
				if( Input.GetKeyDown( KeyCode.S ) ) m_rNetworkManager.StartServer();
				if( Input.GetKeyDown( KeyCode.H ) ) m_rNetworkManager.StartHost();
				if( Input.GetKeyDown( KeyCode.C ) ) m_rNetworkManager.StartClient();
			}
			if( NetworkServer.active
            &&  NetworkClient.active )
			{
				if( Input.GetKeyDown( KeyCode.X ) ) m_rNetworkManager.StopHost();
			}
		}

		void    OnGUI()
		{
			if( !showGUI )  return;

			int xpos = 10 + offsetX;
			int ypos = 40 + offsetY;
			int spacing = 24;

			if( !NetworkClient.active
            &&  !NetworkServer.active
            &&  m_rNetworkManager.matchMaker == null )
			{
				if( GUI.Button( new Rect( xpos, ypos, 200, 20 ), "LAN Host(H)" ) )          m_rNetworkManager.StartHost();
				ypos    +=  spacing;

				if( GUI.Button( new Rect( xpos, ypos, 105, 20 ), "LAN Client(C)" ) )        m_rNetworkManager.StartClient();
				m_rNetworkManager.networkAddress
                    =   GUI.TextField( new Rect( xpos + 110, ypos, 90, 20 ), m_rNetworkManager.networkAddress );
				ypos    +=  spacing;

				if( GUI.Button( new Rect( xpos, ypos, 200, 20 ), "LAN Server Only(S)" ) )   m_rNetworkManager.StartServer();
				ypos    +=  spacing;
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
            ||  NetworkClient.active )
			{
				if( GUI.Button( new Rect( xpos, ypos, 200, 20 ), "Stop (X)" ) )
				{
					m_rNetworkManager.StopHost();
				}
				ypos += spacing;
			}

			if( !NetworkServer.active
            &&  !NetworkClient.active )
			{
				ypos += 10;

				if( m_rNetworkManager.matchMaker == null )
				{
					if( GUI.Button( new Rect( xpos, ypos, 200, 20 ), "Enable Match Maker (M)" ) )
					{
						m_rNetworkManager.StartMatchMaker();
					}
					ypos += spacing;
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

					if( GUI.Button( new Rect( xpos, ypos, 200, 20 ), "Change MM server" ) )
					{
						showServer = !showServer;
					}
					if( showServer )
					{
						ypos += spacing;
						if( GUI.Button( new Rect( xpos, ypos, 100, 20 ), "Local" ) )
						{
							m_rNetworkManager.SetMatchHost( "localhost", 1337, false );
							showServer  =   false;
						}
						ypos += spacing;
						if( GUI.Button( new Rect( xpos, ypos, 100, 20 ), "Internet" ) )
						{
							m_rNetworkManager.SetMatchHost( "mm.unet.unity3d.com", 443, true );
							showServer = false;
						}
						ypos += spacing;
						if( GUI.Button( new Rect( xpos, ypos, 100, 20 ), "Staging" ) )
						{
							m_rNetworkManager.SetMatchHost( "staging-mm.unet.unity3d.com", 443, true );
							showServer  =   false;
						}
					}

					ypos += spacing;

					GUI.Label( new Rect( xpos, ypos, 300, 20 ), "MM Uri: " + m_rNetworkManager.matchMaker.baseUri );
					ypos += spacing;

					if( GUI.Button( new Rect( xpos, ypos, 200, 20 ), "Disable Match Maker" ) )
					{
						m_rNetworkManager.StopMatchMaker();
					}
					ypos += spacing;
				}
			}
		}

        //  コールバック
        static  void    OnMatchJoinRequest_CallBack( Match.JoinMatchResponse _Response )
        {

        }
	}
};
#endif //ENABLE_UNET
