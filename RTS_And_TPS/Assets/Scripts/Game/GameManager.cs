
using   UnityEngine;
using   UnityEngine.UI;
using   UnityEngine.Networking;
using   UnityEngine.SceneManagement;
using   System.Collections;
using   System.Collections.Generic;

public class GameManager : NetworkBehaviour {

    //  シーン内の状態 
    public  enum    State{
        Ready,      //  準備時間
        CountDown,  //  開始までのカウントダウン中
        WaveReady,  //  ウェーブ開始準備
        InGame,     //  ゲーム中
        GameOver,   //  ゲームオーバー
        Reuslt,     //  結果発表
    }
    //  メッセージ
    public  class   MainMessage{
        public  string  message;
        public  float   delay;
        public  float   displayTime;
        public  MainMessage(){
            message     =   "";
            delay       =   0.0f;
            displayTime =   0.0f;
        }
    }

    //  公開パラメータ
    public  string                  c_RetryScene    =   "";
    public  Font                    c_Font          =   null;
    public  GameObject              c_StageDrum     =   null;

    //  固定パラメータ
    public  float                   c_StartCDTime   =   3.0f;
    private float                   c_WaveInterval  =   30.0f;
    public  float                   c_StartResource =   150.0f;

    //  内部パラメータ
    [ SyncVar ]
    private State                   m_State         =   State.Ready;
    [ SyncVar ]
    private float                   m_StateTimer    =   0.0f;
    [ SyncVar ]
    public  float                   m_GameSpeed     =   1.0f;
    [ SyncVar ]
    public  int                     m_WaveLevel     =   0;
    [ SyncVar ]
    public  float                   m_Resource      =   0.0f;
    [ SyncVar ]
    public  float                   m_GlobalScore   =   0.0f;

    private Queue< MainMessage >    m_MainMessageQ  =   new Queue< MainMessage >();
    private float                   m_SDPlaceTimer  =   0.0f;
    private float                   m_PreStateTimer =   0.0f;

    //  共有パラメータ
    private SyncListBool            m_rIsReadyList  =   new SyncListBool();
    private SyncListString          m_rNameList     =   new SyncListString();
    private SyncListFloat           m_rScoreList    =   new SyncListFloat();
    private SyncListFloat           m_rDamageList   =   new SyncListFloat();
    private SyncListInt             m_rKillList     =   new SyncListInt();
    private SyncListInt             m_rDeathList    =   new SyncListInt();
    private SyncListInt             m_rRivivalList  =   new SyncListInt();
    private SyncListInt             m_rHSKillList   =   new SyncListInt();

    //  外部へのアクセス
    private LinkManager             m_rLinkManager  =   null;
    private WaveManager             m_rWaveManager  =   null;
    private SkyManager              m_rSkyManager   =   null;
    private Transform               m_rResources    =   null;

    private Text                    m_rWaveText     =   null;
    private Text                    m_rResourceText =   null;
    private Text                    m_rResourceTextOutline =   null;
    private Text                    m_rScoreText    =   null;
    private AcqScore_Control        m_rAcqScore     =   null;
    private AcqRecord_Control       m_rAcqRecord    =   null;
    private AcqScore_Control        m_rAcqResource  =   null;
    private DamageFilter_Control    m_rDFControl    =   null;

    private StageDrum_Control       m_rSDrumControl =   null;

    //  関連パラメータ
    //private SoundController         m_rHeartSound   =   null;

	// Use this for initialization
	void    Start()
    {
        //  アクセスを取得 
        m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );
        m_rWaveManager  =   FunctionManager.GetAccessComponent< WaveManager >( "EnemySpawnRoot" );
        m_rSkyManager   =   FunctionManager.GetAccessComponent< SkyManager >( "SkyManager" );
        m_rResources    =   FunctionManager.GetAccessComponent< Transform >( "FieldResources" );
         
        m_rWaveText     =   FunctionManager.GetAccessComponent< Text >( "Canvas/Column_Wave/Text_Score" );
        m_rResourceText =   FunctionManager.GetAccessComponent< Text >( "Canvas/Column_Resource/Text_Resource" );
        m_rResourceTextOutline =   FunctionManager.GetAccessComponent< Text >( "Canvas/Column_Resource/Text_Resource (Outline)" );
        m_rScoreText    =   FunctionManager.GetAccessComponent< Text >( "Canvas/Column_Score/Text_Score" );
        m_rAcqScore     =   FunctionManager.GetAccessComponent< AcqScore_Control >( "Canvas/AcqScore" );
        m_rAcqRecord    =   FunctionManager.GetAccessComponent< AcqRecord_Control >( "Canvas/AcqRecord" );
        m_rAcqResource  =   FunctionManager.GetAccessComponent< AcqScore_Control >( "Canvas/AcqResource" );
        m_rDFControl    =   FunctionManager.GetAccessComponent< DamageFilter_Control >( "Canvas/DamageFilter" );

        //  パラメータを初期化
        m_Resource      =   c_StartResource;

        //  共通の処理
        {
            //  空設定
            m_rSkyManager.ChangeSky( 0 );

            //  BGM再生開始
            BGMManager.ChangeBGM( "BGM_Ready", 0.5f, 0.0f, 0.0f, 0.0f );
        }

