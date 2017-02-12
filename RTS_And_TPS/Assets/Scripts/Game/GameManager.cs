
using   UnityEngine;
using   UnityEngine.UI;
using   UnityEngine.Networking;
using   UnityEngine.SceneManagement;
using   System.Collections;
using   System.Collections.Generic;

public class GameManager : NetworkBehaviour {
    //  難易度
    public  enum    GameDifficulty{
        Easy,       //  簡単
        Normal,     //  普通
        Hard,       //  高難易度
        DeathMarch  //  最高難易度
    }
          
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
    public  struct  HighScoreData{
        public  int     wave;
        public  int     score;
    }

    //  公開パラメータ
    public  string                  c_RetryScene    =   "";
    public  string                  c_QuitScene     =   "";
    public  Font                    c_Font          =   null;
    public  GameObject              c_StageDrum     =   null;

    //  固定パラメータ
    public  float                   c_StartCDTime   =   3.0f;
    private float                   c_WaveInterval  =   36.6f;
    public  float                   c_StartResource =   150.0f;

    //  内部パラメータ
    [ SyncVar ]
    private GameDifficulty          m_Difficulty    =   GameDifficulty.Normal;    
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
    [ SyncVar ]
    public  bool                    m_IsMulti       =   false;

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
    private SyncListFloat           m_rIncomeList   =   new SyncListFloat();
    private SyncListFloat           m_rConsumList   =   new SyncListFloat();

    private SyncListFloat           m_rResourceList =   new SyncListFloat();

    //  外部へのアクセス
    private LinkManager             m_rLinkManager  =   null;
    private WaveManager             m_rWaveManager  =   null;
    private SkyManager              m_rSkyManager   =   null;
    private Transform               m_rResources    =   null;

    private Text                    m_rWaveText     =   null;
    private Text                    m_rResourceText =   null;
    private Text                    m_rResourceTextOutline =   null;
    private Text                    m_rScoreText    =   null;
    private Text                    m_rDifficultyText    =   null;
    private AcqScore_Control        m_rAcqScore     =   null;
    private AcqRecord_Control       m_rAcqRecord    =   null;
    private AcqScore_Control        m_rAcqResource  =   null;
    private AcqScore_Control        m_rAcqResourceM =   null;
    private DamageFilter_Control    m_rDFControl    =   null;

    private StageDrum_Control       m_rSDrumControl =   null;

    //  関連パラメータ
    //private SoundController         m_rHeartSound   =   null;

    //  ハイスコア
    private HighScoreData[]         m_HSDataSingle  =   new HighScoreData[ 4 ];
    private HighScoreData[]         m_HSDataMulti   =   new HighScoreData[ 4 ];
    private bool                    m_NewRecord     =   false;

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
        m_rDifficultyText    =   FunctionManager.GetAccessComponent< Text >( "Canvas/Column_Difficulty/Text" );
        m_rAcqScore     =   FunctionManager.GetAccessComponent< AcqScore_Control >( "Canvas/AcqScore" );
        m_rAcqRecord    =   FunctionManager.GetAccessComponent< AcqRecord_Control >( "Canvas/AcqRecord" );
        m_rAcqResource  =   FunctionManager.GetAccessComponent< AcqScore_Control >( "Canvas/AcqResource" );
        m_rAcqResourceM =   FunctionManager.GetAccessComponent< AcqScore_Control >( "Canvas/AcqResource_Minus" );
        m_rDFControl    =   FunctionManager.GetAccessComponent< DamageFilter_Control >( "Canvas/DamageFilter" );

        //  パラメータを初期化
        m_Resource      =   c_StartResource;

        //  共通の処理
        {
            //  空設定
            m_rSkyManager.ChangeSky( 0 );

            //  BGM再生開始
            BGMManager.ChangeBGM( "BGM_Ready", 0.5f, 0.0f, 0.0f, 0.0f );

            //  ハイスコアをロード
            LoadHighScore();
        }

