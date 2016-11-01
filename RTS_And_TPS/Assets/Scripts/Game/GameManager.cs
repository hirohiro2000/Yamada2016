
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;
using   System.Collections.Generic;

public class GameManager : NetworkBehaviour {

    //  シーン内の状態
    public  enum    State{
        Ready,      //  準備時間
        CountDown,  //  開始までのカウントダウン中
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
    public  Font                    c_Font          =   null;

    //  固定パラメータ
    private float                   c_CountDownTime =   30.0f - 0.001f;

    //  内部パラメータ
    [ SyncVar ]
    private State                   m_State         =   State.Ready;
    [ SyncVar ]
    private float                   m_StateTimer    =   0.0f;
    [ SyncVar ]
    public  float                   m_GameSpeed     =   1.0f;

    private Queue< MainMessage >    m_MainMessageQ  =   new Queue< MainMessage >();

    //  共有パラメータ
    public  SyncListBool            m_rIsReadyList  =   new SyncListBool();

    //  外部へのアクセス
    private LinkManager             m_rLinkManager  =   null;

	// Use this for initialization
	void    Start()
    {
        //  パラメータ初期化
        m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );
	}
	
	// Update is called once per frame
	void    Update()
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

        //  サーバー側の処理
        if( isServer )  UpdateIn_Server();
        //  クライアント側の処理
        else            UpdateIn_Client();
	}
    void    UpdateIn_Server()
    {
        //  リストの項目数を調整
        UpdateListItem();

        //  状態に応じて処理を行う
	    switch( m_State ){
            case    State.Ready:        Update_Ready();     break;
            case    State.CountDown:    Update_CountDown(); break;
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
        if( m_StateTimer >= c_CountDownTime ){
            //  ゲーム開始
            ChangeState( State.InGame );
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
            NetworkManager.singleton.ServerChangeScene( "NetworkTPS" );
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
        if( m_State == State.CountDown ){
            PrintMessage( "ゲーム開始まであと  " + ( int )( c_CountDownTime - m_StateTimer + 1 ) +  "  秒" );
        }
        //  ゲームが開始されました
        if( m_State == State.InGame
        &&  CheckTimeShift( m_StateTimer, 1.2f, 1.2f + 5.0f ) ){
            PrintMessage( "敵が拠点への侵攻を開始しました" );
        }

        //  拠点が破壊されました
        if( m_State == State.GameOver
        &&  CheckTimeShift( m_StateTimer, 1.7f, 1.7f + 3.7f ) ){
            PrintMessage( "拠点が制圧されました" );
        }
        //  ゲーム終了
        if( m_State == State.GameOver
        &&  CheckTimeShift( m_StateTimer, 5.2f + 1.7f, 5.2f + 1.2f + 3.2f ) ){
            PrintMessage( "ゲーム 終了" );
        }

        //  リザルト画面表示
        if( m_State == State.Reuslt
        &&  m_StateTimer >= 1.7f
        &&  !GetIsReady( m_rLinkManager.m_LocalPlayerID ) ){
            ResultButton_Input  input;
            input   =   PrintResult( m_StateTimer - 1.7f );

            //  続けるボタンが押された
            if( input.pushOK )  m_rLinkManager.m_rLocalNPControl.CmdSend_GMIsReady( true );
            //  やめるボタンが押された
            if( input.pushQuit ){}
        }

        //  ほかのプレイヤーを待っています 
        if( m_State == State.Reuslt
        &&  GetIsReady( m_rLinkManager.m_LocalPlayerID ) ){
            PrintMessage( "他のプレイヤーを待っています" );
        }
    }

    //  メッセージを表示
    void    PrintMessage( string _Content )
    {
        GUIStyle    countStyle  =   new GUIStyle( GUI.skin.label );
        countStyle.fontSize     =   15;
        countStyle.fontStyle    =   FontStyle.Normal;
        countStyle.font         =   c_Font;

        Vector2     contentSize =   countStyle.CalcSize( new GUIContent( _Content ) );

        //  フレーム表示
        {
            Rect    rect    =   FunctionManager.AdjustRectCanvasToGUI(
                FunctionManager.AR_TYPE.TOP_CENTER,
                new Rect( 0.0f, -160.0f, contentSize.x + 36.0f, 36.0f )
            );
            GUI.Box( rect, "" );
        }

        //  内容を表示
        {
            Rect    rect    =   FunctionManager.AdjustRectCanvasToGUI(
                FunctionManager.AR_TYPE.TOP_CENTER,
                new Rect( 0.0f, -161.0f, contentSize.x, contentSize.y )
            );

            GUI.color           =   Color.white;
            GUI.backgroundColor =   Color.white;

            GUI.Label( rect, _Content, countStyle );
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

            //  順位を決定
            //List< ForSort > rSortList   =   new List< ForSort >();
            //for( int i = 0; i < 8; i++ ){
            //    int score   =   ( int )m_rScoreList[ i ];
            //    if( !m_rLinkManager.CheckActiveClient( i ) )    score   =   -1000000;
            //    rSortList.Add( new ForSort( i, score ) );
            //}
            //rSortList.Sort( ( a, b ) => b.score - a.score );

            //  項目の種類
            {
                GUI.BeginGroup( new Rect( -1.0f, 28.0f, 480.0f, 26 ) );

                //GUI.Label( new Rect( 174, 0.0f, 480.0f, 26.0f ), "Score" );
                GUI.Label( new Rect( 264, 0.0f, 480.0f, 26.0f ), "Kill" );
                GUI.Label( new Rect( 311, 0.0f, 480.0f, 26.0f ), "Death" );
                GUI.Label( new Rect( 361, 0.0f, 480.0f, 26.0f ), "Capture" );
                GUI.Label( new Rect( 423, 0.0f, 480.0f, 26.0f ), "Object" );

                GUI.EndGroup();
            }

            //  表示
            float   interval_0  =   0.5f;//0.4f;
            float   interval_1  =   1.0f;//0.8f;
            for( int i = 0; i < 8; i++ ){
                if( i <  3 && _Timer - 0.5f < interval_0 * 5 + interval_1 * ( 3 - i ) )  continue;
                if( i >= 3 && _Timer - 0.5f < interval_0 * ( 8 - i ) )                   continue;

                int     playerID    =   i;//rSortList[ i ].playerID;
                bool    isActiveID  =   m_rLinkManager.CheckActiveClient( playerID );
                //if( !CheckActiveClient( playerID ) )                                    continue;

                //  自分の項目をピックアップ
                if( playerID == m_rLinkManager.m_LocalPlayerID ){
                    GUI.Box( new Rect( 0.0f, 50.0f + 26.0f * i, 480.0f, 26.0f ), "" );
                }

                GUI.BeginGroup( new Rect( -1.0f, 52.0f + 26.0f * i, 480.0f, 26 ) );

                GUI.Label( new Rect( 26.0f, 0.0f, 480.0f, 26.0f ), rankSymbol[ i ] );

                {
                    GUIStyle    fontStyle   =   new GUIStyle( GUI.skin.label );
        
                    fontStyle.alignment     =   TextAnchor.MiddleRight;

                    string      content     =   ( isActiveID )? ( ( int )0 ).ToString() : "_______";
                    Vector2     contentSize =   fontStyle.CalcSize( new GUIContent( content ) );

                    GUI.color           =   Color.white;
                    GUI.backgroundColor =   Color.white;

                    GUI.Label( new Rect( 217.0f - contentSize.x, 0.0f, contentSize.x, contentSize.y ), content, fontStyle );
                }

                //GUI.Label( new Rect( 168.0f, 0.0f, 480.0f, 26.0f ), ( isActiveID )? ( ( int ) m_rScoreList[ playerID ] ).ToString().PadLeft( 7, '_' ) : "_______" );

                GUI.Label( new Rect( 262.0f, 0.0f, 480.0f, 26.0f ), ( isActiveID )? ( 0 ).ToString().PadLeft( 3, '_' ) : "___" );
                GUI.Label( new Rect( 318.0f, 0.0f, 480.0f, 26.0f ), ( isActiveID )? ( 0 ).ToString().PadLeft( 3, '_' ) : "___" );
                GUI.Label( new Rect( 374.0f, 0.0f, 480.0f, 26.0f ), ( isActiveID )? ( 0 ).ToString().PadLeft( 3, '_' ) : "___" );
                GUI.Label( new Rect( 432.0f, 0.0f, 480.0f, 26.0f ), ( isActiveID )? ( 0 ).ToString().PadLeft( 3, '_' ) : "___" );
                if( isActiveID )    GUI.Label( new Rect( 64.0f, 0.0f, 480.0f, 26.0f ), playerName[ i ] );
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
    public  void    SetIsReady( int _ClientID, bool _IsReady )
    {
        //  項目がなければ拡張する
        CheckWhetherExist_Bool( m_rIsReadyList, _ClientID );

        //  値を設定
        m_rIsReadyList[ _ClientID ] =   _IsReady;
    }
    public  bool    GetIsReady( int _ClientID )
    {
        //  項目がなければ拡張する
        CheckWhetherExist_Bool( m_rIsReadyList, _ClientID );

        //  結果を返す
        return  m_rIsReadyList[ _ClientID ];
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

    //  外部からの操作
    public  void    StartCountDown()
    {
        if( m_State != State.Ready )    return;

        ChangeState( State.CountDown );
    }
    public  void    GameOver()
    {
        if( m_State != State.InGame )   return;

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

    //  アクセス
    public  State   GetState(){
        return  m_State;
    }
    public  float   GetGameSpeed()
    {
        return  m_GameSpeed;
    }
//=========================================================================================
//      リクエスト
//=========================================================================================
    //  メッセージ共有
    [ ClientRpc ]
    public  void    RpcMainMessage( string _Message, float _DisplayTime, float _Delay )
    {
        SetMainMassage( _Message, _DisplayTime, _Delay );
    }
}