        //  サーバーでの処理
        if( NetworkServer.active ){
            ReplaceStageDrum();
        }
	}
	
	// Update is called once per frame
	void    Update()
    {
        //  共通の処理 
        UpdateIn_Common();

        //  サーバー側の処理
        if( NetworkServer.active )  UpdateIn_Server();
        //  クライアント側の処理
        else                        UpdateIn_Client();
	}
    void    UpdateIn_Common()
    {
        //  タイムスケール更新
        if( m_State == State.InGame
        ||  m_State == State.CountDown )    Time.timeScale  =   m_GameSpeed;
        else                                Time.timeScale  =   1.0f;

        //  共通の処理
        if( m_MainMessageQ.Count > 0 ){
            MainMessage rMessage    =   m_MainMessageQ.Peek();

            //  タイマー更新
            if( rMessage.delay > 0.0f ) rMessage.delay         =   Mathf.Max( rMessage.delay       - Time.deltaTime, 0.0f );
            else                        rMessage.displayTime   =   Mathf.Max( rMessage.displayTime - Time.deltaTime, 0.0f );
            if( rMessage.displayTime == 0.0f ){
                m_MainMessageQ.Dequeue();
            }
        }

        //  カウント音
        {
            int curCount    =   ( int )( c_StartCDTime  - m_StateTimer ) + 1;
            if( m_State == State.WaveReady ){
                curCount    =   ( int )( c_WaveInterval - m_StateTimer ) + 1;
            }

            //  カウントチェック
            if( m_State == State.CountDown
            ||  m_State == State.WaveReady ){
                int soundTime   =   10;
                for( int i = 1; i < soundTime + 1; i++ ){
                    if( curCount        <= i
                    &&  m_PreStateTimer >  i ){
                        SoundController.PlayNow( "UI_Clock", 0.0f, 0.075f, 1, 1.0f );
                    }
                }
            }

            //  現在のタイマーを保存
            m_PreStateTimer =   curCount;
        }

        //  空模様を更新
        m_rSkyManager.ChangeSky( Mathf.Max( m_WaveLevel - 1, 0 ) );

        //  ＵＩの更新  
        {
            if( m_rWaveText )               m_rWaveText.text                =   m_WaveLevel.ToString();
            if( m_rResourceText )           m_rResourceText.text            =   ( ( int )m_Resource ).ToString();
            if( m_rResourceTextOutline )    m_rResourceTextOutline.text     =   ( ( int )m_Resource ).ToString();
            if( m_rScoreText )              m_rScoreText.text               =   ( ( int )m_GlobalScore ).ToString();

            //  ダメージフィルター更新  
            if( m_rLinkManager
            &&  m_rLinkManager.m_rLocalPlayer ){
                GameObject      rMyPlayer   =   m_rLinkManager.m_rLocalPlayer;
                TPSPlayer_HP    rHealth     =   ( rMyPlayer )? rMyPlayer.GetComponent< TPSPlayer_HP >() : null;
                float           dyingLine   =   0.5f;

                if( rHealth ){
                    //  ピンチフィルター 
                    if( rHealth.m_CurHP <= rHealth.m_MaxHP * dyingLine )    m_rDFControl.SetEffect_Dying( true );
                    else                                                    m_rDFControl.SetEffect_Dying( false );
                }
                else{
                    m_rDFControl.SetEffect_Dying( false );
                }
            }
        }

        //  キー入力  
        if( Input.GetKeyDown( KeyCode.Return ) ){
            if( m_State == State.CountDown
            &&  !GetFromList_IsReady( m_rLinkManager.m_LocalPlayerID ) ){
                if( NetworkServer.active )  SetToList_IsReady( m_rLinkManager.m_LocalPlayerID, true );
                else                        m_rLinkManager.m_rLocalNPControl.CmdSend_GMIsReady( true );
            }
            if( m_State == State.WaveReady
            &&  m_StateTimer >= 6.8f
            &&  !GetFromList_IsReady( m_rLinkManager.m_LocalPlayerID ) ){
                if( NetworkServer.active )  SetToList_IsReady( m_rLinkManager.m_LocalPlayerID, true );
                else                        m_rLinkManager.m_rLocalNPControl.CmdSend_GMIsReady( true );
            }
        }
    }
    void    UpdateIn_Server()
    {
        //  リストの項目数を調整
        UpdateListItem();

        //  ドラム缶配置タイマー更新
        if( m_SDPlaceTimer > 0.0f ){
            m_SDPlaceTimer  -=  Time.deltaTime;
            m_SDPlaceTimer  =   Mathf.Max( m_SDPlaceTimer, 0.0f );
            if( m_SDPlaceTimer <= 0.0f ){
                ReplaceStageDrum();
            }
        }

        //  状態に応じて処理を行う
	    switch( m_State ){
            case    State.Ready:        Update_Ready();     break;
            case    State.CountDown:    Update_CountDown(); break;
            case    State.WaveReady:    Update_WaveReady(); break;
            case    State.InGame:       Update_InGame();    break;
            case    State.GameOver:     Update_GameOver();  break;
            case    State.Reuslt:       Update_Result();    break;
        }
    }
    void    UpdateIn_Client()
    {

    }
    
    void    Update_Ready()
    {
        //  タイマーを進める
        m_StateTimer    +=  Time.deltaTime;
    }
    void    Update_CountDown()
    {
        //  タイマーを進める
        m_StateTimer    +=  Time.deltaTime;
        //  ゲーム開始
        if( m_StateTimer >= c_StartCDTime
        ||  CountIsReady() >= NetworkManager.singleton.numPlayers ){
            //  ゲーム開始
            ChangeState( State.InGame );
            //  ウェーブ開始
            m_rWaveManager.StartWave();

            //  ウェーブ開始処理
            StartWave();
        }
    }
    void    Update_WaveReady()
    {
        //  タイマーを進める
        m_StateTimer    +=  Time.deltaTime;
        if( m_StateTimer >= c_WaveInterval
        ||  CountIsReady() >= NetworkManager.singleton.numPlayers ){
            //  ゲーム開始
            ChangeState( State.InGame );
            //  ウェーブ開始
            m_rWaveManager.StartWave();

            //  ウェーブ開始処理
            StartWave();
        }
    }
    void    Update_InGame()
    {
        //  タイマーを進める
        m_StateTimer    +=  Time.deltaTime;
    }
    void    Update_GameOver()
    {
        //  タイマーを進める
        m_StateTimer    +=  Time.deltaTime;

        //  タイマーチェック
        if( m_StateTimer >= 1.7f + 7.5f ){ 
            //  リストをクリア
            for( int i = 0; i < m_rIsReadyList.Count; i++ ){
                m_rIsReadyList[ i ] =   false;
            }
            //  リザルト画面へ
            ChangeState( State.Reuslt );
        }
    }
    void    Update_Result()
    {
        //  タイマーを進める
        m_StateTimer    +=  Time.deltaTime;

        //  全員の準備が完了したらシーンをリロード
        if( CountIsReady() >= NetworkManager.singleton.numPlayers ){
            //  シーンをリロード
            NetworkManager.singleton.ServerChangeScene( c_RetryScene );
        }
    }

    //  リスト更新
    void    UpdateListItem()
    {
        //  クライアント数
        int numClient   =   NetworkServer.connections.Count;
        
        //  項目数更新
        CheckWhetherExist_Bool( m_rIsReadyList, numClient );
    }

    //  ＵＩ描画
    void    OnGUI()
    {
        //  ゲームスピード変更
        {
            Rect    rect    =   FunctionManager.AdjustRectCanvasToGUI(
                FunctionManager.AR_TYPE.TOP_RIGHT,
                new Rect( -96.0f, -68.0f, 100, 20 )
            );
            if( GUI.Button( rect, "x " + ( int )m_GameSpeed ) ){
                m_rLinkManager.m_rLocalNPControl.CmdChange_GameSpeed( ( m_GameSpeed > 1.0f )? 1.0f : 2.0f );
            }
        }
        //  外部からのメッセージ
        if( m_MainMessageQ.Count > 0 ){
            MainMessage rMessage    =   m_MainMessageQ.Peek();
            if( rMessage.displayTime >  0.0f
            &&  rMessage.delay       == 0.0f ){
                PrintMessage( rMessage.message );
            }
        }

        //  カウントダウン
        if( m_State == State.CountDown
        &&  !GetFromList_IsReady( m_rLinkManager.m_LocalPlayerID ) ){
            PrintMessage( "ゲーム開始まであと  " + ( int )( c_StartCDTime - m_StateTimer + 1 ) + "  秒"
                +   "\n" + "Enter キーで開始" );
        }
        if( m_State == State.CountDown
        &&  GetFromList_IsReady( m_rLinkManager.m_LocalPlayerID ) ){
            PrintMessage( "ゲーム開始まであと  " + ( int )( c_StartCDTime - m_StateTimer + 1 ) + "  秒"
                +   "\n" + "他のプレイヤーを待っています" );
        }

        //  ゲームが開始されました 
        if( m_State == State.InGame
        &&  m_WaveLevel == 1
        &&  CheckTimeShift( m_StateTimer, 1.2f, 1.2f + 5.0f ) ){
            PrintMessage( "敵が拠点への侵攻を開始しました" );
        }

        //  ウェーブインターバル
        if( m_State == State.WaveReady
        &&  m_StateTimer >= 6.8f
        &&  !GetFromList_IsReady( m_rLinkManager.m_LocalPlayerID ) ){
            if( m_WaveLevel % 3 == 2 ){
                PrintMessage( "敵の大軍が接近中です… あと  " + ( int )( c_WaveInterval - m_StateTimer + 1 ) + "  秒"
                +   "\n" + "Enter キーで開始" );
            }
            else{
                PrintMessage( "次のウェーブまであと  " + ( int )( c_WaveInterval - m_StateTimer + 1 ) + "  秒"
                +   "\n" + "Enter キーで開始" );
            }
        }
        if( m_State == State.WaveReady
        &&  m_StateTimer >= 6.8f
        &&  GetFromList_IsReady( m_rLinkManager.m_LocalPlayerID ) ){
            if( m_WaveLevel % 3 == 2 ){
                PrintMessage( "敵の大軍が接近中です… あと  " + ( int )( c_WaveInterval - m_StateTimer + 1 ) + "  秒"
                +   "\n" + "他のプレイヤーを待っています" );
            }
            else{
                PrintMessage( "次のウェーブまであと  " + ( int )( c_WaveInterval - m_StateTimer + 1 ) + "  秒"
                +   "\n" + "他のプレイヤーを待っています" );
            }
        }

        //  拠点が破壊されました 
        if( m_State == State.GameOver
        &&  CheckTimeShift( m_StateTimer, 1.7f, 1.7f + 3.7f ) ){
            PrintMessage( "味方が全滅しました" );
        }
        //  ゲーム終了
        if( m_State == State.GameOver
        &&  CheckTimeShift( m_StateTimer, 5.2f + 1.7f, 5.2f + 1.2f + 3.2f ) ){
            PrintMessage( "ゲーム 終了" );
        }

        //  リザルト画面表示
        if( m_State == State.Reuslt
        &&  m_StateTimer >= 1.7f
        &&  !GetFromList_IsReady( m_rLinkManager.m_LocalPlayerID ) )
        {
            ResultButton_Input  input;
            input   =   PrintResult( m_StateTimer - 1.7f );

            //  続けるボタンが押された
            if( input.pushOK )  m_rLinkManager.m_rLocalNPControl.CmdSend_GMIsReady( true );
            //  やめるボタンが押された
            if( input.pushQuit ){}
        }

        //  ほかのプレイヤーを待っています 
        if( m_State == State.Reuslt
        &&  GetFromList_IsReady( m_rLinkManager.m_LocalPlayerID ) ){
            PrintMessage( "他のプレイヤーを待っています" );
        }
    }

    //  メッセージを表示
    void    PrintMessage( string _Content )
    {
        PrintMessage( _Content, -150.0f );
    }
    void    PrintMessage( string _Content, float _Height )
    {
        GUIStyle    countStyle  =   new GUIStyle( GUI.skin.label );
        countStyle.fontSize     =   15;
        countStyle.fontStyle    =   FontStyle.Normal;
        countStyle.font         =   c_Font;

        PrintMessage( _Content, _Height, countStyle );
    }
    void    PrintMessage( string _Content, float _Height, GUIStyle _Style )
    {
        Vector2 contentSize =   _Style.CalcSize( new GUIContent( _Content ) );

        //  フレーム表示
        {
            Rect    rect    =   FunctionManager.AdjustRectCanvasToGUI(
                FunctionManager.AR_TYPE.TOP_CENTER,
                new Rect( 0.0f, _Height, contentSize.x + 36.0f, contentSize.y + 8.0f )
            );
            GUI.Box( rect, "" );
        }

        //  内容を表示
        {
            Rect    rect    =   FunctionManager.AdjustRectCanvasToGUI(
                FunctionManager.AR_TYPE.TOP_CENTER,
                new Rect( 0.0f, _Height - 1.0f, contentSize.x, contentSize.y )
            );

            GUI.color           =   Color.white;
            GUI.backgroundColor =   Color.white;

            GUI.Label( rect, _Content, _Style );
        }
    }
    //  リザルト画面表示 
    struct  ResultButton_Input{
        public  bool    pushOK;
        public  bool    pushQuit;
        public  ResultButton_Input( bool _PushOK, bool _PushQuit ){
            pushOK      =   _PushOK;
            pushQuit    =   _PushQuit;
        }
    };
    struct  ForSort{
        public  int id;
        public  int score;
        public  ForSort( int _ID, int _Score ){
            id      =   _ID;
            score   =   _Score;
        }
    };
    ResultButton_Input  PrintResult( float _Timer )
    {
        //  ボタンの入力データ
        ResultButton_Input  inputData   =   new ResultButton_Input( false, false );

        //  グループ開始
        {
            Rect    rect    =   FunctionManager.AdjustRectCanvasToGUI(
                FunctionManager.AR_TYPE.MIDDLE_CENTER,
                new Rect( 0.0f, 0.0f, 480.0f, 310.0f )
            );
            GUI.BeginGroup( rect );
        }

        //  フレーム
        GUI.Box( new Rect( 0.0f, 0.0f, 480.0f, 310.0f ), "Result" );
        
        //  項目の表示
        {
            string[]    rankSymbol  =   {   "1st",  "2nd",  "3rd",  "4th",  "5th",  "6th",  "7th",  "8th"   };
            string[]    playerName  =   {   "PLAYER_A", "PLAYER_B", "PLAYER_C", "PLAYER_D", "PLAYER_E", "PLAYER_F", "PLAYER_G", "PLAYER_H"  };

            //  名前を設定
            for( int i = 0; i < m_rNameList.Count; i++ ){
                playerName[ i ] =   m_rNameList[ i ];
            }

            //  順位を決定
            List< ForSort > rSortList   =   new List< ForSort >();
            {
                for( int i = 0; i < 8; i++ )    rSortList.Add( new ForSort( i, -100 ) );
                for( int i = 0; i < m_rNameList.Count; i++ ){
                    rSortList[ i ]  =   new ForSort( i, ( m_rScoreList.Count > i )? ( int )m_rScoreList[ i ] : 0 );
                }
                rSortList.Sort( ( a, b ) => b.score - a.score );
            }

            //  項目の種類
            {
                GUI.BeginGroup( new Rect( -1.0f, 28.0f, 480.0f, 26 ) );

                //GUI.Label( new Rect( 174, 0.0f, 480.0f, 26.0f ), "Score" );
                GUI.Label( new Rect( 264, 0.0f, 480.0f, 26.0f ), "Kill" );
                GUI.Label( new Rect( 311, 0.0f, 480.0f, 26.0f ), "Death" );
                GUI.Label( new Rect( 364, 0.0f, 480.0f, 26.0f ), "Revive" );
                GUI.Label( new Rect( 420, 0.0f, 480.0f, 26.0f ), "Damage" );

                GUI.EndGroup();
            }

            //  表示
            float   interval_0  =   0.5f;//0.4f;
            float   interval_1  =   1.0f;//0.8f;
            for( int i = 0; i < 8; i++ ){
                if( i <  3 && _Timer - 0.5f < interval_0 * 5 + interval_1 * ( 3 - i ) )  continue;
                if( i >= 3 && _Timer - 0.5f < interval_0 * ( 8 - i ) )                   continue;

                int     playerID    =   rSortList[ i ].id;
                bool    isActiveID  =   m_rLinkManager.CheckActiveClient( playerID );
                //if( !CheckActiveClient( playerID ) )                                    continue;

                //  自分の項目をピックアップ
                if( playerID == m_rLinkManager.m_LocalPlayerID ){
                    GUI.Box( new Rect( 0.0f, 50.0f + 26.0f * i, 480.0f, 26.0f ), "" );
                }

                GUI.BeginGroup( new Rect( -1.0f, 52.0f + 26.0f * i, 480.0f, 26 ) );

                GUI.Label( new Rect( 26.0f, 0.0f, 480.0f, 26.0f ), rankSymbol[ i ] );

                //  スコアを表示 
                {
                    GUIStyle    fontStyle   =   new GUIStyle( GUI.skin.label );
                    fontStyle.alignment     =   TextAnchor.MiddleRight;

                    string      content     =   ( m_rScoreList.Count > playerID )? ( ( int )m_rScoreList[ playerID ] ).ToString() : "_______";
                    Vector2     contentSize =   fontStyle.CalcSize( new GUIContent( content ) );

                    GUI.color           =   Color.white;
                    GUI.backgroundColor =   Color.white;

                    GUI.Label( new Rect( 217.0f - contentSize.x, 0.0f, contentSize.x, contentSize.y ), content, fontStyle );
                }

                //GUI.Label( new Rect( 168.0f, 0.0f, 480.0f, 26.0f ), ( isActiveID )? ( ( int ) m_rScoreList[ playerID ] ).ToString().PadLeft( 7, '_' ) : "_______" );

                GUI.Label( new Rect( 262.0f, 0.0f, 480.0f, 26.0f ), ( isActiveID )?( ( m_rKillList.Count  > playerID )?   m_rKillList[ playerID ].ToString().PadLeft( 3, '_' ) : "__0" )    : "___" );
                GUI.Label( new Rect( 318.0f, 0.0f, 480.0f, 26.0f ), ( isActiveID )?( ( m_rDeathList.Count > playerID )?   m_rDeathList[ playerID ].ToString().PadLeft( 3, '_' ) : "__0" )   : "___" );
                GUI.Label( new Rect( 374.0f, 0.0f, 480.0f, 26.0f ), ( isActiveID )?( ( m_rRivivalList.Count > playerID )? m_rRivivalList[ playerID ].ToString().PadLeft( 3, '_' ) : "__0" ) : "___" );
                GUI.Label( new Rect( 432.0f, 0.0f, 480.0f, 26.0f ), ( isActiveID )?( ( m_rDamageList.Count > playerID )?  ( ( int )m_rDamageList[ playerID ] ).ToString().PadLeft( 3, '_' ) : "__0" )  : "___" );
                if( isActiveID )    GUI.Label( new Rect( 64.0f, 0.0f, 480.0f, 26.0f ), playerName[ playerID ] );
                else                GUI.Label( new Rect( 64.0f, 0.0f, 480.0f, 26.0f ), "________" );

                GUI.EndGroup();
            }
        }

        //  ボタン表示
        if( _Timer - 0.5f > 5.5f + 2.0f ){
            inputData.pushOK    =   GUI.Button( new Rect( 100, 272, 100.0f, 26.0f ), "続ける" );
            inputData.pushQuit  =   GUI.Button( new Rect( 280, 272, 100.0f, 26.0f ), "やめる" );
        }

        //  グループ終了
        GUI.EndGroup();

        return  inputData;
    }

    //  状態管理
    void    ChangeState( State _NextState )
    {
        m_State         =   _NextState;
        m_StateTimer    =   0.0f;

        //  リストをクリア
        for( int i = 0; i < m_rIsReadyList.Count; i++ ){
            m_rIsReadyList[ i ] =   false;
        }
    }
    bool    CheckTimeShift( float _Start, float _End )
    {
        return  CheckTimeShift( m_StateTimer, _Start, _End );
    }
    bool    CheckTimeShift( float _Timer, float _Start, float _End )
    {
        if( _Timer >= _Start
        &&  _Timer <  _End )    return  true;
        else                    return  false;
    }

    //  リスト操作
    public  void    SetToList_IsReady( int _ClientID, bool _IsReady )
    {
        //  項目がなければ拡張する
        CheckWhetherExist_Bool( m_rIsReadyList, _ClientID + 1 );

        //  値を設定
        m_rIsReadyList[ _ClientID ] =   _IsReady;
    }
    public  void    SetToList_PlayerName( int _ClientID, string _Name )
    {
        //  項目がなければ拡張する
        CheckWhetherExist_String( m_rNameList, _ClientID + 1 );

        //  値を設定
        m_rNameList[ _ClientID ] =   _Name;
    }
    public  void    SetToList_Score( int _ClientID, float _AddScore )
    {
        //  項目がなければ拡張する
        CheckWhetherExist_Float( m_rScoreList, _ClientID + 1 );

        //  値を設定
        m_rScoreList[ _ClientID ]   +=  _AddScore;
    }
    public  void    SetToList_Damage( int _ClientID, float _AddDamage )
    {
        //  項目がなければ拡張する
        CheckWhetherExist_Float( m_rDamageList, _ClientID + 1 );

        //  値を設定
        m_rDamageList[ _ClientID ]  +=  _AddDamage;
    }
    public  void    SetToList_Kill( int _ClientID, int _AddValue )
    {
        //  項目がなければ拡張する
        CheckWhetherExist_Int( m_rKillList, _ClientID + 1 );

        //  値を設定
        m_rKillList[ _ClientID ]    +=  _AddValue;
    }
    public  void    SetToList_Death( int _ClientID, int _AddValue )
    {
        //  項目がなければ拡張する
        CheckWhetherExist_Int( m_rDeathList, _ClientID + 1 );

        //  値を設定
        m_rDeathList[ _ClientID ]   +=  _AddValue;
    }
    public  void    SetToList_Rivival( int _ClientID, int _AddValue )
    {
        //  項目がなければ拡張する
        CheckWhetherExist_Int( m_rRivivalList, _ClientID + 1 );

        //  値を設定
        m_rRivivalList[ _ClientID ] +=  _AddValue;
    }
    public  void    SetToList_HSKill( int _ClientID, int _AddValue )
    {
        //  項目がなければ拡張する
        CheckWhetherExist_Int( m_rHSKillList, _ClientID + 1 );

        //  値を設定
        m_rHSKillList[ _ClientID ]  +=  _AddValue;
    }

    public  bool    GetFromList_IsReady( int _ClientID )
    {
        return  m_rIsReadyList[ _ClientID ];
    }
    public  string  GetFromList_PlayerName( int _ClientID )
    {
        return  m_rNameList[ _ClientID ];
    }
    public  float   GetFromList_Score( int _ClientID )
    {
        return  m_rScoreList[ _ClientID ];
    }
    public  float   GetFromList_Damage( int _ClientID )
    {
        return  m_rDamageList[ _ClientID ];
    }
    public  int     GetFromList_Kill( int _ClientID )
    {
        return  m_rKillList[ _ClientID ];
    }
    public  int     GetFromList_Death( int _ClientID )
    {
        return  m_rDeathList[ _ClientID ];
    }
    public  int     GetFromList_Rivival( int _ClientID )
    {
        return  m_rRivivalList[ _ClientID ];
    }
    public  int     GetFromList_HSKill( int _ClientID )
    {
        return  m_rHSKillList[ _ClientID ];
    }

    public  int     GetNumItem_IsReady()
    {
        return  m_rIsReadyList.Count;
    }
    public  int     GetNumItem_PlayerName()
    {
        return  m_rNameList.Count;
    }
    public  int     GetNumItem_Score()
    {
        return  m_rScoreList.Count;
    }
    public  int     GetNumItem_Damage()
    {
        return  m_rDamageList.Count;
    }
    public  int     GetNumItem_Kill()
    {
        return  m_rKillList.Count;
    }
    public  int     GetNumItem_Death()
    {
        return  m_rDeathList.Count;
    }
    public  int     GetNumItem_Rivival()
    {
        return  m_rRivivalList.Count;
    }
    public  int     GetNumItem_HSKill()
    {
        return  m_rHSKillList.Count;
    }

    //  準備が完了したプレイヤーを数える
    public  int     CountIsReady()
    {
        int count   =   0;
        for( int i = 0; i < m_rIsReadyList.Count; i++ ){
            if( m_rIsReadyList[ i ] )   ++count;
        }
        return  count;
    }

    //  項目数が足りているかどうか調べる（足りなければ追加する）
    void    CheckWhetherExist_Bool( SyncListBool _rList, int _NeedCount )
    {
        //  項目がなければ拡張する
        if( _rList.Count < _NeedCount ){
            //  必要な項目の数を計算
            int needItem    =   _NeedCount - _rList.Count;

            //  項目を追加
            for( int i = 0; i < needItem; i++ ){
                _rList.Add( false );
            }
        }
    }
    void    CheckWhetherExist_String( SyncListString _rList, int _NeedCount )
    {
        //  項目がなければ拡張する
        if( _rList.Count < _NeedCount ){
            //  必要な項目の数を計算
            int needItem    =   _NeedCount - _rList.Count;

            //  項目を追加
            for( int i = 0; i < needItem; i++ ){
                _rList.Add( "Null" );
            }
        }
    }
    void    CheckWhetherExist_Float( SyncListFloat _rList, int _NeedCount )
    {
        //  項目がなければ拡張する 
        if( _rList.Count < _NeedCount ){
            //  必要な項目の数を計算
            int needItem    =   _NeedCount - _rList.Count;

            //  項目を追加
            for( int i = 0; i < needItem; i++ ){
                _rList.Add( 0.0f );
            }
        }
    }
    void    CheckWhetherExist_Int( SyncListInt _rList, int _NeedCount )
    {
        //  項目がなければ拡張する
        if( _rList.Count < _NeedCount ){
            //  必要な項目の数を計算
            int needItem    =   _NeedCount - _rList.Count;

            //  項目を追加
            for( int i = 0; i < needItem; i++ ){
                _rList.Add( 0 );
            }
        }
    }    

    //  出撃済みプレイヤーの数を調べる
    int     CheckAlreadyPlayer()
    {
        GameObject[]    objList =   GameObject.FindGameObjectsWithTag( "Player" );
        int             count   =   0;
        for( int i = 0; i < objList.Length; i++ ){
            //  未出撃プレイヤーならスキップ
            if( objList[ i ].GetComponent< PlayerCommander_Control >() )    continue;

            //  カウントアップ
            ++count;
        }

        return  count;
    }

    //  インターバル開始処理
    void    StandbyProc_WaveInterval()
    {
        //  破壊されたタワーを修復
        for( int i = 0; i < m_rResources.childCount; i++ ){
            GameObject  rObj    =   m_rResources.GetChild( i ).gameObject;
            rObj.SetActive( false );
            rObj.SetActive( true );
        }

        //  ウェーブクリア音 
        SoundController.PlayNow( "WaveClear",  1.8f, 0.125f, 0.75f, 6.0f );
            //  ピーク時だけ大げさな効果音
            if( m_WaveLevel % 3 == 0 ){
                SoundController.PlayNow( "WaveClear2", 1.8f, 0.15f, 1.0f,  8.0f );
            }

        //  BGM変更
        if( m_WaveLevel % 3 == 0 )  BGMManager.ChangeBGM( "BGM_Interval_0", 0.5f, 2.0f, 1.0f, 6.5f );
        else                        BGMManager.ChangeBGM( "BGM_Interval_0", 0.5f, 2.0f, 3.0f, 6.5f );
    }
    void    StandbyProc_WaveIntervalInServer()
    {
        //  インターバル開始
        ChangeState( State.WaveReady );

        //  リストをクリア
        for( int i = 0; i < m_rIsReadyList.Count; i++ ){
            m_rIsReadyList[ i ] =   false;
        }

        //  一定ウェーブごとにドラム缶を再配置
        if( m_WaveLevel > 0
        &&  m_WaveLevel % 3 == 0 ){
            ReplaceStageDrum();
        }
    }
    //  ウェーブ開始処理
    void    StandbyProc_Wave()
    {
        //  ウェーブ開始音 
        SoundController.PlayNow( "WaveStart", 1.2f, 0.5f, 1.0f, 4.0f );

        //  BGM変更  
        {
            bool    isPeek  =   m_WaveLevel % 3 == 0 && m_WaveLevel > 0;
            if( isPeek ){
                int numBGM      =   3;
                int useBGMID    =   ( ( m_WaveLevel / 3 ) - 1 ) % numBGM;

                BGMManager.ChangeBGM( "BGM_InBattle_Peek_" + useBGMID, 0.5f, 0.0f, 1.0f, 2.0f );                
            }
            else{
                int numBGM      =   4;
                int useBGMID    =   ( m_WaveLevel / 3 ) % numBGM;

                BGMManager.ChangeBGM( "BGM_InBattle_" + useBGMID, 0.5f, 0.0f, 1.0f, 2.0f );
            }
        }
    }
    void    StandbyProc_WaveInServer()
    {

    }
    //  ゲームオーバー処理
    void    StandbyProc_GameOver()
    {
        //  BGM変更 
        BGMManager.ChangeBGM( "BGM_Result", 0.5f, 1.0f, 6.0f, 6.0f );
    }
    void    StandbyProc_GameOverInServer()
    {

    }

    //  外部からの操作
    public  void    StartCountDown()
    {
        if( m_State != State.Ready )    return;

        ChangeState( State.CountDown );
    }
    public  void    StartWaveInterval()
    {
        //  インターバル開始処理
        StandbyProc_WaveInterval();
        StandbyProc_WaveIntervalInServer();

        //  クライアントにも同じ処理をリクエスト
        RpcStartWaveInterval_StandbyProc();
    }
    public  void    StartWave()
    {
        //  ウェーブ開始処理
        StandbyProc_Wave();
        StandbyProc_WaveInServer();

        //  クライアントにも同じ処理をリクエスト
        RpcStartWave_StandbyProc();
    }

    public  void    GameOver()
    {
        if( m_State != State.InGame )   return;

        //  ゲームオーバー処理
        StandbyProc_GameOver();
        StandbyProc_GameOverInServer();

        //  クライアントにも同じ処理をリクエスト
        RpcGameOver_StandbyProc();

        //  ゲームオーバー
        ChangeState( State.GameOver );
    }

    public  void    SetMainMassage( string _Message, float _DisplayTime, float _Delay )
    {
        MainMessage rData   =   new MainMessage();
        rData.message       =   _Message;
        rData.displayTime   =   _DisplayTime;
        rData.delay         =   _Delay;

        m_MainMessageQ.Enqueue( rData );
    }
    public  void    SetAcqScoreNotrice( float _AddScore, int _ClientID )
    {
        if( m_rLinkManager.m_LocalPlayerID != _ClientID )   return;

        m_rAcqScore.SetAddScore( ( int )_AddScore );
    }
    public  void    SetAcqRecord( string _Record, int _ClientID )
    {
        if( m_rLinkManager.m_LocalPlayerID != _ClientID )   return;

        m_rAcqRecord.SetRecord( _Record );
    }
    public  void    SetAcqRecord( string _Record, float _DispTime, int _ClientID )
    {
        if( m_rLinkManager.m_LocalPlayerID != _ClientID )   return;

        m_rAcqRecord.SetRecord( _Record, _DispTime, AcqRecord_Control.ColorType.Default );
    }
    public  void    SetAcqRecord( string _Record, float _DispTime, int _ClientID, AcqRecord_Control.ColorType _ColorType )
    {
        if( m_rLinkManager.m_LocalPlayerID != _ClientID )   return;

        m_rAcqRecord.SetRecord( _Record, _DispTime, _ColorType );
    }
    public  void    SetAcqResource( float _AddResource )
    {
        m_rAcqResource.SetAddScore( _AddResource ); 
    }

    public  void    SetDamageFilterEffect()
    {
        SetDamageFilterEffect( 0.6f, 1.0f );
    }
    public  void    SetDamageFilterEffect( float _Time, float _Power )
    {
        m_rDFControl.SetEffect( _Time, _Power );
    }
    
    public  void    ReplaceStageDrum()
    {
        //  既に配置されている場合は不足分を補充
        if( m_rSDrumControl ){
            m_rSDrumControl.Supple();
        }
        //  まだ配置されていない場合は新しく生成
        else{
            //  新しいドラムを生成
            GameObject  rObj    =   Instantiate( c_StageDrum );

            //  アクセスを取得
            m_rSDrumControl     =   rObj.GetComponent< StageDrum_Control >();
        }
    }

    //  アクセス
    public  void    AddResource( float _AddValue )
    {
        m_Resource      +=  _AddValue;
        m_Resource      =   Mathf.Max( m_Resource, 0.0f );
    }
    public  void    AddGlobalScore( float _AddScore, int _ClientID )
    {
        m_GlobalScore   +=  _AddScore;

        //  個人スコア加算
        SetToList_Score( _ClientID, _AddScore );
    }
    public  State   GetState(){
        return  m_State;
    }
    public  float   GetGameSpeed()
    {
        return  m_GameSpeed;
    }
    public  float   GetResource()
    {
        return  m_Resource;
    }
    public  float   GetGlobalScore()
    {
        return  m_GlobalScore;
    }