        //  サーバーでの処理
        if( NetworkServer.active ){
            ReplaceStageDrum();

            //  難易度読み込み
            m_Difficulty    =   ( GameDifficulty )PlayerPrefs.GetInt( "Difficulty", ( int )GameDifficulty.Normal );

            //  リスト数初期化
            CheckWhetherExist_Bool( m_rIsReadyList, 2 );
            CheckWhetherExist_String( m_rNameList, 2 );
            CheckWhetherExist_Float( m_rScoreList, 2 );
            CheckWhetherExist_Float( m_rDamageList, 2 );
            CheckWhetherExist_Int( m_rKillList, 2 );
            CheckWhetherExist_Int( m_rDeathList, 2 );
            CheckWhetherExist_Int( m_rRivivalList, 2 );
            CheckWhetherExist_Int( m_rHSKillList, 2 );
            CheckWhetherExist_Float( m_rIncomeList, 2 );
            CheckWhetherExist_Float( m_rConsumList, 2 );

            CheckWhetherExist_Float( m_rResourceList, 2 );

            //  リソース初期化
            //for( int i = 0; i < m_rResourceList.Count; i++ ){
            //    m_rResourceList[ i ]    =   c_StartResource;
            //}

            //this.enabled    =   false;
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
        ||  m_State == State.CountDown
        ||  m_State == State.WaveReady )    Time.timeScale  =   m_GameSpeed;
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
            if( m_rResourceText )           m_rResourceText.text            =   ( ( int )m_rResourceList[ m_rLinkManager.m_LocalPlayerID ] ).ToString();
            if( m_rResourceTextOutline )    m_rResourceTextOutline.text     =   ( ( int )m_rResourceList[ m_rLinkManager.m_LocalPlayerID ] ).ToString();
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