//=========================================================================================
//      リクエスト
//=========================================================================================
    //  メッセージ共有
    [ ClientRpc ]
    public  void    RpcMainMessage( string _Message, float _DisplayTime, float _Delay )
    {
        if( NetworkServer.active )  return;

        SetMainMassage( _Message, _DisplayTime, _Delay );
    }
    //  インターバル開始処理
    [ ClientRpc ]
    public  void    RpcStartWaveInterval_StandbyProc()
    {
        //  サーバーでは処理を行わない
        if( NetworkServer.active )  return;

        //  インターバル開始処理
        StandbyProc_WaveInterval();
    }
    //  ウェーブ開始処理
    [ ClientRpc ]
    public  void    RpcStartWave_StandbyProc()
    {
        //  サーバーでは処理を行わない
        if( NetworkServer.active )  return;

        //  ウェーブ開始処理
        StandbyProc_Wave();
    }
    //  ゲームオーバー処理
    [ ClientRpc ]
    public  void    RpcGameOver_StandbyProc()
    {
        //  サーバーでは処理を行わない
        if( NetworkServer.active )  return;

        //  ゲームオーバー処理
        StandbyProc_GameOver();
    }
    //  スコア獲得通知
    [ ClientRpc ]
    public  void    RpcGetScoreNotice( float _AddScore, int _ClientID )
    {
        //  サーバーでは処理を行わない
        if( NetworkServer.active )                          return;
        //  指定されたプレイヤー以外は無視する
        if( m_rLinkManager.m_LocalPlayerID != _ClientID )   return;
        
        //  スコア獲得通知
        m_rAcqScore.SetAddScore( ( int )_AddScore );
    }
    //  レコードを通知
    [ ClientRpc ]
    public  void    RpcRecordNotice( string _Record, int _ClientID )
    {
        //  サーバーでは処理を行わない
        if( NetworkServer.active )                          return;
        //  指定されたプレイヤー以外は無視する
        if( m_rLinkManager.m_LocalPlayerID != _ClientID )   return;
        
        //  レコードを通知
        m_rAcqRecord.SetRecord( _Record );
    }
    [ ClientRpc ]
    public  void    RpcRecordNoticeD( string _Record, float _DisplayTime, int _ClientID )
    {
        //  サーバーでは処理を行わない
        if( NetworkServer.active )                          return;
        //  指定されたプレイヤー以外は無視する
        if( m_rLinkManager.m_LocalPlayerID != _ClientID )   return;
        
        //  レコードを通知
        m_rAcqRecord.SetRecord( _Record, _DisplayTime, AcqRecord_Control.ColorType.Default );
    }
    [ ClientRpc ]
    public  void    RpcRecordNoticeDRed( string _Record, float _DisplayTime, int _ClientID )
    {
        //  サーバーでは処理を行わない
        if( NetworkServer.active )                          return;
        //  指定されたプレイヤー以外は無視する
        if( m_rLinkManager.m_LocalPlayerID != _ClientID )   return;
        
        //  レコードを通知
        m_rAcqRecord.SetRecord( _Record, _DisplayTime, AcqRecord_Control.ColorType.Emergency );
    }
    [ ClientRpc ]
    public  void    RpcRecordNotice_ToOther( string _Record, int _ExclusionID )
    {
        //  指定されたプレイヤーは無視する
        if( m_rLinkManager.m_LocalPlayerID == _ExclusionID )    return;
        
        //  レコードを通知
        m_rAcqRecord.SetRecord( _Record, 2.6f, AcqRecord_Control.ColorType.Default );
    }
    [ ClientRpc ]
    public  void    RpcRecordNoticeE_ToOther( string _Record, int _ExclusionID )
    {
        //  指定されたプレイヤーは無視する
        if( m_rLinkManager.m_LocalPlayerID == _ExclusionID )    return;

        //  レコードを通知
        m_rAcqRecord.SetRecord( _Record, 2.6f, AcqRecord_Control.ColorType.Emergency );
    }
}