        //  記録を削除（開発用コマンド）
        if( Input.GetKey( KeyCode.RightShift )
        &&  Input.GetKeyDown( KeyCode.Alpha0 ) ){
            //  削除
            PlayerPrefs.DeleteAll(); 

            //  通知
            SetAcqRecord( "記録を削除しました！", 2.0f, m_rLinkManager.m_LocalPlayerID, AcqRecord_Control.ColorType.Emergency );
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

        //  マルチフラグ更新
        if( m_IsMulti != m_rWaveManager.m_IsMultiMode ){
            m_IsMulti   =   m_rWaveManager.m_IsMultiMode;
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
        if( m_StateTimer >= 1.7f + 7.5f + 5.0f ){ 
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
        int numClient   =   NetworkManager.singleton.numPlayers;
        
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
                new Rect( -54.0f, -110.0f, 100, 20 )
            );
            if( GUI.Button( rect, "x " + ( int )m_GameSpeed ) ){
                m_rLinkManager.m_rLocalNPControl.CmdChange_GameSpeed( ( m_GameSpeed > 1.0f )? 1.0f : 2.0f );

                //  効果音再生
                SoundController.PlayNow( "UI_FocusChange", 0.0f, 0.05f, 1.0f, 1.0f );
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

        //  難易度選択    
        if( m_State == State.Ready ){
            //  難易度選択   
            GUI.Box( new Rect( 183, 232, 130, 83 ), "Difficulty" );
            {
                string[]    c_DifficultName =   {   "Easy", "Normal",   "Hard", "Death March"   };
                string      difcultStr      =   c_DifficultName[ ( int )m_Difficulty ];

                //  選択されている難易度
                GUI.Box( new Rect( 195, 260, 106, 20 ), "" );
                //  テキスト
                FunctionManager.GUILabel( FunctionManager.AR_TYPE.MIDDLE_CENTER, new Vector2( 195, -260 ), difcultStr, Vector2.one * 0.5f, new Vector2( 106, 20 ) );

                //  難易度を下げる 
                if( m_Difficulty > GameDifficulty.Easy
                &&  GUI.Button( new Rect( 195, 286, 46, 20 ), "<<" ) ){
                    m_rLinkManager.m_rLocalNPControl.CmdChangeDifficult( ( GameDifficulty )Mathf.Max( ( int )m_Difficulty - 1, 0 ) );

                    //  効果音再生
                    SoundController.PlayNow( "UI_FocusChange", 0.0f, 0.05f, 1.0f, 1.0f );
                }
                //  難易度を上げる
                if( m_Difficulty < GameDifficulty.DeathMarch
                &&  GUI.Button( new Rect( 255, 286, 46, 20 ), ">>" ) ){
                    m_rLinkManager.m_rLocalNPControl.CmdChangeDifficult( ( GameDifficulty )Mathf.Min( ( int )m_Difficulty + 1, ( int )GameDifficulty.DeathMarch ) );

                    //  効果音再生
                    SoundController.PlayNow( "UI_FocusChange", 0.0f, 0.05f, 1.0f, 1.0f );
                }

                //  表示を更新
                if( m_rDifficultyText )     m_rDifficultyText.text  =   difcultStr;
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
        &&  CheckTimeShift( m_StateTimer, 6.9f, 10.4f ) ){
            PrintMessage( "ゲーム 終了" );
        }
        if( m_State == State.GameOver
        &&  CheckTimeShift( m_StateTimer, 11.4f, 14.9f ) ){
            PrintMessage( "お疲れ様でした" );
        }

        //  ほかのプレイヤーを待っています 
        if( m_State == State.Reuslt
        &&  GetFromList_IsReady( m_rLinkManager.m_LocalPlayerID ) ){
            PrintMessage( "他のプレイヤーを待っています" );
        }

        //  リザルト画面表示 
        if( m_State == State.Reuslt 
        &&  m_StateTimer >= 1.5f
        &&  !GetFromList_IsReady( m_rLinkManager.m_LocalPlayerID ) )
        {
            ResultButton_Input  input;
            input   =   PrintResult( m_StateTimer - 1.7f );

            //  続けるボタンが押された
            if( input.pushOK )      m_rLinkManager.m_rLocalNPControl.CmdSend_GMIsReady( true );
            //  やめるボタンが押された 
            if( input.pushQuit ){
                NetworkManager.singleton.GetComponent< MyNetworkManagerHUD >().Stop();

                NetworkManager.singleton.dontDestroyOnLoad  =   false;
                DestroyImmediate( NetworkManager.singleton.gameObject );
                SceneManager.LoadScene( c_QuitScene );
            }
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
        _Timer  =   10000.0f;

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
        GUI.Box( new Rect( 0.0f, 0.0f, 480.0f, 310.0f ), "" );
        GUI.Box( new Rect( 0.0f, 0.0f, 480.0f, 36.0f ),  "Result" );

        //  結果発表
        {
            float   offsetY     =   0;
            float   offsetY1    =   -6;
            float   offsetY2    =   10;

            GUI.Label( new Rect( 18.0f, offsetY + offsetY1 + 50.0f, 200.0f, 26.0f ), "Mode" ); 
            GUI.Label( new Rect( 18.0f, offsetY + offsetY1 + 76.0f, 200.0f, 26.0f ), "Difficult" );
            GUI.Label( new Rect( 18.0f, offsetY + offsetY2 + 112.0f, 200.0f, 26.0f ), "Wave" );
            GUI.Label( new Rect( 18.0f, offsetY + offsetY2 + 138.0f, 200.0f, 26.0f ), "Score" );

            string[]    difficultName   =   {   "Easy",     "Normal",   "Hard",     "Death March"   };
            string      modeStr         =   ( m_IsMulti )? "Multi" : "Single";
            string      difficultStr    =   difficultName[ ( int )m_Difficulty ];
            string      waveStr         =   m_WaveLevel.ToString();
            string      scoreStr        =   m_GlobalScore.ToString();
             
            FunctionManager.GUILabel( FunctionManager.AR_TYPE.TOP_RIGHT, new Vector2( -324.0f,  -50.0f - offsetY - offsetY1 ), modeStr,      new Vector2( 1.0f, 1.0f ), new Vector2( 480, 310 ) );
            FunctionManager.GUILabel( FunctionManager.AR_TYPE.TOP_RIGHT, new Vector2( -324.0f,  -76.0f - offsetY - offsetY1 ), difficultStr, new Vector2( 1.0f, 1.0f ), new Vector2( 480, 310 ) );
            FunctionManager.GUILabel( FunctionManager.AR_TYPE.TOP_RIGHT, new Vector2( -324.0f, -112.0f - offsetY - offsetY2 ), waveStr,      new Vector2( 1.0f, 1.0f ), new Vector2( 480, 310 ) );
            FunctionManager.GUILabel( FunctionManager.AR_TYPE.TOP_RIGHT, new Vector2( -324.0f, -138.0f - offsetY - offsetY2 ), scoreStr,     new Vector2( 1.0f, 1.0f ), new Vector2( 480, 310 ) );

            //  ハイスコア更新
            {
                GUI.BeginGroup( new Rect( 0, 97, 174, 23 ) );
                    GUI.Box( new Rect( -10, 0, 260, 20 ), "" );
                    if( m_NewRecord ){
                        GUI.Label( new Rect( 13, 0, 160, 26 ), "ハイスコア更新！" );
                    }
                GUI.EndGroup();
            }
        }

        //  線
        GUI.BeginGroup( new Rect( 174.0f, 36.0f, 2.0f, 139.0f ) );
            GUI.Box( new Rect( -1.0f, -5.0f, 12.0f, 149.0f ), "" );
        GUI.EndGroup();

        //  ハイスコア
        {
            //  ラベル
            GUI.Label( new Rect( 295.0f, 37.0f, 100.0f, 26.0f ), "High Score" ); 

            //  線
            GUI.BeginGroup( new Rect( 176.0f, 59.0f, 306, 1.0f ) );
                GUI.Box( new Rect( -1.0f, -6, 308, 12 ), "" );
            GUI.EndGroup();

            //  罫線
            for( int i = 0; i < 4; i++ ){
                //  線
                GUI.BeginGroup( new Rect( 176.0f, 83.0f + i * 23, 306, 1.0f ) );
                    GUI.Box( new Rect( -1.0f, -6, 308, 12 ), "" );
                GUI.EndGroup();
            }

            //  縦線
            GUI.BeginGroup( new Rect( 221.0f, 59.0f, 1.0f, 116.0f ) );
                GUI.Box( new Rect( -1.0f, -5.0f, 12.0f, 149.0f ), "" );
            GUI.EndGroup();

            //  縦線
            GUI.BeginGroup( new Rect( 349.0f, 59.0f, 1.0f, 116.0f ) );
                GUI.Box( new Rect( -1.0f, -5.0f, 12.0f, 149.0f ), "" );
            GUI.EndGroup();

            //  ラベル 
            GUI.Label( new Rect( 186.0f, 62.0f, 100.0f, 24.0f ), "Diffy" );
            GUI.Label( new Rect( 267.0f, 62.0f, 100.0f, 24.0f ), "Single" );
            GUI.Label( new Rect( 400.0f, 62.0f, 100.0f, 24.0f ), "Multi" );

            //  難易度 
            GUI.Label( new Rect( 188, 62 + 23 * 1, 100, 24 ), "DM" );
            GUI.Label( new Rect( 193, 62 + 23 * 2, 100, 24 ), "H" );
            GUI.Label( new Rect( 193, 62 + 23 * 3, 100, 24 ), "N" );
            GUI.Label( new Rect( 193, 62 + 23 * 4, 100, 24 ), "E" );

            //  縦線（スコア）
            GUI.BeginGroup( new Rect( 256.0f, 84.0f, 1.0f, 91.0f ) );
                GUI.Box( new Rect( -1.0f, -5.0f, 12.0f, 149.0f ), "" );
            GUI.EndGroup();

            //  縦線（スコア）
            GUI.BeginGroup( new Rect( 384.0f, 84.0f, 1.0f, 91.0f ) );
                GUI.Box( new Rect( -1.0f, -5.0f, 12.0f, 149.0f ), "" );
            GUI.EndGroup();

            //  スコアを表示（シングル）
            for( int i = 0; i < 4; i++ ){
                //  ピックアップ  
                if( m_NewRecord
                &&  m_rWaveManager.m_IsMultiMode == false
                &&  ( int )m_Difficulty == 3 - i ){
                    int space   =   1;
                    GUI.BeginGroup( new Rect( 222 + space, 84 + space + 23 * i, 34 - space * 2, 22 - space * 2 ), "" );
                        GUI.Box( new Rect( -5, -5, 34 + 12, 22 + 12 ), "" );
                    GUI.EndGroup();

                    GUI.BeginGroup( new Rect( 257 + space, 84 + space + 23 * i, 92 - space * 2, 22 - space * 2 ), "" );
                        GUI.Box( new Rect( -5, -5, 93 + 12, 22 + 12 ), "" );
                    GUI.EndGroup();
                }

                if( m_HSDataSingle[ 3 - i ].wave == 0 ) continue;

                //  ウェーブ数
                FunctionManager.GUILabel( FunctionManager.AR_TYPE.TOP_CENTER, new Vector2( -1.0f, -85.0f - 23 * i ), m_HSDataSingle[ 3 - i ].wave.ToString(),  new Vector2( 0.5f, 1.0f ), new Vector2( 480, 310 ) ); 
                //  スコア
                FunctionManager.GUILabel( FunctionManager.AR_TYPE.TOP_CENTER, new Vector2( 63.0f, -85.0f - 23 * i ), m_HSDataSingle[ 3 - i ].score.ToString(), new Vector2( 0.5f, 1.0f ), new Vector2( 480, 310 ) ); 
            }
            //  スコアを表示（マルチ）
            for( int i = 0; i < 4; i++ ){
                //  ピックアップ  
                if( m_NewRecord
                &&  m_rWaveManager.m_IsMultiMode == true
                &&  ( int )m_Difficulty == 3 - i ){
                    int space   =   1;
                    GUI.BeginGroup( new Rect( 350 + space, 84 + space + 23 * i, 34 - space * 2, 22 - space * 2 ), "" );
                        GUI.Box( new Rect( -5, -5, 34 + 12, 22 + 12 ), "" );
                        //GUI.Box( new Rect( -5, -5, 34 + 12, 22 + 12 ), "" );
                    GUI.EndGroup();

                    GUI.BeginGroup( new Rect( 385 + space, 84 + space + 23 * i, 93 - space * 2, 22 - space * 2 ), "" );
                        GUI.Box( new Rect( -5, -5, 93 + 12, 22 + 12 ), "" );
                        //GUI.Box( new Rect( -5, -5, 93 + 12, 22 + 12 ), "" );
                    GUI.EndGroup();
                }

                if( m_HSDataMulti[ 3 - i ].wave == 0 )  continue;

                //  ウェーブ数
                FunctionManager.GUILabel( FunctionManager.AR_TYPE.TOP_CENTER, new Vector2( 127.0f, -85.0f - 23 * i ), m_HSDataMulti[ 3 - i ].wave.ToString(),  new Vector2( 0.5f, 1.0f ), new Vector2( 480, 310 ) ); 
                //  スコア
                FunctionManager.GUILabel( FunctionManager.AR_TYPE.TOP_CENTER, new Vector2( 191.0f, -85.0f - 23 * i ), m_HSDataMulti[ 3 - i ].score.ToString(), new Vector2( 0.5f, 1.0f ), new Vector2( 480, 310 ) ); 
            }
        }
        
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
                GUI.Box( new Rect( -5.0f, 175.0f, 490, 26 ), "" );
                GUI.BeginGroup( new Rect( -1.0f, 178.0f, 480.0f, 26 ) );

                //GUI.Label( new Rect( 176, 0.0f, 480.0f, 26.0f ), "Score" ); 
                GUI.Label( new Rect( 253, 0.0f, 480.0f, 26.0f ), "Kill" );
                GUI.Label( new Rect( 292, 0.0f, 480.0f, 26.0f ), "Damage" );
                GUI.Label( new Rect( 351, 0.0f, 480.0f, 26.0f ), "Resource" );
                GUI.Label( new Rect( 418, 0.0f, 480.0f, 26.0f ), "Consum" );

                GUI.EndGroup();
            }

            //  表示
            for( int i = 0; i < 2; i++ ){

                int     playerID    =   rSortList[ i ].id;
                bool    isActiveID  =   m_rLinkManager.CheckActiveClient( playerID );
                //if( !CheckActiveClient( playerID ) )                                    continue;

                //  自分の項目をピックアップ
                //if( playerID == m_rLinkManager.m_LocalPlayerID ){
                //    GUI.Box( new Rect( 0.0f, 207.0f + 26.0f * i, 480.0f, 26.0f ), "" );
                //}

                GUI.BeginGroup( new Rect( -1.0f, 212.0f + 31.0f * i, 480.0f, 26 ) );

                GUI.Label( new Rect( 26.0f, 0.0f, 480.0f, 26.0f ), rankSymbol[ i ] );

                //  スコアを表示   
                {
                    GUIStyle    fontStyle   =   new GUIStyle( GUI.skin.label );
                    fontStyle.alignment     =   TextAnchor.MiddleRight;

                    string      content     =   ( m_rScoreList.Count > playerID )? ( ( int )m_rScoreList[ playerID ] ).ToString().PadLeft( 7, '_' ) : "_______";
                    Vector2     contentSize =   fontStyle.CalcSize( new GUIContent( content ) );

                    GUI.color           =   Color.white;
                    GUI.backgroundColor =   Color.white;

                    GUI.Label( new Rect( 217.0f - contentSize.x, 0.0f, contentSize.x, contentSize.y ), content, fontStyle );
                }

                GUI.Label( new Rect( 252.0f, 0.0f, 480.0f, 26.0f ), ( isActiveID )?( ( m_rKillList.Count  > playerID )?   m_rKillList[ playerID ].ToString().PadLeft( 3, '_' ) : "__0" )    : "___" );
                GUI.Label( new Rect( 302.0f, 0.0f, 480.0f, 26.0f ), ( isActiveID )?( ( m_rDamageList.Count > playerID )?  ( ( int )m_rDamageList[ playerID ] ).ToString().PadLeft( 4, '_' ) :  "___0" ) :  "____" );
                GUI.Label( new Rect( 360.0f, 0.0f, 480.0f, 26.0f ), ( isActiveID )?( ( m_rIncomeList.Count > playerID )?  ( ( int )m_rIncomeList[ playerID ] ).ToString().PadLeft( 5, '_' ) : "____0" ) : "_____" );
                GUI.Label( new Rect( 425.0f, 0.0f, 480.0f, 26.0f ), ( isActiveID )?( ( m_rConsumList.Count > playerID )?  ( ( int )m_rConsumList[ playerID ] ).ToString().PadLeft( 5, '_' ) : "____0" ) : "_____" );
                if( isActiveID )    GUI.Label( new Rect( 64.0f, 0.0f, 480.0f, 26.0f ), playerName[ playerID ] );
                else                GUI.Label( new Rect( 64.0f, 0.0f, 480.0f, 26.0f ), "________" );

                GUI.EndGroup();
            }
        }

        //  フレーム表示    
        GUI.Box( new Rect( 0.0f, 274, 480.0f, 36.0f ), "" );

        //  ボタン表示
        if( _Timer - 0.5f > 5.5f + 2.0f ){
            inputData.pushOK    =   GUI.Button( new Rect( 100, 280, 100.0f, 24.0f ), "続ける" );
            inputData.pushQuit  =   GUI.Button( new Rect( 280, 280, 100.0f, 24.0f ), "やめる" );
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

    //  ハイスコア関係
    void    LoadHighScore()
    {
        string[]    c_ModeStr   =   {   "Single",   "Multi"     };
        string[]    c_DiffyStr  =   {   "E",    "N",    "H",    "DM"    };
        for( int m = 0; m < 2; m++ ){
            string  modeStr     =   c_ModeStr[ m ];
            for( int d = 0; d < 4; d++ ){
                string  diffyStr    =   c_DiffyStr[ d ];

                if( m == 0 ){
                    m_HSDataSingle[ d ].wave    =   PlayerPrefs.GetInt( modeStr + "_" + diffyStr +  "_Wave", 0 );
                    m_HSDataSingle[ d ].score   =   PlayerPrefs.GetInt( modeStr + "_" + diffyStr + "_Score", 0 );
                }
                else{
                    m_HSDataMulti[ d ].wave     =   PlayerPrefs.GetInt( modeStr + "_" + diffyStr +  "_Wave", 0 );
                    m_HSDataMulti[ d ].score    =   PlayerPrefs.GetInt( modeStr + "_" + diffyStr + "_Score", 0 );
                }
            }
        }
    }
    void    SaveHighScore()
    {
        string[]    c_ModeStr   =   {   "Single",   "Multi"     };
        string[]    c_DiffyStr  =   {   "E",    "N",    "H",    "DM"    };
        for( int m = 0; m < 2; m++ ){
            string  modeStr     =   c_ModeStr[ m ];
            for( int d = 0; d < 4; d++ ){
                string  diffyStr    =   c_DiffyStr[ d ];

                if( m == 0 ){
                    PlayerPrefs.SetInt( modeStr + "_" + diffyStr +  "_Wave", m_HSDataSingle[ d ].wave );
                    PlayerPrefs.SetInt( modeStr + "_" + diffyStr + "_Score", m_HSDataSingle[ d ].score );
                }
                else{
                    PlayerPrefs.SetInt( modeStr + "_" + diffyStr +  "_Wave", m_HSDataMulti[ d ].wave );
                    PlayerPrefs.SetInt( modeStr + "_" + diffyStr + "_Score", m_HSDataMulti[ d ].score );
                }
            }
        }
    }

    bool    UpdateHighScore( GameDifficulty _Difficulty, bool _IsMulti, int _Wave, int _Score )
    {
        if( _IsMulti ){
            if( _Score >= m_HSDataMulti[ ( int )_Difficulty ].score ){
                m_HSDataMulti[ ( int )_Difficulty ].score   =   _Score;
                m_HSDataMulti[ ( int )_Difficulty ].wave    =   _Wave;

                return  true;
            }
        }
        else{
            if( _Score >= m_HSDataSingle[ ( int )_Difficulty ].score ){
                m_HSDataSingle[ ( int )_Difficulty ].score  =   _Score;
                m_HSDataSingle[ ( int )_Difficulty ].wave   =   _Wave;

                return  true;
            }
        }

        return  false;
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
        if( m_State > State.InGame )    return;

        //  項目がなければ拡張する
        CheckWhetherExist_Float( m_rScoreList, _ClientID + 1 );

        //  値を設定
        m_rScoreList[ _ClientID ]   +=  _AddScore;
    }
    public  void    SetToList_Damage( int _ClientID, float _AddDamage )
    {
        if( m_State > State.InGame )    return;

        //  項目がなければ拡張する
        CheckWhetherExist_Float( m_rDamageList, _ClientID + 1 );

        //  値を設定
        m_rDamageList[ _ClientID ]  +=  _AddDamage;
    }
    public  void    SetToList_Kill( int _ClientID, int _AddValue )
    {
        if( m_State > State.InGame )    return;

        //  項目がなければ拡張する
        CheckWhetherExist_Int( m_rKillList, _ClientID + 1 );

        //  値を設定
        m_rKillList[ _ClientID ]    +=  _AddValue;
    }
    public  void    SetToList_Death( int _ClientID, int _AddValue )
    {
        if( m_State > State.InGame )    return;

        //  項目がなければ拡張する
        CheckWhetherExist_Int( m_rDeathList, _ClientID + 1 );

        //  値を設定
        m_rDeathList[ _ClientID ]   +=  _AddValue;
    }
    public  void    SetToList_Rivival( int _ClientID, int _AddValue )
    {
        if( m_State > State.InGame )    return;

        //  項目がなければ拡張する
        CheckWhetherExist_Int( m_rRivivalList, _ClientID + 1 );

        //  値を設定
        m_rRivivalList[ _ClientID ] +=  _AddValue;
    }
    public  void    SetToList_HSKill( int _ClientID, int _AddValue )
    {
        if( m_State > State.InGame )    return;

        //  項目がなければ拡張する
        CheckWhetherExist_Int( m_rHSKillList, _ClientID + 1 );

        //  値を設定
        m_rHSKillList[ _ClientID ]  +=  _AddValue;
    }
    public  void    SetToList_Income( int _ClientID, float _AddValue )
    {
        if( m_State > State.InGame )    return;

        //  項目がなければ拡張する
        CheckWhetherExist_Float( m_rIncomeList, _ClientID + 1 );

        //  値を設定
        m_rIncomeList[ _ClientID ]  +=  _AddValue;
    }
    public  void    SetToList_Consum( int _ClientID, float _AddValue )
    {
        if( m_State > State.InGame )    return;

        //  項目がなければ拡張する
        CheckWhetherExist_Float( m_rConsumList, _ClientID + 1 );

        //  値を設定
        m_rConsumList[ _ClientID ]  +=  _AddValue;
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
    public  float   GetFromList_Income( int _ClientID )
    {
        return  m_rIncomeList[ _ClientID ];
    }
    public  float   GetFromList_Consum( int _ClientID )
    {
        return  m_rConsumList[ _ClientID ];
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
    public  int     GetNumItem_Income()
    {
        return  m_rIncomeList.Count;
    }
    public  int     GetNumItem_Consum()
    {
        return  m_rConsumList.Count;
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

        //  女の子がいたらMPを回復
        GameObject      rMyPlayer   =   m_rLinkManager.m_rLocalPlayer;
        GirlController  rMyGirl     =   rMyPlayer.GetComponent< GirlController >();
        if( rMyGirl ){
            rMyGirl.ResetMP();
        }
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

        //  ゲームスピードをリセット
        m_GameSpeed     =   1.0f;
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

        //  ハイスコアチェック
        m_NewRecord     =   UpdateHighScore( m_Difficulty, m_IsMulti, m_WaveLevel, ( int )m_GlobalScore );
        if( m_NewRecord ){
            //  ハイスコアを保存
            SaveHighScore();
        }

        //  終了
        //SoundController.PlayNow( "Voice_G2D_Result", 16.0f, 0.1f, 1.0f, 10.0f ); 
        if( !m_NewRecord )  SoundController.PlayNow( "Voice_G2D_Result_" + Random.Range( 0, 2 ), 16.4f, 0.1f, 1.0f, 10.0f );
        else                SoundController.PlayNow( "Voice_G2D_Result_" + Random.Range( 2, 4 ), 16.4f, 0.1f, 1.0f, 10.0f );
    }
    void    StandbyProc_GameOverInServer()
    {

    }

    //  外部からの操作    
    public  void    StartCountDown()
    {
        if( m_State != State.Ready )    return;

        ChangeState( State.CountDown );

        //  難易度に応じて開始資源を変更
        {
            float[] c_ResourceRatio =   {
                1.0f,   1.0f,   2.0f,   4.0f
            };
            float   resourceRate    =   c_ResourceRatio[ ( int )m_Difficulty ];

            m_Resource  =   c_StartResource * resourceRate;

            for( int i = 0; i < m_rResourceList.Count; i++ ){
                m_rResourceList[ i ]    =   c_StartResource * resourceRate;
            }
        }

        //  難易度を保存
        PlayerPrefs.SetInt( "Difficulty", ( int )m_Difficulty );
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
    public  void    SetAcqResource_Minus( float _MinusResource )
    {
        m_rAcqResourceM.SetAddScore( _MinusResource ); 
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
    [ Server ]
    public  void            AddResource( float _AddValue, int _ClientID )
    {
        if( m_State > State.InGame )    return;

        if( _AddValue < 0 ){
            m_rResourceList[ _ClientID ]    +=  _AddValue;
            m_rResourceList[ _ClientID ]    =   Mathf.Max( m_rResourceList[ _ClientID ], 0.0f );
        }
        else{
            for( int i = 0; i < m_rResourceList.Count; i++ ){
                m_rResourceList[ i ]    +=  _AddValue;
                m_rResourceList[ i ]    =   Mathf.Max( m_rResourceList[ i ], 0.0f );
            }
        }
    }
    [ Server ]
    public  void            AddGlobalScore( float _AddScore, int _ClientID )
    {
        if( m_State > State.InGame )    return;

        m_GlobalScore   +=  _AddScore;

        //  個人スコア加算
        SetToList_Score( _ClientID, _AddScore );
    }
    [ Server ]
    public  void            SetDifficulty( GameDifficulty _Difficulty )
    {
        m_Difficulty    =   _Difficulty;
    }
    [ Server ]
    public  void            OnConnectedNewPlayer( int _ConnectID )
    {
        RpcRecordNotice_ToOther( "新たなプレイヤーが参戦しました", _ConnectID );  
        RpcRecordNotice( "ゲームに参加しました", _ConnectID );
    }
    [ Server ]
    public  void            OnDisconnectedPlayer( int _ConnectID )
    {
        RpcRecordNoticeE_ToOther( GetFromList_PlayerName( _ConnectID ) + "  が離脱しました", _ConnectID );  
    }

    //  取得
    public  State           GetState(){
        return  m_State;
    }
    public  float           GetGameSpeed()
    {
        return  m_GameSpeed;
    }
    public  float           GetResource()
    {
        return  m_rResourceList[ m_rLinkManager.m_LocalPlayerID ];
    }
    public  float           GetGlobalScore()
    {
        return  m_GlobalScore;
    }
    public  GameDifficulty  GetDifficulty()
    {
        return  m_Difficulty;
    }
//=========================================================================================
//      中継
//=========================================================================================
    private GirlController  GetMyGirlControl()
    {
        GameObject      rLocalPlayer    =   m_rLinkManager.m_rLocalPlayer;
        if( !rLocalPlayer )     return  null;

        GirlController  rGirlControl    =   rLocalPlayer.GetComponent< GirlController >();
        if( !rGirlControl )     return  null;

        return  rGirlControl;
    }
//----------------------------------------------------------------------------------------- 
    public  void            CallToMyGirl_RideToRobo()
    {
        GirlController  rGirlControl    =   GetMyGirlControl();
        if( !rGirlControl )     return;

        //  呼び出し
        rGirlControl.RideToRobo();
    }
    public  void            CallToMyGirl_GetOffToRobo()
    {
        GirlController  rGirlControl    =   GetMyGirlControl();
        if( !rGirlControl )     return;

        //  呼び出し
        rGirlControl.GetOutOfTheRobo();
    }

    public  void            CallToMyGirl_PlaceDrum()
    {
        GirlController  rGirlControl    =   GetMyGirlControl();
        if( !rGirlControl )     return;

        //  呼び出し
        rGirlControl.PlaceDrum();
    }
    public  void            CallToMyGirl_PlaceTimeBomb()
    {
        GirlController  rGirlControl    =   GetMyGirlControl();
        if( !rGirlControl )     return;

        //  呼び出し
        rGirlControl.PlaceTimeBomb();
    }
    public  void            CallToMyGirl_PlaceC4()
    {
        GirlController  rGirlControl    =   GetMyGirlControl();
        if( !rGirlControl )     return;

        //  呼び出し
        rGirlControl.PlaceC4();
    }
    public  void            CallToMyGirl_ExplodingC4()
    {
        GirlController  rGirlControl    =   GetMyGirlControl();
        if( !rGirlControl )     return;

        //  呼び出し
        rGirlControl.ExplodingC4();
    }

    public  void            CallToMyGirl_PlaceOK()
    {
        GirlController  rGirlControl    =   GetMyGirlControl();
        if( !rGirlControl )     return;

        //  呼び出し
        rGirlControl.PlaceOK();
    }

//=========================================================================================
//      サウンド
//=========================================================================================
    public  void            PlaySound_UI_Select( float _Volume )
    {
        SoundController.PlayNow( "UI_FocusChange", 0.0f, _Volume, 1.0f, 1.0f );
    }
    public  void            PlaySound_UI_Determ( float _Volume )
    {
        SoundController.PlayNow( "UI_Click", 0.0f, _Volume, 1.0f, 1.0f );
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
