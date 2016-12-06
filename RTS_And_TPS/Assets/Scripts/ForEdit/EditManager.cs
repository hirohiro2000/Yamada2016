
using   System;
using   System.IO;
using   System.Collections;
using   System.Collections.Generic;
using   UnityEngine;
using   UnityEngine.Networking;

#if     UNITY_EDITOR
using   UnityEditor;
#endif

public class EditManager : MonoBehaviour {

    //  編集モード
    public  enum    EditMode{
        Wall,           //  壁を編集
        Base,           //  拠点を編集
        SpawnPoint,     //  エネミーの巣を編集
        LaunchPoint,    //  出撃地点を編集
        Empty,          //  空
        //NavMesh,        //  ナビメッシュ編集
        Select,     //  選択
    }

    //  カメラ制御
    class   EditCamera
    {
        public  EditManager m_rParent       =   null;
        public  float       m_OlthoMax      =   18.78261f;
        public  float       m_OlthoMin      =   18.78261f;

        private Vector3     m_PrevWorld     =   Vector3.zero;

        private Camera      m_rDragCamera   =   null;

        public  EditCamera( EditManager _rEditManager )
        {
            m_rParent       =   _rEditManager;
            m_rDragCamera   =   FunctionManager.GetAccessComponent< Camera >( "Drag Camera" );
        }

        //  更新
        public  void    Update( Camera _rCamera ){
            //  ズームイン、ズームアウト
            Update_Zoom( _rCamera );
            //  スライド
            Update_Move( _rCamera );
        }
        //  ズーム
        private void    Update_Zoom( Camera _rCamera ){
            float   wheelInput  =   -Input.GetAxis( "Mouse ScrollWheel" );
            float   zoomRatio   =   10.0f;
            
            if( Mathf.Abs( wheelInput ) > 0.01f ){
                _rCamera.orthographicSize   +=  wheelInput * zoomRatio;
                _rCamera.orthographicSize   =   Math.Min( _rCamera.orthographicSize, m_OlthoMax );
                _rCamera.orthographicSize   =   Math.Max( _rCamera.orthographicSize, m_OlthoMin );

                UpdateDragCamera( _rCamera );
                UpdatePrevWorld();
            }
        }
        //  移動
        private void    Update_Move( Camera _rCamera ){
            //  スライド開始
            if( Input.GetMouseButtonDown( 2 ) ){
                UpdateDragCamera( _rCamera );
                UpdatePrevWorld();
            }
            //  スライド中
            if( Input.GetMouseButton( 2 ) ){
                //  ワールド基準のカーソル変位量を計算
                Vector3 curWorld        =   m_rDragCamera.ScreenToWorldPoint( Input.mousePosition );
                Vector3 vMoveAtWorld    =   curWorld - m_PrevWorld;
            
                //  変位量反映
                _rCamera.transform.position -=  vMoveAtWorld;

                //  現在の座標を保存
                m_PrevWorld     =   curWorld;
            }

            //  座標制限
            LimitPosition( _rCamera );
        }

        //  座標制限
        private void    LimitPosition( Camera _rCamera ){
            float       wholeWidth  =   m_rParent.m_MapWidth  * m_rParent.c_GridWidth;
            float       wholeHeight =   m_rParent.m_MapHeight * m_rParent.c_GridWidth;

            Transform   rTrans      =   _rCamera.transform;
            float       olthoRatio  =   ( m_OlthoMax - _rCamera.orthographicSize ) / ( m_OlthoMax - m_OlthoMin );
            float       limitRatio  =   0.5f * olthoRatio;
            if( m_OlthoMax == m_OlthoMin ){
                limitRatio  =   0.0f;
            }

            //  範囲内に制限
            Vector3     camPos      =   rTrans.position;
            camPos.x    =   Mathf.Clamp( camPos.x, wholeWidth  * -limitRatio, wholeWidth  * limitRatio );
            camPos.z    =   Mathf.Clamp( camPos.z, wholeHeight * -limitRatio, wholeHeight * limitRatio );

            //  反映
            rTrans.position =   camPos;
        }
        //  ドラッグ用カメラ更新
        private void    UpdateDragCamera( Camera _rCamera ){
            m_rDragCamera.transform.position    =   _rCamera.transform.position;
            m_rDragCamera.orthographicSize      =   _rCamera.orthographicSize;
        }
        //  ドラッグ用座標を更新
        private void    UpdatePrevWorld(){
            m_PrevWorld =   m_rDragCamera.ScreenToWorldPoint( Input.mousePosition );
        }
    }

    //  ウィンドウ統括ウィンドウ
    class   WindowOversee
    {
        private EditManager         m_rParent       =   null;

        private Rect                m_WindowRect    =   new Rect();

        public  bool                m_UseOutput     =   false;
        public  bool                m_UseMapConfig  =   false;
        public  bool                m_UseToolBox    =   false;
        public  bool                m_UseInfo       =   false;
        public  bool                m_UseBox        =   false;
        public  bool                m_UseCamera     =   false;
        public  bool                m_UseTest       =   false;
        public  bool                m_UseSetting    =   false;
        public  bool                m_UseLog        =   false;
		public	bool		m_UseGameWorldParameter	=	false;

		private Window_ToolBox      m_rToolBox      =   null;
        private Window_MapConfig    m_rMapConfig    =   null;
        private Window_FileMenu     m_rFileMenu     =   null;
        private Window_Info         m_rInfo         =   null;
        private Window_Block        m_rBlock        =   null;
		private Window_GameWorldParameter m_rGameWorldParameter = null;

		public  WindowOversee( EditManager _rEditManager )
        {
            m_rParent       =   _rEditManager;
            m_WindowRect    =   FunctionManager.AdjustRectCanvasToGUI(
                FunctionManager.AR_TYPE.BOTTOM_LEFT,
                new Rect( 10, 50, 140, 200 ),
                new Vector2( 0, 0 )
            );

            m_rToolBox      =   new Window_ToolBox( _rEditManager, this );
            m_rMapConfig    =   new Window_MapConfig( _rEditManager, this );
            m_rFileMenu     =   new Window_FileMenu( _rEditManager, this );
            m_rInfo         =   new Window_Info( _rEditManager, this );
            m_rBlock        =   new Window_Block( _rEditManager, this );
			m_rGameWorldParameter =	new Window_GameWorldParameter(_rEditManager, this);
		}

        public  void    Update()
        {
            //  自身を更新
            if( m_rParent.m_OpenWindow ){
                float   screenRatio =   Screen.width / 1024.0f;
                m_WindowRect        =   FunctionManager.AdjustRectCanvasToGUI(
                    FunctionManager.AR_TYPE.BOTTOM_LEFT,
                    new Rect( ( int )( 10 * screenRatio ), ( int )( 50 * screenRatio ), 190, 230 ),
                    new Vector2( 0, 0 )
                );
                GUI.Window( 0, m_WindowRect, WindowProc, "Window" );
            }

            //  ほかのウィンドウを更新
            if( m_UseToolBox )      m_rToolBox.Update();
            if( m_UseMapConfig )    m_rMapConfig.Update();
            if( m_UseOutput )       m_rFileMenu.Update();
            if( m_UseInfo )         m_rInfo.Update();
            if( m_UseBox )          m_rBlock.Update();
			if( m_UseGameWorldParameter ) m_rGameWorldParameter.Update();
		}
        private void    WindowProc( int _WindowID )
        {
            //  閉じる
            if( Input.GetMouseButtonDown( 0 )
            ||  Input.GetMouseButtonDown( 1 ) ){
                float   screenRatio         =   Screen.width / 1024.0f;
                Rect    checkRect           =   m_WindowRect;
                        checkRect.height    +=  32 * screenRatio;
                if( !CheckOverlap( Input.mousePosition, checkRect ) )   m_rParent.m_OpenWindow  =   false;
            }

            //  項目の表示
            {
                float   space   =   22.0f;
                float   offsetY =   24.0f;

                //  出力設定
                if( CheckChangeBool_True( ref m_UseOutput, GUI.Toggle( new Rect( 10, offsetY, 100, 20 ), m_UseOutput, "  File" ) ) ){
                    m_rFileMenu.InitWindowRect();
                }
                offsetY +=  space;

                //  マップ設定
                if( CheckChangeBool_True( ref m_UseMapConfig, GUI.Toggle( new Rect( 10, offsetY, 100, 20 ), m_UseMapConfig, "  Edit" ) ) ){
                    m_rMapConfig.InitWindowRect();
                }
                offsetY +=  space;

                //  ツールボックス
                if( CheckChangeBool_True( ref m_UseToolBox, GUI.Toggle( new Rect( 10, offsetY, 100, 20 ), m_UseToolBox, "  Tool Box" ) ) ){
                    m_rToolBox.InitWindowRect();
                }
                offsetY +=  space;

                //  ボックス
                if( CheckChangeBool_True( ref m_UseBox, GUI.Toggle( new Rect( 10, offsetY, 100, 20 ), m_UseBox, "  Block" ) ) ){
                    m_rBlock.InitWindowRect();
                }
                offsetY +=  space;

                //  情報
                if( CheckChangeBool_True( ref m_UseInfo, GUI.Toggle( new Rect( 10, offsetY, 100, 20 ), m_UseInfo, "  Info" ) ) ){
                    m_rInfo.InitWindowRect();
                }
                offsetY +=  space;

				//  ゲームワールド能力設定
				if (CheckChangeBool_True(ref m_UseGameWorldParameter, GUI.Toggle(new Rect(10, offsetY, 200, 20), m_UseGameWorldParameter, "  GameWorldParameter"))){
					m_rGameWorldParameter.InitWindowRect();
				}
				offsetY += space;

				//  カメラ
				//m_UseCamera     =   GUI.Toggle( new Rect( 10, offsetY, 100, 20 ), m_UseCamera, "  Camera" );
				//offsetY +=  space;

				//  テスト
				m_UseTest       =   GUI.Toggle( new Rect( 10, offsetY, 100, 20 ), m_UseTest, "  Play Test" );
                offsetY +=  space;

                //  設定
                //m_UseSetting    =   GUI.Toggle( new Rect( 10, offsetY, 100, 20 ), m_UseSetting, "  Setting" );
                //offsetY +=  space;

                //  ログ
                m_UseLog        =   GUI.Toggle( new Rect( 10, offsetY, 100, 20 ), m_UseLog, "  Log" );
                offsetY +=  space;
            }
        }
        private bool    CheckChangeBool_True( ref bool _Dest, bool _New )
        {
            bool    oldDest =   _Dest;
            _Dest   =   _New;

            if( oldDest == _New )   return  false;
            if( oldDest )           return  false;

            return  true;
        }

        public  bool    CheckOverlap( Vector2 _ScreenPoint )
        {
            _ScreenPoint    =   FunctionManager.ScreenToGUI( _ScreenPoint );
            return  m_WindowRect.Overlaps( new Rect( _ScreenPoint.x, _ScreenPoint.y, 1, 1 ), true );
        }
        public  bool    CheckOverlap( Vector2 _ScreenPoint, Rect _CheckRect )
        {
            _ScreenPoint    =   FunctionManager.ScreenToGUI( _ScreenPoint );
            return  _CheckRect.Overlaps( new Rect( _ScreenPoint.x, _ScreenPoint.y, 1, 1 ), true );
        }
        public  bool    CheckOverlap_All( Vector2 _ScreenPoint )
        {
            if( m_rParent.m_OpenWindow
            &&  CheckOverlap( _ScreenPoint ) )              return  true;
            if( m_UseToolBox
            &&  m_rToolBox.CheckOverlap( _ScreenPoint ) )   return  true;
            if( m_UseMapConfig
            &&  m_rMapConfig.CheckOverlap( _ScreenPoint ) ) return  true;
            if( m_UseOutput
            &&  m_rFileMenu.CheckOverlap( _ScreenPoint ) )  return  true;
            if( m_UseInfo
            &&  m_rInfo.CheckOverlap( _ScreenPoint ) )      return  true;
            if( m_UseBox
            &&  m_rBlock.CheckOverlap( _ScreenPoint ) )     return  true;

            return  false;
        }
    }
    //  ツールボックス
    class   Window_ToolBox
    {
        private EditManager     m_rParent       =   null;
        private WindowOversee   m_rOversee      =   null;
        private Rect            m_WindowRect    =   new Rect();

        public  Window_ToolBox( EditManager _rParent, WindowOversee _rOversee )
        {
            m_rParent       =   _rParent;
            m_rOversee      =   _rOversee;
            
            InitWindowRect();
        }

        public  void    Update()
        {
            m_WindowRect    =   GUI.Window( 1, m_WindowRect, WindowProc, "Tool Box" );
        }
        private void    WindowProc( int _WindowID )
        {
            //  閉じるボタン
            {
                Rect    rect    =   FunctionManager.AdjustRectCanvasToGUI(
                    FunctionManager.AR_TYPE.TOP_RIGHT,
                    new Rect( -6.0f, -5.0f, 14.0f, 10.0f ),
                    new Vector2( 1.0f, 1.0f ),
                    new Vector2( m_WindowRect.width, m_WindowRect.height )
                );
                if( GUI.Button( rect, "" ) ){
                    m_rOversee.m_UseToolBox =   false;
                }
            }

            //  ドラッグできるようにする
            GUI.DragWindow( new Rect( 0, 0, m_WindowRect.width, 18 ) );

            //  項目の表示
            {
                float   space   =   22.0f;
                float   offsetY =   24.0f;
                
                //  項目名
                {
                    FunctionManager.GUILabel(
                        FunctionManager.AR_TYPE.TOP_CENTER,
                        new Vector2( 0.0f, -offsetY ),
                        "――   ブラシ選択   ――",
                        new Vector2( 0.5f, 1.0f ),
                        new Vector2( m_WindowRect.width, m_WindowRect.height )
                    );
                }
                offsetY +=  space;

                //  項目の表示
                {
                    string[]    buttonText  =   {
                        "壁",
                        "拠点",
                        "敵の巣",
                        "出撃地点",
                        "空",
                        "選択",
                    };
                    Rect        rect        =   FunctionManager.AdjustRectCanvasToGUI(
                        FunctionManager.AR_TYPE.TOP_CENTER,
                        new Rect( 0, -offsetY, 160, 80 ),
                        new Vector2( 0.5f, 1.0f ),
                        new Vector2( m_WindowRect.width, m_WindowRect.height )
                    );
                    m_rParent.m_EditMode
                        =   ( EditMode )GUI.SelectionGrid( rect, ( int )m_rParent.m_EditMode, buttonText, 2 );

                    offsetY +=  rect.height;
                }
                offsetY +=  10;

                //  項目名
                {
                    FunctionManager.GUILabel(
                        FunctionManager.AR_TYPE.TOP_CENTER,
                        new Vector2( 0.0f, -offsetY ),
                        "――   選択メニュー  ――", 
                        new Vector2( 0.5f, 1.0f ),
                        new Vector2( m_WindowRect.width, m_WindowRect.height )
                    );
                }
                offsetY +=  space;

                //  項目を表示
                {
                    Rect    rect    =   FunctionManager.AdjustRectCanvasToGUI(
                        FunctionManager.AR_TYPE.TOP_CENTER,
                        new Rect( 0, -offsetY, 160, 22 ),
                        new Vector2( 0.5f, 1.0f ),
                        new Vector2( m_WindowRect.width, m_WindowRect.height )
                    );
                    if( GUI.Button( rect, "斜面を作成" ) ){
                        m_rParent.SetSlope( m_rParent.m_rFocusList );
                        m_rParent.SetFocus_All( false );
                    }

                    offsetY +=  rect.height;
                }
                offsetY +=  10;

                //  項目名
                {
                    FunctionManager.GUILabel(
                        FunctionManager.AR_TYPE.TOP_CENTER,
                        new Vector2( 0.0f, -offsetY ),
                        "――   まとめて操作   ――",
                        new Vector2( 0.5f, 1.0f ),
                        new Vector2( m_WindowRect.width, m_WindowRect.height )
                    );
                }
                offsetY +=  space;

                //  項目を表示
                {
                    Rect    rect    =   FunctionManager.AdjustRectCanvasToGUI(
                        FunctionManager.AR_TYPE.TOP_CENTER,
                        new Rect( 0, -offsetY, 160, 30 ),
                        new Vector2( 0.5f, 1.0f ),
                        new Vector2( m_WindowRect.width, m_WindowRect.height )
                    );
                    if( GUI.Button( rect, "塗りつぶす" ) ){
                        m_rParent.FillGrid();
                    }

                    offsetY +=  rect.height;
                }
                offsetY +=  5;

                //  項目を表示
                {
                    Rect    rect    =   FunctionManager.AdjustRectCanvasToGUI(
                        FunctionManager.AR_TYPE.TOP_CENTER,
                        new Rect( 0, -offsetY, 160, 20 ),
                        new Vector2( 0.5f, 1.0f ),
                        new Vector2( m_WindowRect.width, m_WindowRect.height )
                    );
                    if( GUI.Button( rect, "クリア" ) ){
                        m_rParent.Erase_All();
                    }

                    offsetY +=  rect.height;
                }
                offsetY +=  10;

                //  項目名
                {
                    FunctionManager.GUILabel(
                        FunctionManager.AR_TYPE.TOP_CENTER,
                        new Vector2( 0.0f, -offsetY ),
                        "――   編集メニュー   ――",
                        new Vector2( 0.5f, 1.0f ),
                        new Vector2( m_WindowRect.width, m_WindowRect.height )
                    );
                }
                offsetY +=  space;

                //  項目を表示
                {
                    Rect    rect    =   FunctionManager.AdjustRectCanvasToGUI(
                        FunctionManager.AR_TYPE.TOP_LEFT,
                        new Rect( 10, -offsetY, 78, 26 ),
                        new Vector2( 0.0f, 1.0f ),
                        new Vector2( m_WindowRect.width, m_WindowRect.height )
                    );
                    GUI.Button( rect, "元に戻す" );
                }
                {
                    Rect    rect    =   FunctionManager.AdjustRectCanvasToGUI(
                        FunctionManager.AR_TYPE.TOP_RIGHT,
                        new Rect( -10, -offsetY, 78, 26 ),
                        new Vector2( 1.0f, 1.0f ),
                        new Vector2( m_WindowRect.width, m_WindowRect.height )
                    );
                    GUI.Button( rect, "やり直す" );

                    offsetY +=  rect.height;
                }
                offsetY +=  5;
            }
        }

        public  void    InitWindowRect()
        {
            m_WindowRect    =   FunctionManager.AdjustRectCanvasToGUI(
                FunctionManager.AR_TYPE.BOTTOM_RIGHT,
                new Rect( -20, 20, 180, 280 + 54 ),
                new Vector2( 1.0f, 0.0f )
            );
        }
        public  bool    CheckOverlap( Vector2 _ScreenPoint )
        {
            _ScreenPoint    =   FunctionManager.ScreenToGUI( _ScreenPoint );
            return  m_WindowRect.Overlaps( new Rect( _ScreenPoint.x, _ScreenPoint.y, 1, 1 ), true );
        }
    }
    //  編集ウィンドウ 
    class   Window_MapConfig
    {
        private EditManager     m_rParent       =   null;
        private WindowOversee   m_rOversee      =   null;
        private Rect            m_WindowRect    =   new Rect();

        private string          m_WidthText     =   "";
        private string          m_HeightText    =   "";
        private string          m_DepthText     =   "";

        public  Window_MapConfig( EditManager _rParent, WindowOversee _rOversee )
        {
            m_rParent       =   _rParent;
            m_rOversee      =   _rOversee;
            
            m_WidthText     =   _rParent.m_MapWidth.ToString();
            m_HeightText    =   _rParent.m_MapHeight.ToString();
            m_DepthText     =   _rParent.m_MapDepth.ToString();

            InitWindowRect();
        }

        public  void    Update()
        {
            m_WindowRect    =   GUI.Window( 2, m_WindowRect, WindowProc, "Map Config" );
        }
        private void    WindowProc( int _WindowID )
        {
            //  閉じるボタン
            {
                Rect    rect    =   FunctionManager.AdjustRectCanvasToGUI(
                    FunctionManager.AR_TYPE.TOP_RIGHT,
                    new Rect( -6.0f, -5.0f, 14.0f, 10.0f ),
                    new Vector2( 1.0f, 1.0f ),
                    new Vector2( m_WindowRect.width, m_WindowRect.height )
                );
                if( GUI.Button( rect, "" ) ){
                    m_rOversee.m_UseMapConfig   =   false;
                }
            }

            //  ドラッグできるようにする
            GUI.DragWindow( new Rect( 0, 0, m_WindowRect.width, 18 ) );

            //  項目の表示
            {
                float   space   =   22.0f;
                float   offsetY =   24.0f;
                
                //  項目名
                {
                    FunctionManager.GUILabel(
                        FunctionManager.AR_TYPE.TOP_CENTER,
                        new Vector2( 0.0f, -offsetY ),
                        "――   サイズ変更   ――",
                        new Vector2( 0.5f, 1.0f ),
                        new Vector2( m_WindowRect.width, m_WindowRect.height )
                    );
                }
                offsetY +=  space;

                //  項目の表示
                GUI.Label( new Rect( 11, offsetY, 100, 20 ), "X"  );
                m_WidthText     =    ParseString( GUI.TextField( new Rect( 26, offsetY + 1, 30, 20 ), m_WidthText ), m_WidthText );

                //  項目の表示
                GUI.Label( new Rect( 62, offsetY, 100, 20 ), "Y"  );
                m_HeightText    =   ParseString( GUI.TextField( new Rect( 77, offsetY + 1, 30, 20 ), m_HeightText ), m_HeightText );

                GUI.Label( new Rect( 115, offsetY, 100, 20 ), "H"  );
                m_DepthText     =   ParseString( GUI.TextField( new Rect( 130, offsetY + 1, 30, 20 ), m_DepthText ), m_DepthText );
                offsetY +=  space;
                offsetY +=  6;

                //  項目の表示
                {
                    Rect    rect    =   FunctionManager.AdjustRectCanvasToGUI(
                        FunctionManager.AR_TYPE.TOP_CENTER,
                        new Rect( 0, -offsetY, 150, 20 ),
                        new Vector2( 0.5f, 1.0f ),
                        new Vector2( m_WindowRect.width, m_WindowRect.height )
                    );
                    if( GUI.Button( rect, "変更" ) ){
                        m_rParent.ReplaceGrid( Int32.Parse( m_WidthText ), Int32.Parse( m_HeightText ), Int32.Parse( m_DepthText ) );
                    }
                }
                offsetY +=  space;
                offsetY +=  10;

                //  項目名
                {
                    FunctionManager.GUILabel(
                        FunctionManager.AR_TYPE.TOP_CENTER,
                        new Vector2( 0.0f, -offsetY ),
                        "――   マップ拡張   ――",
                        new Vector2( 0.5f, 1.0f ),
                        new Vector2( m_WindowRect.width, m_WindowRect.height )
                    );
                }
                offsetY +=  space;
                offsetY +=  10;

                //  項目名
                {
                    FunctionManager.GUILabel(
                        FunctionManager.AR_TYPE.TOP_CENTER,
                        new Vector2( 0.0f, -offsetY ),
                        "――   マップ編集   ――",
                        new Vector2( 0.5f, 1.0f ),
                        new Vector2( m_WindowRect.width, m_WindowRect.height )
                    );
                }
                offsetY +=  space;
            }
        }

        private string  ParseString( string _CheckStr, string _SpareStr )
        {
            int     number  =   0;

            //  変換
            try{
                number  =   Int32.Parse( _CheckStr );
            }
            //  変換失敗
            catch( Exception _Exception ){
                //  警告回避
                if( _Exception.Message != "" ){}
            
                //  失敗した場合は予備を返す
                return  _SpareStr;
            }

            //  成功
            return  Mathf.Max( 1, number ).ToString();
        }

        public  void    InitWindowRect()
        {
            m_WindowRect    =   FunctionManager.AdjustRectCanvasToGUI(
                FunctionManager.AR_TYPE.BOTTOM_LEFT,
                new Rect( 190, 50, 170, 200 ),
                new Vector2( 0.0f, 0.0f )
            );
            
            m_WidthText     =   m_rParent.m_MapWidth.ToString();
            m_HeightText    =   m_rParent.m_MapHeight.ToString();
            m_DepthText     =   m_rParent.m_MapDepth.ToString();
        }
        public  bool    CheckOverlap( Vector2 _ScreenPoint )
        {
            _ScreenPoint    =   FunctionManager.ScreenToGUI( _ScreenPoint );
            return  m_WindowRect.Overlaps( new Rect( _ScreenPoint.x, _ScreenPoint.y, 1, 1 ), true );
        }
    }
    //  ファイル管理ウィンドウ
    class   Window_FileMenu
    {
        private EditManager     m_rParent       =   null;
        private WindowOversee   m_rOversee      =   null;
        private Rect            m_WindowRect    =   new Rect();

        private Vector2         m_FileScroll    =   Vector2.zero;

        private DirectoryInfo   m_rDirInfo      =   null;
        private FileInfo[]      m_rFileInfo     =   null;
        private Rect            m_FileRect      =   new Rect();

        private bool            m_SaveWindow    =   false;
        private string          m_SaveName      =   "";
        private Rect            m_SaveRect      =   new Rect();
        private bool            m_SaveLight     =   false;

        public  Window_FileMenu( EditManager _rParent, WindowOversee _rOversee )
        {
            m_rDirInfo      =   new DirectoryInfo( Application.dataPath + "/Resources/EditData/" );

            m_rParent       =   _rParent;
            m_rOversee      =   _rOversee;
            
            InitWindowRect();
        }

        public  void    Update()
        {
            m_WindowRect    =   GUI.Window( 3, m_WindowRect, WindowProc, "File Menu" );

            //  セーブウィンドウ表示
            if( m_SaveWindow ){
                Print_SaveWindow();
            }
            //  読み込みリストを表示
            if( m_rFileInfo != null ){
                Print_OpenFileList();
            }
        }
        private void    WindowProc( int _WindowID )
        {
            //  閉じるボタン
            {
                Rect    rect    =   FunctionManager.AdjustRectCanvasToGUI(
                    FunctionManager.AR_TYPE.TOP_RIGHT,
                    new Rect( -6.0f, -5.0f, 14.0f, 10.0f ),
                    new Vector2( 1.0f, 1.0f ),
                    new Vector2( m_WindowRect.width, m_WindowRect.height )
                );
                if( GUI.Button( rect, "" ) ){
                    m_rOversee.m_UseOutput  =   false;
                }
            }

            //  ドラッグできるようにする
            GUI.DragWindow( new Rect( 0, 0, m_WindowRect.width, 18 ) );

            //  項目の表示
            {
                float   space   =   22.0f;
                float   offsetY =   24.0f;
                
                //  項目名
                {
                    FunctionManager.GUILabel(
                        FunctionManager.AR_TYPE.TOP_CENTER,
                        new Vector2( 0.0f, -offsetY ),
                        "――   マップ   ――", 
                        new Vector2( 0.5f, 1.0f ),
                        new Vector2( m_WindowRect.width, m_WindowRect.height )
                    );
                }
                offsetY +=  space;

                //  項目の表示
                {
                    Rect    rect    =   FunctionManager.AdjustRectCanvasToGUI(
                        FunctionManager.AR_TYPE.TOP_CENTER,
                        new Rect( 0, -offsetY, 140, 30 ),
                        new Vector2( 0.5f, 1.0f ),
                        new Vector2( m_WindowRect.width, m_WindowRect.height )
                    );
                    if( GUI.Button( rect, "保存" ) ){
                        m_SaveWindow    =   true;
                        m_rFileInfo     =   null;
                    }
                    offsetY +=  rect.height;
                }
                offsetY +=  5;

                //  項目の表示
                {
                    Rect    rect    =   FunctionManager.AdjustRectCanvasToGUI(
                        FunctionManager.AR_TYPE.TOP_CENTER,
                        new Rect( 0, -offsetY, 140, 20 ),
                        new Vector2( 0.5f, 1.0f ),
                        new Vector2( m_WindowRect.width, m_WindowRect.height )
                    );
                    if( GUI.Button( rect, "ファイルを開く" ) ){
                        m_SaveWindow    =   false;
                        m_FileScroll    =   Vector2.zero;
                        m_rFileInfo     =   m_rDirInfo.GetFiles( "*.prefab" );
                    }
                }
                offsetY +=  space;
                offsetY +=  10;

                //  項目名
                {
                    FunctionManager.GUILabel(
                        FunctionManager.AR_TYPE.TOP_CENTER,
                        new Vector2( 0.0f, -offsetY ),
                        "――   その他   ――", 
                        new Vector2( 0.5f, 1.0f ),
                        new Vector2( m_WindowRect.width, m_WindowRect.height )
                    );
                }
                offsetY +=  space;

                //  項目の表示
                {
                    Rect    rect    =   FunctionManager.AdjustRectCanvasToGUI(
                        FunctionManager.AR_TYPE.TOP_CENTER,
                        new Rect( 0, -offsetY, 140, 24 ),
                        new Vector2( 0.5f, 1.0f ),
                        new Vector2( m_WindowRect.width, m_WindowRect.height )
                    );
                    if( GUI.Button( rect, "NavMesh 作成" ) ){
#if     UNITY_EDITOR
                        NavMeshBuilder.BuildNavMesh();
#endif
                    }
                }
            }
        }
        private void    Print_OpenFileList()
        {
            //  フレーム
            {
                m_FileRect      =   m_WindowRect;
                m_FileRect.x    +=  m_WindowRect.width + 20.0f;

                //  フレーム描画
                GUI.Box( m_FileRect, "File List" );

                //  グループ開始
                GUI.BeginGroup( m_FileRect );
            }

            //  閉じるボタン
            {
                Rect    rect    =   FunctionManager.AdjustRectCanvasToGUI(
                    FunctionManager.AR_TYPE.TOP_RIGHT,
                    new Rect( -6.0f, -5.0f, 14.0f, 10.0f ),
                    new Vector2( 1.0f, 1.0f ),
                    new Vector2( m_FileRect.width, m_FileRect.height )
                );
                if( GUI.Button( rect, "" ) ){
                    m_rFileInfo =   null;
                }
            }

            //  スクロール
            if( m_rFileInfo != null ){
                //  スクロール
                Rect    scrollRect  =   new Rect( 0.0f, 0.0f, m_WindowRect.width, m_WindowRect.height );
                scrollRect.width    -=  14;
                scrollRect.height   -=  30;
                scrollRect.x        +=  10;
                scrollRect.y        +=  24;

                //  スクロール開始
                float   itemHeight  =   m_rFileInfo.Length * 24;
                m_FileScroll    =   GUI.BeginScrollView( scrollRect, m_FileScroll, new Rect( 0, 0, scrollRect.width - 50, itemHeight ) );

                //  リスト表示
                if( m_rFileInfo != null ){
                    for( int i = 0; i < m_rFileInfo.Length; i++ ){
                        FileInfo    rInfo       =   m_rFileInfo[ i ];
                        string      fileName    =   rInfo.Name;
                                    fileName    =   fileName.Substring( 0, fileName.Length - 7 );
                        if( fileName.Length > 12 ){
                            fileName    =   fileName.Substring( 0, 12 ) + "...";
                        }

                        //  読み込みデータ決定
                        if( GUI.Button( new Rect( 0, 0 + 24 * i, m_WindowRect.width - 38, 20 ), fileName ) ){
                            m_rParent.LoadEditData( rInfo.Name );
                            m_SaveName  =   rInfo.Name.Substring( 0, rInfo.Name.Length - 7 );
                            m_rFileInfo =   null;
                            break;
                        }
                    }
                }

                //  スクロール終了
                GUI.EndScrollView();
            }

            //  グループ終了
            GUI.EndGroup();
        }
        private void    Print_SaveWindow()
        {
            //  フレーム
            {
                m_SaveRect          =   m_WindowRect;
                m_SaveRect.x        +=  m_WindowRect.width + 20.0f;
                m_SaveRect.height   =   133.0f;

                //  フレーム描画
                GUI.Box( m_SaveRect, "Save Config" );

                //  グループ開始
                GUI.BeginGroup( m_SaveRect );
            }

            //  閉じるボタン
            {
                Rect    rect    =   FunctionManager.AdjustRectCanvasToGUI(
                    FunctionManager.AR_TYPE.TOP_RIGHT,
                    new Rect( -6.0f, -5.0f, 14.0f, 10.0f ),
                    new Vector2( 1.0f, 1.0f ),
                    new Vector2( m_FileRect.width, m_FileRect.height )
                );
                if( GUI.Button( rect, "" ) ){
                    m_SaveWindow    =   false;
                }
            }

            //  項目の表示
            float   space   =   22.0f;
            float   offsetY =   24.0f;

            //  項目名の表示
            {
                FunctionManager.GUILabel(
                    FunctionManager.AR_TYPE.TOP_CENTER,
                    new Vector2( 0.0f, -offsetY ),
                    "―   ファイル名   ―", 
                    new Vector2( 0.5f, 1.0f ),
                    new Vector2( m_WindowRect.width, m_WindowRect.height )
                );
            }
            offsetY +=  space;

            //  入力欄
            {
                m_SaveName  =   GUI.TextField( new Rect( 10, offsetY, 140, 21 ), m_SaveName );
            }
            offsetY +=  space;
            offsetY +=  5;

            //  チェックボックス
            {
                m_SaveLight =   GUI.Toggle( new Rect( 10, offsetY, 80, 21 ), m_SaveLight, "  軽量版" );
            }

            offsetY +=  space;
            offsetY +=  5;

            //  項目の表示
            {
                Rect    rect    =   FunctionManager.AdjustRectCanvasToGUI(
                    FunctionManager.AR_TYPE.TOP_CENTER,
                    new Rect( 0, -offsetY, 140, 20 ),
                    new Vector2( 0.5f, 1.0f ),
                    new Vector2( m_WindowRect.width, m_WindowRect.height )
                );
                if( GUI.Button( rect, "保存" )
                &&  CheckSaveName( m_SaveName ) ){
                    if( m_SaveLight )   m_rParent.SaveToPrefab_Light( m_SaveName );
                    else                m_rParent.SaveEditData( m_SaveName );

                    m_SaveWindow    =   false;
                }
                offsetY +=  rect.height;
            }

            //  グループ終了
            GUI.EndGroup();
        }
        private bool    CheckSaveName( string _CheckStr )
        {
            if( _CheckStr.Length == 0 )             return  false;
            if( _CheckStr.IndexOf( '.' )  != -1 )   return  false;
            if( _CheckStr.IndexOf( '\\' ) != -1 )   return  false;
            if( _CheckStr.IndexOf( '/' )  != -1 )   return  false;
            if( _CheckStr.IndexOf( ':' )  != -1 )   return  false;
            if( _CheckStr.IndexOf( '*' )  != -1 )   return  false;
            if( _CheckStr.IndexOf( '?' )  != -1 )   return  false;
            if( _CheckStr.IndexOf( '"' )  != -1 )   return  false;
            if( _CheckStr.IndexOf( '<' )  != -1 )   return  false;
            if( _CheckStr.IndexOf( '>' )  != -1 )   return  false;
            if( _CheckStr.IndexOf( '|' )  != -1 )   return  false;

            if( _CheckStr.Trim().Length == 0 )      return  false;

            return  true;
        }

        public  void    InitWindowRect()
        {
            m_WindowRect    =   FunctionManager.AdjustRectCanvasToGUI(
                FunctionManager.AR_TYPE.TOP_LEFT,
                new Rect( 10, -100, 160, 200 ),
                new Vector2( 0.0f, 1.0f )
            );
            m_FileRect      =   m_WindowRect;
            m_FileRect.x    +=  m_WindowRect.width + 20.0f;

            m_SaveWindow    =   false;

            m_rFileInfo     =   null;
            m_FileScroll    =   Vector2.zero;
        }
        public  bool    CheckOverlap( Vector2 _ScreenPoint )
        {
            _ScreenPoint    =   FunctionManager.ScreenToGUI( _ScreenPoint );

            if( m_WindowRect.Overlaps( new Rect( _ScreenPoint.x, _ScreenPoint.y, 1, 1 ), true ) )   return  true;
            if( m_SaveWindow
            &&  m_SaveRect.Overlaps( new Rect( _ScreenPoint.x, _ScreenPoint.y, 1, 1 ), true ) )     return  true;
            if( m_rFileInfo != null
            &&  m_FileRect.Overlaps( new Rect( _ScreenPoint.x, _ScreenPoint.y, 1, 1 ), true ) )     return  true;

            return  false;
        }
    }
    //  情報ウィンドウ
    class   Window_Info
    {
        private EditManager     m_rParent       =   null;
        private WindowOversee   m_rOversee      =   null;
        private Rect            m_WindowRect    =   new Rect();

        public  Window_Info( EditManager _rParent, WindowOversee _rOversee )
        {
            m_rParent   =   _rParent;
            m_rOversee  =   _rOversee;

            InitWindowRect();
        }

        public  void    Update()
        {
            m_WindowRect    =   GUI.Window( 4, m_WindowRect, WindowProc, "Information" );
        }
        private void    WindowProc( int _WindowID )
        {
            //  閉じるボタン
            {
                Rect    rect    =   FunctionManager.AdjustRectCanvasToGUI(
                    FunctionManager.AR_TYPE.TOP_RIGHT,
                    new Rect( -6.0f, -5.0f, 14.0f, 10.0f ),
                    new Vector2( 1.0f, 1.0f ),
                    new Vector2( m_WindowRect.width, m_WindowRect.height )
                );
                if( GUI.Button( rect, "" ) ){
                    m_rOversee.m_UseInfo    =   false;
                }
            }

            //  ドラッグできるようにする
            GUI.DragWindow( new Rect( 0, 0, m_WindowRect.width, 18 ) );

            //  項目の表示
            {
                float   space   =   22.0f;
                float   offsetY =   24.0f;
                
                //  項目の表示
                {
                    GUI.Label( new Rect( 10, offsetY, 200, 20 ), "Cursor :" );

                    FunctionManager.GUILabel(
                        FunctionManager.AR_TYPE.TOP_RIGHT,
                        new Vector2( -12.0f, -offsetY ),
                        ""  +   ( ( int )m_rParent.m_GridCursor.x + 1 ).ToString() .PadLeft( 3, '_' ) + ",  "
                            +   ( ( int )m_rParent.m_GridCursor.y + 1 ).ToString() .PadLeft( 3, '_' ),
                        new Vector2( 1.0f, 1.0f ),
                        new Vector2( m_WindowRect.width, m_WindowRect.height )
                    );
                }
                offsetY +=  space;
                
                //  項目の表示
                {
                    GUI.Label( new Rect( 10, offsetY, 200, 20 ), "Select :" );

                    Vector3 focusSize   =   m_rParent.CalcFocusSize( m_rParent.m_rFocusList );
                    FunctionManager.GUILabel(
                        FunctionManager.AR_TYPE.TOP_RIGHT,
                        new Vector2( -12.0f, -offsetY ),
                        ""  +   ( ( int )focusSize.x ).ToString().PadLeft( 3, '_' ) + ",  "
                            +   ( ( int )focusSize.y ).ToString().PadLeft( 3, '_' ) + ",  "
                            +   ( ( int )focusSize.z ).ToString().PadLeft( 3, '_' ),
                        new Vector2( 1.0f, 1.0f ),
                        new Vector2( m_WindowRect.width, m_WindowRect.height )
                    );
                }
                offsetY +=  space;

                //  項目の表示 
                {
                    GUI.Label( new Rect( 10, offsetY, 200, 20 ), "MapSize :" );

                    FunctionManager.GUILabel(
                        FunctionManager.AR_TYPE.TOP_RIGHT,
                        new Vector2( -12.0f, -offsetY ),
                        ""  +   m_rParent.m_MapWidth.ToString() .PadLeft( 3, '_' ) + ",  "
                            +   m_rParent.m_MapHeight.ToString().PadLeft( 3, '_' ) + ",  "
                            +   m_rParent.m_MapDepth.ToString() .PadLeft( 3, '_' ),
                        new Vector2( 1.0f, 1.0f ),
                        new Vector2( m_WindowRect.width, m_WindowRect.height )
                    );
                }
                offsetY +=  space;
            }
        }

        public  void    InitWindowRect()
        {
            m_WindowRect    =   FunctionManager.AdjustRectCanvasToGUI(
                FunctionManager.AR_TYPE.TOP_RIGHT,
                new Rect( -20, -20, 180, 94 ),
                new Vector2( 1.0f, 1.0f )
            );
        }
        public  bool    CheckOverlap( Vector2 _ScreenPoint )
        {
            _ScreenPoint    =   FunctionManager.ScreenToGUI( _ScreenPoint );
            return  m_WindowRect.Overlaps( new Rect( _ScreenPoint.x, _ScreenPoint.y, 1, 1 ), true );
        }
    }
	//	ゲームワールド能力設定ウィンドウ
	class   Window_GameWorldParameter
	{
		private EditManager		m_rParent		=	null;
		private WindowOversee	m_rOversee		=	null;
		private Rect			m_WindowRect	=	new Rect();

		private Vector2 scrollViewVector = Vector2.zero;

		private bool Resizeing = false;
		private float ClickY = .0f;
		private float ClickHeight = .0f;

		public Window_GameWorldParameter(EditManager _rParent, WindowOversee _rOversee)
		{
			m_rParent = _rParent;
			m_rOversee = _rOversee;

			InitWindowRect();
		}
		public void Update()
		{
			m_WindowRect = GUI.Window(6, m_WindowRect, WindowProc, "GameWorldParameter");
			if (Input.GetMouseButtonUp(0))
			{
				Resizeing = false;
			}

		}
		private void WindowProc(int _WindowID)
		{
			//  閉じるボタン
			{
				Rect rect = FunctionManager.AdjustRectCanvasToGUI(
					FunctionManager.AR_TYPE.TOP_RIGHT,
					new Rect(-6.0f, -5.0f, 14.0f, 10.0f),
					new Vector2(1.0f, 1.0f),
					new Vector2(m_WindowRect.width, m_WindowRect.height)
				);
				if (GUI.Button(rect, ""))
				{
					m_rOversee.m_UseGameWorldParameter = false;
				}
			}



			//  ドラッグできるようにする
			GUI.DragWindow(new Rect(0, 0, m_WindowRect.width, 18));



			//  項目の表示
			{
				float space = 22.0f;
				float offsetY = 24.0f - 18.0f;
				scrollViewVector = GUI.BeginScrollView(new Rect(0, 18, m_WindowRect.width, m_WindowRect.height - 18 - 30), scrollViewVector, new Rect(0, 0, 0, 300));
				GameWorldParameter param = GameWorldParameter.instance;

				//  項目の表示
				{
					DispLabel("-----TPSプレイヤー-----", ref offsetY, space);
					DispValue("体力", ref param.TPSPlayer.Health, ref offsetY, space);
					DispValue("移動速度", ref param.TPSPlayer.WalkSpeed, ref offsetY, space);
					DispValue("ジャンプ力", ref param.TPSPlayer.JumpPower, ref offsetY, space);
					DispValue("ホバー力", ref param.TPSPlayer.HoverPower, ref offsetY, space);
					DispValue("ホバー速度", ref param.TPSPlayer.HoverSpeed, ref offsetY, space);
					DispValue("ホバー時間", ref param.TPSPlayer.HoverTime, ref offsetY, space);
					DispValue("ステップ量", ref param.TPSPlayer.StepPower, ref offsetY, space);



					offsetY += space;
				}
				GUI.EndScrollView();
				#if     UNITY_EDITOR
				if(GUI.Button(new Rect(10, m_WindowRect.height - 30, 200,20), "インスペクタ表示"))
				{
						Selection.objects = new GameObject[] { param.gameObject };
				}
				#endif

				if (GUI.RepeatButton(new Rect(0, m_WindowRect.height - 10, m_WindowRect.width, 10), "▼"))
				{

					Resizeing = true;
					ClickHeight = m_WindowRect.height;
					ClickY = Input.mousePosition.y;

				}
				if (Resizeing == true)
				{
					m_WindowRect.height = ClickHeight + (ClickY - Input.mousePosition.y);
					if (m_WindowRect.height < 60)
						m_WindowRect.height = 60;
				}

			}


		}

		void DispValue(string name,ref float value ,ref float offsetY,float space)
		{
			GUI.Label(new Rect(10, offsetY, 100, 20), name);
			string ret = GUI.TextField(new Rect(200, offsetY, 90, 20),value.ToString());
			value = float.Parse(ret);


			offsetY += space;
		}
		void DispLabel(string name, ref float offsetY, float space)
		{
			GUI.Label(new Rect(10, offsetY, 400, 20), name);

			offsetY += space;
		}
		public void InitWindowRect()
		{
			m_WindowRect = FunctionManager.AdjustRectCanvasToGUI(
				FunctionManager.AR_TYPE.TOP_RIGHT,
				new Rect(-20, -20, 480, 94),
				new Vector2(1.0f, 1.0f)
			);
		}
		public bool CheckOverlap(Vector2 _ScreenPoint)
		{
			_ScreenPoint = FunctionManager.ScreenToGUI(_ScreenPoint);
			return m_WindowRect.Overlaps(new Rect(_ScreenPoint.x, _ScreenPoint.y, 1, 1), true);
		}

	}

	//  ブロックウィンドウ
	class   Window_Block
    {
        private EditManager     m_rParent       =   null;
        private WindowOversee   m_rOversee      =   null;
        private Rect            m_WindowRect    =   new Rect();

        private Vector2         m_FileScroll    =   Vector2.zero;

        private DirectoryInfo   m_rDirInfo      =   null;
        private FileInfo[]      m_rFileInfo     =   null;
        private Rect            m_FileRect      =   new Rect();

        public  Window_Block( EditManager _rParent, WindowOversee _rOversee )
        {
            m_rDirInfo  =   new DirectoryInfo( Application.dataPath + "/Resources/EditImport_Block/" );

            m_rParent   =   _rParent;
            m_rOversee  =   _rOversee;

            InitWindowRect();
        }
        public  void    Update()
        {
            m_WindowRect    =   GUI.Window( 5, m_WindowRect, WindowProc, "Block Config" );

            //  読み込みリストを表示
            if( m_rFileInfo != null ){
                Print_OpenFileList();
            }
        }
        private void    WindowProc( int _WindowID )
        {
            //  閉じるボタン
            {
                Rect    rect    =   FunctionManager.AdjustRectCanvasToGUI(
                    FunctionManager.AR_TYPE.TOP_RIGHT,
                    new Rect( -6.0f, -5.0f, 14.0f, 10.0f ),
                    new Vector2( 1.0f, 1.0f ),
                    new Vector2( m_WindowRect.width, m_WindowRect.height )
                );
                if( GUI.Button( rect, "" ) ){
                    m_rOversee.m_UseBox =   false;
                }
            }

            //  ドラッグできるようにする
            GUI.DragWindow( new Rect( 0, 0, m_WindowRect.width, 18 ) );

            //  項目の表示
            {
                float   space   =   22.0f;
                float   offsetY =   24.0f;
                
                //  項目名 
                {
                    FunctionManager.GUILabel(
                        FunctionManager.AR_TYPE.TOP_CENTER,
                        new Vector2( 0.0f, -offsetY ),
                        "――   使用ブロック   ――",
                        new Vector2( 0.5f, 1.0f ),
                        new Vector2( m_WindowRect.width, m_WindowRect.height )
                    );
                }
                offsetY +=  space;

                //  項目の表示
                {
                    FunctionManager.GUILabel(
                        FunctionManager.AR_TYPE.TOP_CENTER,
                        new Vector2( 0.0f, -offsetY ),
                        m_rParent.m_UseBlockName,
                        new Vector2( 0.5f, 1.0f ),
                        new Vector2( m_WindowRect.width, m_WindowRect.height )
                    );
                }
                offsetY +=  space;
                offsetY +=  4;

                //  項目を表示
                {
                    Rect    rect    =   FunctionManager.AdjustRectCanvasToGUI(
                        FunctionManager.AR_TYPE.TOP_CENTER,
                        new Rect( 0, -offsetY, 160, 20 ),
                        new Vector2( 0.5f, 1.0f ),
                        new Vector2( m_WindowRect.width, m_WindowRect.height )
                    );
                    if( GUI.Button( rect, "変更" ) ){
                        m_FileScroll    =   Vector2.zero;
                        m_rFileInfo     =   m_rDirInfo.GetFiles( "*.prefab" );
                    }

                    offsetY +=  rect.height;
                }
                offsetY +=  10;

                //  項目名 
                {
                    FunctionManager.GUILabel(
                        FunctionManager.AR_TYPE.TOP_CENTER,
                        new Vector2( 0.0f, -offsetY ),
                        "――   配置設定   ――",
                        new Vector2( 0.5f, 1.0f ),
                        new Vector2( m_WindowRect.width, m_WindowRect.height )
                    );
                }
                offsetY +=  space;

                //  項目の表示
                {
                    FunctionManager.GUILabel(
                        FunctionManager.AR_TYPE.TOP_CENTER,
                        new Vector2( 0.0f, -offsetY ),
                        "" + ( int )m_rParent.m_BlockAngle + " 度",
                        new Vector2( 0.5f, 1.0f ),
                        new Vector2( m_WindowRect.width, m_WindowRect.height )
                    );
                }
                //  項目を表示
                {
                    Rect    rect    =   FunctionManager.AdjustRectCanvasToGUI(
                        FunctionManager.AR_TYPE.TOP_LEFT,
                        new Rect( 10, -offsetY - 1, 40, 18 ),
                        new Vector2( 0.0f, 1.0f ),
                        new Vector2( m_WindowRect.width, m_WindowRect.height )
                    );
                    if( GUI.Button( rect, "左" ) ){
                        m_rParent.AddPlaceBlockAngle( -90 );
                    }
                }
                //  項目を表示
                {
                    Rect    rect    =   FunctionManager.AdjustRectCanvasToGUI(
                        FunctionManager.AR_TYPE.TOP_RIGHT,
                        new Rect( -10, -offsetY - 1, 40, 18 ),
                        new Vector2( 1.0f, 1.0f ),
                        new Vector2( m_WindowRect.width, m_WindowRect.height )
                    );
                    if( GUI.Button( rect, "右" ) ){
                        m_rParent.AddPlaceBlockAngle( +90 );
                    }
                }

                offsetY +=  space;
            }
        }
        private void    Print_OpenFileList()
        {
            //  フレーム
            {
                m_FileRect      =   m_WindowRect;
                m_FileRect.x    -=  m_WindowRect.width + 20.0f;

                //  フレーム描画
                GUI.Box( m_FileRect, "File List" );

                //  グループ開始
                GUI.BeginGroup( m_FileRect );
            }

            //  閉じるボタン
            {
                Rect    rect    =   FunctionManager.AdjustRectCanvasToGUI(
                    FunctionManager.AR_TYPE.TOP_RIGHT,
                    new Rect( -6.0f, -5.0f, 14.0f, 10.0f ),
                    new Vector2( 1.0f, 1.0f ),
                    new Vector2( m_FileRect.width, m_FileRect.height )
                );
                if( GUI.Button( rect, "" ) ){
                    m_rFileInfo =   null;
                }
            }

            //  スクロール
            if( m_rFileInfo != null ){
                //  スクロール
                Rect    scrollRect  =   new Rect( 0.0f, 0.0f, m_WindowRect.width, m_WindowRect.height );
                scrollRect.width    -=  14;
                scrollRect.height   -=  30;
                scrollRect.x        +=  10;
                scrollRect.y        +=  24;

                //  スクロール開始
                float   itemHeight  =   m_rFileInfo.Length * 24;
                m_FileScroll    =   GUI.BeginScrollView( scrollRect, m_FileScroll, new Rect( 0, 0, scrollRect.width - 50, itemHeight ) );

                //  リスト表示
                if( m_rFileInfo != null ){
                    for( int i = 0; i < m_rFileInfo.Length; i++ ){
                        FileInfo    rInfo       =   m_rFileInfo[ i ];
                        string      fileName    =   rInfo.Name;
                                    fileName    =   fileName.Substring( 0, fileName.Length - 7 );
                        if( fileName.Length > 16 ){
                            fileName    =   fileName.Substring( 0, 16 ) + "...";
                        }

                        //  読み込みデータ決定
                        if( GUI.Button( new Rect( 0, 0 + 24 * i, m_WindowRect.width - 38, 20 ), fileName ) ){
                            m_rParent.LoadBlockData( rInfo.Name );
                            m_rFileInfo =   null;
                            break;
                        }
                    }
                }

                //  スクロール終了
                GUI.EndScrollView();
            }

            //  グループ終了
            GUI.EndGroup();
        }

        public  void    InitWindowRect()
        {
            m_WindowRect    =   FunctionManager.AdjustRectCanvasToGUI(
                FunctionManager.AR_TYPE.TOP_RIGHT,
                new Rect( -220, -20, 180, 154 ),
                new Vector2( 1.0f, 1.0f )
            );
            m_FileRect      =   m_WindowRect;
            m_FileRect.x    -=  m_WindowRect.width + 20.0f;

            m_rFileInfo     =   null;
            m_FileScroll    =   Vector2.zero;
        }
        public  bool    CheckOverlap( Vector2 _ScreenPoint )
        {
            _ScreenPoint    =   FunctionManager.ScreenToGUI( _ScreenPoint );

            if( m_WindowRect.Overlaps( new Rect( _ScreenPoint.x, _ScreenPoint.y, 1, 1 ), true ) )   return  true;
            if( m_rFileInfo != null
            &&  m_FileRect.Overlaps( new Rect( _ScreenPoint.x, _ScreenPoint.y, 1, 1 ), true ) )     return  true;

            return  false;
        }
    }
    //  設定ウィンドウ 
    class   Window_Setting
    {
        //private EditManager     m_rParent       =   null;
        //private WindowOversee   m_rOversee      =   null;
        private Rect            m_WindowRect    =   new Rect();

        public  Window_Setting( EditManager _rParent, WindowOversee _rOversee )
        {
            //m_rParent   =   _rParent;
            //m_rOversee  =   _rOversee;
        }

        public  bool    CheckOverlap( Vector2 _ScreenPoint )
        {
            _ScreenPoint    =   FunctionManager.ScreenToGUI( _ScreenPoint );
            return  m_WindowRect.Overlaps( new Rect( _ScreenPoint.x, _ScreenPoint.y, 1, 1 ), true );
        }
    }
    
    public  GameObject          c_GridPanel         =   null;
    public  GameObject          c_SlopeBlock        =   null;
    public  GameObject          c_Base              =   null;
    public  GameObject          c_SpawnPoint        =   null;
    public  GameObject          c_LaunchPoint       =   null;
    public  GameObject          c_EmptyBlock        =   null;

    public  Material            c_StageMaterial     =   null;

    public  Color               c_DefaultColor      =   Color.black;
    public  Color               c_FocusColor        =   Color.white;

    public  float               c_GridWidth         =   1.0f;
    public  float               c_GridScale         =   1.0f;

    public  float               c_OlthoSize         =   1.8f;
    public  float               c_PlaceHeight       =   0.0f;

    public  float               m_WallHeight        =   1.0f;

    public  int                 m_MapWidth          =   1;
    public  int                 m_MapHeight         =   1;
    public  int                 m_MapDepth          =   1;

    public  bool                m_VisibleEmpty      =   true;
    public  bool                m_UseHeightColor    =   true;

    public  GameObject          c_PlayTestShell     =   null;
    public  GameObject          c_EnemySpawnPoint   =   null;
    public  GameObject          c_HomeBase          =   null;

    private string              c_FilePath          =   "Assets/Resources/EditData/";
    private string              c_LightPath         =   "Assets/Resources/EditData_Light/";
    private string              c_MeshPath          =   "Assets/Resources/EditData_Mesh/";
    private string              c_BlockPath         =   "EditImport_Block/";
    private string              c_LoadPath          =   "EditData/";

    private float               c_FrameWidth        =   5.75f;
    private float               c_FrameHeight       =   3.0f;
    private Vector3             c_Center            =   Vector3.zero;

    //  配置ブロック
    public  List< GameObject >  m_rBlockList        =   new List< GameObject >();
    public  GameObject          m_rUseBlock         =   null;
    public  string              m_UseBlockName      =   "";
    public  int                 m_BlockAngle        =   0;

    //  編集パラメータ
    private Transform           m_rEditData         =   null;
    private Transform           m_rGridShell        =   null;

    private Transform           m_rWallShell        =   null;
    private Transform           m_rSlopShell        =   null;
    private Transform           m_rBoxShell         =   null;
    private Transform           m_rBaseShell        =   null;
    private Transform           m_rSpawnPointShell  =   null;
    private Transform           m_rLaunchPointShell =   null;
    private Transform           m_rEmptyShell       =   null;

    private Transform           m_rMapFloor         =   null;
    private Camera              m_rEditCamera       =   null;

    private EditData_Config     m_rDataConfig       =   null;

    private EditMode            m_EditMode          =   EditMode.Select;
    private Vector3             m_PrevPoint         =   -Vector3.one;
    private Vector3             m_PrevMouse         =   Vector3.zero;
    private bool                m_PrevPaint         =   false;

    private Vector3             m_RectStart         =   -Vector3.one;
    private Vector2             m_GridCursor        =   Vector2.zero;

    private bool                m_GridConnectOK     =   false;
    private float               m_GridAccessTimer   =   0.0f;

    //  プレイテスト用パラメータ
    private NetworkManager      m_rNetworkManager   =   null;
    private Transform           m_rPlayTestShell    =   null;
    private bool                m_PlayTest          =   false;
    
    //  ウィンドウパラメータ  
    private WindowOversee       m_rWindowOver       =   null;
    private bool                m_OpenWindow        =   false;

    //  
    private EditCamera          m_rECameraCtrl      =   null;

    private List< GridPanel_Control >
                                m_rGridList         =   new List< GridPanel_Control >();
    private List< GridPanel_Control >
                                m_rFocusList        =   new List< GridPanel_Control >();

	// Use this for initialization
	void    Start()
    {
        //  アクセス取得
        GetAccessToExternalObject();

        //  コントロール初期化
        m_rECameraCtrl  =   new EditCamera( this );

        //  ウィンドウ初期化
        m_rWindowOver   =   new WindowOversee( this );

        //  グリッド初期化
	    ReplaceGrid( m_MapWidth, m_MapHeight, m_MapDepth );
	}
    void    GetAccessToExternalObject()
    {
        m_rNetworkManager   =   FunctionManager.GetAccessComponent< NetworkManager >( "NetworkManager" );

        m_rEditData         =   GameObject.Find( "Edit_Data" ).transform;
        m_rGridShell        =   GameObject.Find( "Grid_Shell" ).transform;
        m_rWallShell        =   GameObject.Find( "Edit_Data/Wall_Shell" ).transform;
        m_rSlopShell        =   GameObject.Find( "Edit_Data/Slope_Shell" ).transform;
        m_rBoxShell         =   GameObject.Find( "Edit_Data/Box_Shell" ).transform;
        m_rBaseShell        =   GameObject.Find( "Edit_Data/Base_Shell" ).transform;
        m_rSpawnPointShell  =   GameObject.Find( "Edit_Data/SpawnPoint_Shell" ).transform;
        m_rLaunchPointShell =   GameObject.Find( "Edit_Data/LaunchPoint_Shell" ).transform;
        m_rEmptyShell       =   GameObject.Find( "Edit_Data/Empty_Shell" ).transform;
        m_rMapFloor         =   GameObject.Find( "Edit_Data/MapFloor" ).transform;
        m_rEditCamera       =   GameObject.Find( "Main Camera" ).GetComponent< Camera >(); 

        m_rDataConfig       =   GameObject.Find( "Edit_Data/ConfigData" ).GetComponent< EditData_Config >();

        if( !m_rBoxShell )  m_rBoxShell =   CreateEmptyObject( "Edit_Data", "Box_Shell" );
    }
	
	// Update is called once per frame
	void    Update()
    {
        //  プレイテスト開始 / 終了
        if( Input.GetKeyDown( KeyCode.F12 ) ){
            if( m_PlayTest )    PT_EndPlayTest();
            else                PT_StartPlayTest();
        }
        //  高さに色をつける
        if( Input.GetKeyDown( KeyCode.F1 ) ){
            m_UseHeightColor    =   !m_UseHeightColor;
        }

        //  プレイテスト中は編集しない
        if( m_PlayTest )    return;

        //  壁を矩形内に生成
        if( Input.GetKeyDown( KeyCode.I ) ){
            SetLargeWall( m_rFocusList );
        }

        //  向きを変更
        if( Input.GetKeyDown( KeyCode.Comma ) )     AddPlaceBlockAngle( -90 );
        if( Input.GetKeyDown( KeyCode.Period ) )    AddPlaceBlockAngle(  90 );

        //  オブジェクトのアクセス取得
        {
            if( m_GridAccessTimer > 0.0f ){
                m_GridAccessTimer   -=  Time.deltaTime;
                m_GridAccessTimer   =   Mathf.Max( m_GridAccessTimer, 0.0f );
                if( m_GridAccessTimer == 0.0f ){
                    UpdateGridObjectAccess();
                }
            }
            if( Input.GetKeyDown( KeyCode.F7 ) ){
                UpdateGridObjectAccess();
            }
        }

        //  接続情報更新
        if( !m_GridConnectOK ){
            m_GridConnectOK =   SetGrid_Connection();
        }

        //  カメラ操作
        m_rECameraCtrl.Update( m_rEditCamera );

        //  サイズ変更
        if( Input.GetKey( KeyCode.LeftShift )
        ||  Input.GetKey( KeyCode.RightShift ) ){
            if( Input.GetKeyDown( KeyCode.UpArrow ) )       ExtensionMap( new Vector3(  0, -1,  0  ), new Vector3(  0,  0,  0  ) );
            if( Input.GetKeyDown( KeyCode.DownArrow ) )     ExtensionMap( new Vector3(  0, -1,  0  ), new Vector3(  0,  1,  0  ) );
            if( Input.GetKeyDown( KeyCode.RightArrow ) )    ExtensionMap( new Vector3( -1,  0,  0  ), new Vector3(  0,  0,  0  ) );
            if( Input.GetKeyDown( KeyCode.LeftArrow ) )     ExtensionMap( new Vector3( -1,  0,  0  ), new Vector3(  1,  0,  0  ) );

            if( Input.GetKeyDown( KeyCode.LeftBracket ) )   ExtensionMap( new Vector3(  0,  0, -1  ), new Vector3(  0,  0,  0  ) );
            if( Input.GetKeyDown( KeyCode.RightBracket ) )  ExtensionMap( new Vector3(  0,  0, -1  ), new Vector3(  0,  0,  1  ) );
        }
        else{
            if( Input.GetKeyDown( KeyCode.UpArrow ) )       ExtensionMap( new Vector3(  0,  1,  0  ), new Vector3(  0,  0,  0  ) );
            if( Input.GetKeyDown( KeyCode.DownArrow ) )     ExtensionMap( new Vector3(  0,  1,  0  ), new Vector3(  0, -1,  0  ) );
            if( Input.GetKeyDown( KeyCode.RightArrow ) )    ExtensionMap( new Vector3(  1,  0,  0  ), new Vector3(  0,  0,  0  ) );
            if( Input.GetKeyDown( KeyCode.LeftArrow ) )     ExtensionMap( new Vector3(  1,  0,  0  ), new Vector3( -1,  0,  0  ) );

            if( Input.GetKeyDown( KeyCode.LeftBracket ) )   ExtensionMap( new Vector3(  0,  0,  1  ), new Vector3(  0,  0,  0  ) );
            if( Input.GetKeyDown( KeyCode.RightBracket ) )  ExtensionMap( new Vector3(  0,  0,  1  ), new Vector3(  0,  0, -1  ) );
        }
        if( ( Input.GetKey( KeyCode.LeftAlt )      || Input.GetKey( KeyCode.RightAlt ) )
        &&  ( Input.GetKey( KeyCode.RightControl ) || Input.GetKey( KeyCode.LeftControl ) ) ){
            if( Input.GetKeyDown( KeyCode.UpArrow ) )       ExtensionMap( new Vector3(  0,  0,  0  ), new Vector3(  0,  0,  0  ), new Vector3(  0, -1,  0  ) );
            if( Input.GetKeyDown( KeyCode.DownArrow ) )     ExtensionMap( new Vector3(  0,  0,  0  ), new Vector3(  0,  0,  0  ), new Vector3(  0,  1,  0  ) );
            if( Input.GetKeyDown( KeyCode.RightArrow ) )    ExtensionMap( new Vector3(  0,  0,  0  ), new Vector3(  0,  0,  0  ), new Vector3( -1,  0,  0  ) );
            if( Input.GetKeyDown( KeyCode.LeftArrow ) )     ExtensionMap( new Vector3(  0,  0,  0  ), new Vector3(  0,  0,  0  ), new Vector3(  1,  0,  0  ) );
        }

        //  ペイント操作
        {
            //  カーソルが触れているマス
            GridPanel_Control   rControl    =   GetHitPanel_InGrid();

            //  クリックする度に選択をリセット
            if( Input.GetMouseButtonDown( 0 ) ){
                if( !Input.GetKey( KeyCode.RightControl ) && !Input.GetKey( KeyCode.LeftControl )
                &&  !Input.GetKey( KeyCode.RightAlt )     && !Input.GetKey( KeyCode.LeftAlt )
                &&  !Input.GetKey( KeyCode.RightShift )   && !Input.GetKey( KeyCode.LeftShift )
                &&  !CheckOverlapWindow() ){
                    SetFocus_All( false );
                }
            }

            //  矩形選択
            if( Input.GetKey( KeyCode.RightShift )
            ||  Input.GetKey( KeyCode.LeftShift ) ){
                if( rControl
                &&  CheckOverlapWindow() == false ){
                    //  選択開始
                    if( Input.GetMouseButtonDown( 0 ) ) m_RectStart =   rControl.m_GridPoint;
                    //  選択中
                    if( Input.GetMouseButton( 0 ) ){
                        if( m_RectStart.x == -1 )       m_RectStart =   rControl.m_GridPoint;
                        else                            SetFocusByRectangle( rControl.m_GridPoint, m_RectStart );
                    }
                }
            }
            //  矩形選択終了
            if( ( !Input.GetKey( KeyCode.RightShift ) && !Input.GetKey( KeyCode.LeftShift ) )
            ||  ( !Input.GetMouseButton( 0 ) ) ){
                m_RectStart =   -Vector3.one;
            }

            //  塗る
            if( Input.GetMouseButton( 0 )
            ||  Input.GetMouseButton( 1 ) ){
                //  補間して塗る
                if( m_PrevPaint ){
                    LerpPaint( m_PrevMouse, Input.mousePosition );
                }

                //  触れているパネルを取得
                if( rControl
                &&  CheckOverlapWindow() == false ){
                    //  前回塗ったマスには塗らない
                    if( rControl.m_GridPoint.x != m_PrevPoint.x
                    ||  rControl.m_GridPoint.y != m_PrevPoint.y ){
                        if( Input.GetMouseButton( 0 ) ) PaintGrid( rControl );
                        if( Input.GetMouseButton( 1 ) ) Erase( rControl );
                    }

                    m_PrevPoint =   rControl.m_GridPoint;
                }
            }
            //  重ね塗り防止フラグリセット
            if( !Input.GetMouseButton( 0 )
            &&  !Input.GetMouseButton( 1 ) ){
                m_PrevPoint     =   -Vector3.one;
                m_PrevPaint     =   false;
            }
            else{
                m_PrevPaint     =   true;
                m_PrevMouse     =   Input.mousePosition;
            }

            //  カーソル位置保存
            if( rControl ){
                m_GridCursor    =   rControl.m_GridPoint;
            }
        }
        //  選択モードでなければ選択を解除
        if( m_EditMode != EditMode.Select ){
            SetFocus_All( false );
        }

        //  選択を拡張 
        if( Input.GetKeyDown( KeyCode.U ) ){
            SetFocus_PlusUP();
        }

        //  選択部分を塗る
        if( Input.GetKeyDown( KeyCode.Alpha1 ) )    PaintGrid_Focus( EditMode.Wall );
        if( Input.GetKeyDown( KeyCode.Alpha2 ) )    PaintGrid_Focus( EditMode.Base );
        if( Input.GetKeyDown( KeyCode.Alpha3 ) )    PaintGrid_Focus( EditMode.SpawnPoint );
        if( Input.GetKeyDown( KeyCode.Alpha4 ) )    PaintGrid_Focus( EditMode.LaunchPoint );
        if( Input.GetKeyDown( KeyCode.Alpha5 ) )    PaintGrid_Focus( EditMode.Empty );
        if( Input.GetKeyDown( KeyCode.Alpha7 ) ){
            SetSlope( m_rFocusList );
            SetFocus_All( false );
        }

        //  選択部分を削除
        if( Input.GetKeyDown( KeyCode.Backspace ) ){
            Erase_Focus();
        }
        //  選択を全削除
        if( Input.GetKeyDown( KeyCode.Delete ) ){
            int loopCount   =   m_rFocusList.Count;
            for( int i = 0; i < loopCount; i++ ){
                if( m_rFocusList.Count == 0 )   break;
                Erase_Vertical( m_rFocusList[ 0 ] );
            }
            SetFocus_All( false );
        }
	}

    void    OnGUI()
    {
        //  プレイテスト中は表示しない
        if( m_PlayTest )    return;

        //  ウィンドウ設定
        {
            float   screenRatio =   Screen.width / 1024.0f;
            Rect    rect    =   FunctionManager.AdjustRectCanvasToGUI(
                FunctionManager.AR_TYPE.BOTTOM_LEFT,
                new Rect( ( int )( 20.0f * screenRatio ), ( int )( 30.0f * screenRatio ), 110.0f, 20.0f ),
                new Vector2( 0.0f, 0.5f )
            );
            m_OpenWindow    =   GUI.Toggle( rect, m_OpenWindow, "  Open Window" );
        }

        //  ウィンドウ更新
        m_rWindowOver.Update();
    }


    //  カーソルがウィンドウ内にあるかどうかをチェック
    bool    CheckWhether_CursorInWindow()
    {
        if( Input.mousePosition.x < 0 )             return  false;
        if( Input.mousePosition.x > Screen.width )  return  false;
        if( Input.mousePosition.y < 0 )             return  false;
        if( Input.mousePosition.y > Screen.height ) return  false;

        return  true;
    }
    //  カーソルがウィンドウに重なっていないかどうかチェック
    bool    CheckOverlapWindow()
    {
        return  CheckOverlapWindow( Input.mousePosition );
    }
    bool    CheckOverlapWindow( Vector3 _ScreenPos )
    {
        if( m_rWindowOver.CheckOverlap_All( _ScreenPos ) )  return  true;

        return  false;
    }
    //  カーソルが触れているパネルを取得
    GridPanel_Control   GetHitPanel_InGrid()
    {
        return  GetHitPanel_InGrid( Input.mousePosition );
    }
    GridPanel_Control   GetHitPanel_InGrid( Vector3 _ScreenPos )
    {
        //  入力情報を設定
        RaycastHit  rHit        =   new RaycastHit();
        Ray         rRay        =   Camera.main.ScreenPointToRay( _ScreenPos );
        float       maxDist     =   float.MaxValue;
        int         layerMask   =   LayerMask.GetMask( "Edit_GridPanel" );
        
        //  レイ判定
        if( Physics.Raycast( rRay, out rHit, maxDist, layerMask ) ){
            return  rHit.collider.GetComponent< GridPanel_Control >();
        }

        //  何も触れていない
        return  null;
    }

    //  エディットデータを保存
    void    SaveEditData( string _FileName )
    {
        //  グリッドの選択状態を解除
        SetFocus_All( false );

        //  データを保存
        {
            m_rDataConfig.Clear();

            m_rDataConfig.m_MapWidth    =   m_MapWidth;
            m_rDataConfig.m_MapHeight   =   m_MapHeight;
            m_rDataConfig.m_MapDepth    =   m_MapDepth;
            m_rDataConfig.m_GridWidth   =   c_GridWidth;
            m_rDataConfig.m_WallHeight  =   m_WallHeight;
        }

        //  保存
        SaveToPrefab( m_rEditData.gameObject, _FileName );
    }
    //  エディットデータを読み込み
    void    LoadEditData( string _FileName )
    {
        //  拡張子がついている場合は消す
        if( _FileName.IndexOf( '.' ) != -1 ){
            string  ext =   Path.GetExtension( _FileName );
            _FileName   =   _FileName.Substring( 0, _FileName.Length - ext.Length );
        }

        //  オブジェクトをロード
        GameObject  rPrefab =   Resources.Load( c_LoadPath + _FileName ) as GameObject;
        //  見つからなかったら終了
        if( !rPrefab )  return;

        //  オブジェクトをクリア
        Erase_All();
        //  グリッドをクリア
        EraseGrid();

        //  データを削除
        if( m_rEditData ){
            Destroy( m_rEditData.gameObject );
            m_rEditData =   null;
        }

        //  プレハブからデータを作成
        GameObject      rNewEDObj   =   Instantiate( rPrefab );
        Transform       rNewEDTrans =   rNewEDObj.transform;

        //  名前を規定値にする
        rNewEDObj.name  =   "Edit_Data";

        //  アクセスを取得
        {
            m_rEditData         =   rNewEDTrans;
            m_rWallShell        =   rNewEDTrans.FindChild( "Wall_Shell" );
            m_rSlopShell        =   rNewEDTrans.FindChild( "Slope_Shell" );
            m_rBoxShell         =   rNewEDTrans.FindChild( "Box_Shell" );
            m_rBaseShell        =   rNewEDTrans.FindChild( "Base_Shell" );
            m_rSpawnPointShell  =   rNewEDTrans.FindChild( "SpawnPoint_Shell" );
            m_rLaunchPointShell =   rNewEDTrans.FindChild( "LaunchPoint_Shell" );
            m_rEmptyShell       =   rNewEDTrans.FindChild( "Empty_Shell" );
            m_rMapFloor         =   rNewEDTrans.FindChild( "MapFloor" );

            m_rDataConfig       =   rNewEDTrans.FindChild( "ConfigData" ).GetComponent< EditData_Config >();

            //  互換
            if( !m_rBoxShell ){
                GameObject  rObj    =   new GameObject( "Box_Shell" );
                m_rBoxShell         =   rObj.transform;
                m_rBoxShell.parent  =   m_rEditData;
            }
            if( !m_rLaunchPointShell ){
                GameObject  rObj            =   new GameObject( "LaunchPoint_Shell" );
                m_rLaunchPointShell         =   rObj.transform;
                m_rLaunchPointShell.parent  =   m_rEditData;
            }
        }

        //  マップ情報取得
        {
            m_MapWidth      =   m_rDataConfig.m_MapWidth;
            m_MapHeight     =   m_rDataConfig.m_MapHeight;
            m_MapDepth      =   m_rDataConfig.m_MapDepth;
            c_GridWidth     =   m_rDataConfig.m_GridWidth;
            m_WallHeight    =   m_rDataConfig.m_WallHeight;
        }

        //  グリッド作成
        {
            //  配置情報計算
            float   wholeWidth  =   c_GridWidth * m_MapWidth;
            float   wholeHeight =   c_GridWidth * m_MapHeight;
            float   gridLeft    =   c_Center.x - wholeWidth  * 0.5f + c_GridWidth * 0.5f;
            float   gridTop     =   c_Center.z - wholeHeight * 0.5f + c_GridWidth * 0.5f;

            //  グリッド配置
            //Transform   rDataTrans  =   m_rDataConfig.transform;
            for( int z = 0; z < m_MapDepth;  z++ ){
            for( int y = 0; y < m_MapHeight; y++ ){
            for( int x = 0; x < m_MapWidth;  x++ ){
                GameObject  rMyObj      =   Instantiate( c_GridPanel );
                Transform   rMyTrans    =   rMyObj.transform;

                //  親を設定
                rMyTrans.parent     =   m_rGridShell;

                //  名前設定
                rMyObj.name         =   "GridPanel_( "
                                    +   "X_" + x.ToString().PadLeft( 3, '_' ) + ", "
                                    +   "Y_" + y.ToString().PadLeft( 3, '_' ) + ", "
                                    +   "H_" + z.ToString().PadLeft( 3, '_' ) + " )";

                //  サイズ設置
                rMyTrans.localScale =   Vector3.one * c_GridWidth * c_GridScale;

                //  座標設定
                rMyTrans.position   =   new Vector3(
                    gridLeft + c_GridWidth * x,
                    c_PlaceHeight,
                    gridTop  + c_GridWidth * y
                );

                //  パラメータ設定
                GridPanel_Control   rControl    =   rMyObj.GetComponent< GridPanel_Control >();
                rControl.m_GridPos      =   rMyTrans.position;
                rControl.m_GridPos.y    =   m_WallHeight * z;

                rControl.m_GridPoint    =   new Vector3( x, y, z );

                //  リストにアクセスを登録
                m_rGridList.Add( rControl );
            }
            }
            }

            //  関係情報設定 
            m_GridConnectOK =   false;
            m_GridConnectOK =   SetGrid_Connection();

            //  俯瞰調整
            float   wRatio  =   wholeWidth  / c_FrameWidth;
            float   hRatio  =   wholeHeight / c_FrameHeight;
            float   lRatio  =   ( wRatio > hRatio )? wRatio : hRatio;

            //  カメラ設定更新
            m_rECameraCtrl.m_OlthoMax       =   c_OlthoSize * lRatio;
            m_rECameraCtrl.m_OlthoMax       =   Mathf.Max( m_rECameraCtrl.m_OlthoMin, m_rECameraCtrl.m_OlthoMax );
            m_rEditCamera.orthographicSize  =   m_rECameraCtrl.m_OlthoMax;
        }

        //  グリッド構成用データ破棄
        m_rDataConfig.Clear();

        //  関連オブジェクトを探す
        m_GridAccessTimer   =   0.1f;
    }

    //  ブロックデータを読み込み
    void    LoadBlockData( string _FileName )
    {
        //  拡張子を除く
        _FileName   =   EraseExtension( _FileName );

        //  オブジェクトをロード
        GameObject  rPrefab     =   Resources.Load( c_BlockPath + _FileName ) as GameObject;
        if( rPrefab ){
            m_rUseBlock         =   rPrefab;
            m_UseBlockName      =   _FileName;
        }
    }

    //  オブジェクトのアクセスを更新 
    void    UpdateGridObjectAccess()
    {
        for( int z = 0; z < m_MapDepth;  z++ ){
        for( int y = 0; y < m_MapHeight; y++ ){
        for( int x = 0; x < m_MapWidth;  x++ ){
            GridPanel_Control   rControl    =   GetGrid_FromPoint( x, y, z );
            Vector3             checkPos    =   rControl.m_GridPos + Vector3.up * 0.5f * m_WallHeight;
            Vector3             boxSize     =   ( Vector3.one * c_GridWidth * 0.5f ) * 0.9f;
                                boxSize.y   =   m_WallHeight * 0.5f * 0.9f;
            Collider[]          collider    =   Physics.OverlapBox( checkPos, boxSize );
            if( collider.Length > 0 ){
                rControl.m_rRelatedObj  =   collider[ 0 ].gameObject;
                //  親をチェック
                string  parentName      =   rControl.m_rRelatedObj.transform.parent.name;
                if( parentName.IndexOf( "Home" )   != -1
                ||  parentName.IndexOf( "Play" )   != -1
                ||  parentName.IndexOf( "SlopeB" ) != -1 ){
                    rControl.m_rRelatedObj  =   rControl.m_rRelatedObj.transform.parent.gameObject;
                }
            }
        }
        }
        }
    }

    //  ゲームオブジェクトを保存
    void    SaveToPrefab( GameObject _rObj )
    {
        DateTime    thisDay     =   DateTime.Now;
        string      prefabName  =   ""
                                +   thisDay.Year
                                +   "_" + thisDay.Month.ToString()     .PadLeft( 2, '0' )
                                +   "_" + thisDay.Day.ToString()       .PadLeft( 2, '0' )
                                +   "_" + thisDay.Hour.ToString()      .PadLeft( 2, '0' )
                                +   "_" + thisDay.Minute.ToString()    .PadLeft( 2, '0' )
                                +   "_" + thisDay.Second.ToString()    .PadLeft( 2, '0' );

        SaveToPrefab( _rObj, prefabName );
    }
    void    SaveToPrefab( GameObject _rObj, string _FileName )
    {
        SaveToPrefab( _rObj, _FileName, c_FilePath );
    }
    void    SaveToPrefab( GameObject _rObj, string _FileName, string _FilePath )
    {
#if     UNITY_EDITOR
        //  保存
        PrefabUtility.CreatePrefab( _FilePath + _FileName + ".prefab", _rObj );
#endif
    }

    //  ゲームオブジェクトを保存（軽量版、読み込み不可）
    void    SaveToPrefab_Light( string _FileName )
    {
        //  壁と斜面をリストにまとめる
        List< Transform >   rTransList  =   new List< Transform >();
        for( int i = 0; i < m_rWallShell.childCount; i++ )  rTransList.Add( m_rWallShell.GetChild( i ) );
        for( int i = 0; i < m_rSlopShell.childCount; i++ )  rTransList.Add( m_rSlopShell.GetChild( i ) );

        //  合体させるためのデータを作成
        CombineInstance[]   combine     =   new CombineInstance[ rTransList.Count ];
        for( int i = 0; i < combine.Length; i++ ){
            combine[ i ].mesh       =   rTransList[ i ].GetComponent< MeshFilter >().sharedMesh;
            combine[ i ].transform  =   rTransList[ i ].localToWorldMatrix;
        }

        //  プレハブを作成
        GameObject      saveObj     =   new GameObject( _FileName );
        MeshFilter      rFilter     =   saveObj.AddComponent< MeshFilter >();
        MeshRenderer    rRenderer   =   saveObj.AddComponent< MeshRenderer >();
        MeshCollider    rCollider   =   saveObj.AddComponent< MeshCollider >();

        //  準備開始
        for( int i = 0; i < rTransList.Count; i++ ){
            rTransList[ i ].gameObject.SetActive( false );
        }
        saveObj.SetActive( false );

        //  メッシュをひとつにまとめる
        rFilter.mesh        =   new Mesh();
        rFilter.mesh.name   =   _FileName;
        rFilter.mesh.CombineMeshes( combine, true, true );

        //  マテリアル設定
        rRenderer.material      =   c_StageMaterial;
        //  コライダー設定
        rCollider.sharedMesh    =   rFilter.mesh;

        //  元に戻す
        for( int i = 0; i < rTransList.Count; i++ ){
            rTransList[ i ].gameObject.SetActive( true );
        }
        saveObj.SetActive( true );

#if     UNITY_EDITOR
        //  アセット作成 
        AssetDatabase.CreateAsset( rFilter.mesh, c_MeshPath + rFilter.mesh.name + ".asset" );
		AssetDatabase.SaveAssets();
#endif

        //  保存
        SaveToPrefab( saveObj, _FileName, c_LightPath );

        //  オブジェクト削除
        Destroy( saveObj );
    }

    //  矩形選択から大きな壁を作成
    void    SetLargeWall( List< GridPanel_Control > _rGridList )
    {
        //  各軸の両端を調べる
        Vector3 pointMin    =   new Vector3( m_MapWidth, m_MapHeight, m_MapDepth );
        Vector3 pointMax    =   -Vector3.one;
        for( int i = 0; i < _rGridList.Count; i++ ){
            Vector3 curPoint    =   _rGridList[ i ].m_GridPoint;

            pointMin.x  =   Mathf.Min( curPoint.x, pointMin.x );
            pointMin.y  =   Mathf.Min( curPoint.y, pointMin.y );
            pointMin.z  =   Mathf.Min( curPoint.z, pointMin.z );

            pointMax.x  =   Mathf.Max( curPoint.x, pointMax.x );
            pointMax.y  =   Mathf.Max( curPoint.y, pointMax.y );
            pointMax.z  =   Mathf.Max( curPoint.z, pointMax.z );
        }

        //  サイズを計算
        Vector3 size        =   pointMax - pointMin + Vector3.one;

        //  座標を計算
        Vector3 maxPos      =   GetGrid_FromPoint( pointMax ).m_GridPos;
        Vector3 minPos      =   GetGrid_FromPoint( pointMin ).m_GridPos;
        Vector3 objPos      =   minPos + ( maxPos - minPos ) * 0.5f;
                objPos.y    +=  m_WallHeight * 0.5f;

        //  生成
        {
            //  オブジェクト作成
            GameObject  rObj    =   Instantiate( m_rUseBlock );
            Transform   rTrans  =   rObj.transform;

            //  親の設定
            rTrans.parent       =   ( m_rUseBlock.name.IndexOf( "EB_" ) != -1 )? m_rBoxShell : m_rWallShell;
            
            //  向き設定
            rTrans.eulerAngles  =   new Vector3( 0.0f, m_BlockAngle, 0.0f );

            //  サイズ設定
            Vector3 worldScale  =   new Vector3(
                c_GridWidth  * size.x,
                m_WallHeight * size.z,
                c_GridWidth  * size.y
            );
            Vector3 localScale      =   rTrans.InverseTransformVector( worldScale );
                    localScale.x    =   Mathf.Abs( localScale.x );
                    localScale.y    =   Mathf.Abs( localScale.y );
                    localScale.z    =   Mathf.Abs( localScale.z );
            rTrans.localScale       =   localScale;

            //  座標設定
            rTrans.position         =   objPos;

            //  ＵＶ設定
            //MeshFilter  rFilter     =   rObj.GetComponent< MeshFilter >();
            

            //  パラメータ更新 
            for( int z = ( int )pointMin.z; z <= pointMax.z; z++ ){
            for( int y = ( int )pointMin.y; y <= pointMax.y; y++ ){
            for( int x = ( int )pointMin.x; x <= pointMax.x; x++ ){
                GridPanel_Control   rGrid   =   GetGrid_FromPoint( x, y, z );
                if( !rGrid )        continue;

                rGrid.m_GridType    =   GridPanel_Control.GridType.Wall;
                rGrid.SetRelatedObject( rObj );
            }
            }
            }
        }

        //  選択解除
        SetFocus_All( false );
    }
    //  矩形選択
    void    SetFocusByRectangle( Vector3 _PointA, Vector3 _PointB )
    {
        //  フォーカス解除
        SetFocus_All( false );

        //  両端を調べる
        Vector3 pointMin    =   new Vector3( m_MapWidth, m_MapHeight, m_MapDepth );
        Vector3 pointMax    =   -Vector3.one;
        {
            pointMin.x      =   Mathf.Min( _PointA.x, _PointB.x );
            pointMin.y      =   Mathf.Min( _PointA.y, _PointB.y );
            pointMin.z      =   Mathf.Min( _PointA.z, _PointB.z );

            pointMax.x      =   Mathf.Max( _PointA.x, _PointB.x );
            pointMax.y      =   Mathf.Max( _PointA.y, _PointB.y );
            pointMax.z      =   Mathf.Max( _PointA.z, _PointB.z );
        }

        //  矩形のサイズを計算
        Vector3 size        =   pointMax - pointMin + Vector3.one;
        Vector3 start       =   pointMin;
        Vector3 end         =   pointMin + size;

        //  選択
        for( int z = ( int )start.z; z < ( int )end.z; z++ ){
        for( int y = ( int )start.y; y < ( int )end.y; y++ ){
        for( int x = ( int )start.x; x < ( int )end.x; x++ ){
            GridPanel_Control   rControl    =   GetGrid_FromPoint( x, y, z );
            if( !rControl )     continue;

            SetFocus( rControl, true );
        }
        }
        }
    }
    //  一段上を追加選択
    void    SetFocus_PlusUP()
    {
        List< GridPanel_Control >   rList   =   new List< GridPanel_Control >();
        for( int i = 0; i < m_rFocusList.Count; i++ ){
            rList.Add( m_rFocusList[ i ] );
        }

        for( int i = 0; i < rList.Count; i++ ){
            GridPanel_Control   rTarget =   GetGrid_FromPoint( rList[ i ].m_GridPoint + new Vector3( 0, 0, 1 ) );
            if( !rTarget )      continue;

            SetFocus( rTarget, true );
        }
    }

    //  グリッド生成
    void    ReplaceGrid( int _Width, int _Height, int _Depth )
    {
        //  グリッド削除
        EraseGrid();

        //  サイズ保存
        m_MapWidth      =   _Width;
        m_MapHeight     =   _Height;
        m_MapDepth      =   _Depth;

        //  配置情報計算
        float   wholeWidth  =   c_GridWidth * _Width;
        float   wholeHeight =   c_GridWidth * _Height;
        float   gridLeft    =   c_Center.x - wholeWidth  * 0.5f + c_GridWidth * 0.5f;
        float   gridTop     =   c_Center.z - wholeHeight * 0.5f + c_GridWidth * 0.5f;

        //  グリッド配置
        for( int z = 0; z < _Depth;  z++ ){
        for( int y = 0; y < _Height; y++ ){
        for( int x = 0; x < _Width;  x++ ){
            GameObject  rObj    =   Instantiate( c_GridPanel );
            Transform   rTrans  =   rObj.transform;

            //  親を設定
            rTrans.parent       =   m_rGridShell;

            //  名前設定
            rObj.name           =   "GridPanel_( "
                                +   "X_" + x.ToString().PadLeft( 3, '_' ) + ", "
                                +   "Y_" + y.ToString().PadLeft( 3, '_' ) + ", "
                                +   "H_" + z.ToString().PadLeft( 3, '_' ) + " )";

            //  サイズ設置
            rTrans.localScale   =   Vector3.one * c_GridWidth * c_GridScale;

            //  座標設定
            rTrans.position     =   new Vector3(
                gridLeft + c_GridWidth * x,
                c_PlaceHeight,
                gridTop  + c_GridWidth * y
            );

            //  パラメータ設定
            GridPanel_Control   rControl    =   rObj.GetComponent< GridPanel_Control >();
            rControl.m_GridPos      =   rTrans.position;
            rControl.m_GridPos.y    =   m_WallHeight * z;

            rControl.m_GridPoint    =   new Vector3( x, y, z );

            //  リストにアクセスを登録
            m_rGridList.Add( rControl );
        }
        }
        }

        //  関係情報設定
        m_GridConnectOK =   false;
        m_GridConnectOK =   SetGrid_Connection();

        //  グリッド初期化
        for( int y = 0; y < _Height; y++ ){
        for( int x = 0; x < _Width;  x++ ){
            GridPanel_Control   rControl    =   GetGrid_FromPoint( x, y, m_MapDepth - 1 );
            rControl.SetActive_Recursion( false );
        }
        }

        //  床サイズ更新
        m_rMapFloor.localScale  =   new Vector3(
            wholeWidth,
            wholeHeight,
            1.0f
        );

        //  俯瞰調整
        float   wRatio  =   wholeWidth  / c_FrameWidth;
        float   hRatio  =   wholeHeight / c_FrameHeight;
        float   lRatio  =   ( wRatio > hRatio )? wRatio : hRatio;

        //  カメラ設定更新
        m_rECameraCtrl.m_OlthoMax       =   c_OlthoSize * lRatio;
        m_rECameraCtrl.m_OlthoMax       =   Mathf.Max( m_rECameraCtrl.m_OlthoMin, m_rECameraCtrl.m_OlthoMax );
        m_rEditCamera.orthographicSize  =   m_rECameraCtrl.m_OlthoMax;
    }
    //  グリッド削除
    void    EraseGrid()
    {
        //  グリッドをクリア
        Erase_All();
        
        //  グリッド削除
        foreach( Transform rTrans in m_rGridShell ){
            Destroy( rTrans.gameObject );
        }
        
        //  リストクリア
        m_rGridList.Clear();
    }

    //  グリッド操作
    bool    SetGrid_Connection()
    {
        //  関係情報設定
        bool    connectionOK    =   true;
        for( int z = 0; z < m_MapDepth;  z++ ){
        for( int y = 0; y < m_MapHeight; y++ ){
        for( int x = 0; x < m_MapWidth;  x++ ){
            GridPanel_Control   rControl    =   GetGrid_FromPoint( x, y, z );
            GridPanel_Control   rUP         =   GetGrid_FromPoint( x, y, z + 1 );
            GridPanel_Control   rDOWN       =   GetGrid_FromPoint( x, y, z - 1 );

            if( !rUP   && z != m_MapDepth - 1 ) connectionOK    =   false;
            if( !rDOWN && z != 0 )              connectionOK    =   false;
            if( !rControl )                     continue;

            rControl.m_rUP      =   rUP;
            rControl.m_rDOWN    =   rDOWN;
        }
        }
        }

        return  connectionOK;
    }

    //  指定座標のグリッドを取得
    GridPanel_Control   GetGrid_FromPoint( Vector3 _Point )
    {
        return  GetGrid_FromPoint( ( int )_Point.x, ( int )_Point.y, ( int )_Point.z );
    }
    GridPanel_Control   GetGrid_FromPoint( int _X, int _Y, int _Z )
    {
        //  アクセス
        int index   =   GetGrid_Index( _X, _Y, _Z );

        //  範囲外チェック
        if( index < 0 )                     return  null;
        if( index > m_rGridList.Count - 1 ) return  null;

        return  m_rGridList[ index ];
    }
    int                 GetGrid_Index( int _X, int _Y, int _Z )
    {
        return  _Z * ( m_MapWidth * m_MapHeight )
            +   _Y * ( m_MapWidth )
            +   _X;
    }

    GridPanel_Control   FindGrid_FromPoint( Vector3 _Point )
    {
        for( int i = 0; i < m_rGridList.Count; i++ ){
            GridPanel_Control   rControl    =   m_rGridList[ i ];
            if( rControl.m_GridPoint.x != _Point.x )    continue;
            if( rControl.m_GridPoint.y != _Point.y )    continue;
            if( rControl.m_GridPoint.z != _Point.z )    continue;

            return  rControl;
        }

        return  null;
    }
    GridPanel_Control   FindGrid_FromPoint( int _X, int _Y, int _Z )
    {
        return  FindGrid_FromPoint( new Vector3( _X, _Y, _Z ) );
    }

    //  ステージを拡張
    void    ExtensionMap( Vector3 _ExtensionSize, Vector3 _PointOffset )
    {
        ExtensionMap( _ExtensionSize, _PointOffset, Vector3.zero );
    }
    void    ExtensionMap( Vector3 _ExtensionSize, Vector3 _PointOffset, Vector3 _PanelOffset )
    {
        //  幅が１以下にならないようにする
        if( m_MapWidth  + _ExtensionSize.x < 1 )    return;
        if( m_MapHeight + _ExtensionSize.y < 1 )    return;
        if( m_MapDepth  + _ExtensionSize.z < 1 )    return;

        //  フォーカスを解除
        SetFocus_All( false );

        //  新しいサイズを計算
        int         newWidth    =   m_MapWidth  + ( int )_ExtensionSize.x;
        int         newHeight   =   m_MapHeight + ( int )_ExtensionSize.y;
        int         newDepth    =   m_MapDepth  + ( int )_ExtensionSize.z;

        //  新しい親を生成
        List< GridPanel_Control >   rNewList        =   new List< GridPanel_Control >();
        GameObject                  newShellObj     =   new GameObject( "Grid_Shell" );
        Transform                   newShellTrans   =   newShellObj.transform;
        
        //  グリッド作成
        {
            //  配置情報計算
            float   wholeWidth  =   c_GridWidth * newWidth;
            float   wholeHeight =   c_GridWidth * newHeight;
            float   gridLeft    =   c_Center.x - wholeWidth  * 0.5f + c_GridWidth * 0.5f;
            float   gridTop     =   c_Center.z - wholeHeight * 0.5f + c_GridWidth * 0.5f;

            //  オブジェクト移動
            {
                //  補正量計算
                Vector3 offset  =   -new Vector3(
                    ( wholeWidth  - ( c_GridWidth * m_MapWidth ) )  * 0.5f,
                    0.0f,
                    ( wholeHeight - ( c_GridWidth * m_MapHeight ) ) * 0.5f
                );
                offset.x    -=  c_GridWidth * _PointOffset.x;
                offset.z    -=  c_GridWidth * _PointOffset.y;
                offset.y    -=  c_GridWidth * _PointOffset.z;

                //  移動
                MoveChildren( m_rWallShell, offset );
                MoveChildren( m_rSlopShell, offset );
                MoveChildren( m_rBaseShell, offset );
                MoveChildren( m_rBoxShell, offset );
                MoveChildren( m_rLaunchPointShell, offset );
                MoveChildren( m_rSpawnPointShell, offset );
                MoveChildren( m_rEmptyShell, offset );
            }

            //  グリッド配置
            for( int z = 0; z < newDepth;  z++ ){
            for( int y = 0; y < newHeight; y++ ){
            for( int x = 0; x < newWidth;  x++ ){
                GameObject  rObj    =   Instantiate( c_GridPanel );
                Transform   rTrans  =   rObj.transform;

                //  親を設定
                rTrans.parent       =   newShellTrans;

                //  名前設定
                rObj.name           =   "GridPanel_( "
                                    +   "X_" + x.ToString().PadLeft( 3, '_' ) + ", "
                                    +   "Y_" + y.ToString().PadLeft( 3, '_' ) + ", "
                                    +   "H_" + z.ToString().PadLeft( 3, '_' ) + " )";

                //  サイズ設置
                rTrans.localScale   =   Vector3.one * c_GridWidth * c_GridScale;

                //  座標設定
                rTrans.position     =   new Vector3(
                    gridLeft + c_GridWidth * x,
                    c_PlaceHeight,
                    gridTop  + c_GridWidth * y
                );

                //  パラメータ設定
                GridPanel_Control   rControl    =   rObj.GetComponent< GridPanel_Control >();
                GridPanel_Control   rSource     =   FindGrid_FromPoint( new Vector3( x, y, z ) + _PointOffset + _PanelOffset );
                    //  同じマスがあればコピー
                    if( rSource ){
                        rControl.CopyParam( rSource );
                        rControl.m_IsActive =   true;

                        //  リストから削除
                        m_rGridList.Remove( rSource );
                    }

                rControl.m_GridPos      =   rTrans.position;
                rControl.m_GridPos.y    =   m_WallHeight * z;

                rControl.m_GridPoint    =   new Vector3( x, y, z );

                //  リストにアクセスを登録
                rNewList.Add( rControl );

                //  押し上げる場合は最下層にブロックを設置
                if( _ExtensionSize.z > 0 
                &&  _PointOffset.z   < 0 ){
                    //  何段上げるか
                    int     numUP   =   ( int )_ExtensionSize.z;
                    if( z < numUP ){
                        //  設置
                        SetWall( rControl );
                    }
                }
            }
            }
            }

            //  コピー先がなかったパネルの関連オブジェクトは削除する 
            for( int i = 0; i < m_rGridList.Count; i++ ){
                if( m_rGridList[ i ].m_rRelatedObj ){
                    Destroy( m_rGridList[ i ].m_rRelatedObj );
                }
            }

            //  パラメータ更新
            m_MapWidth      =   newWidth;
            m_MapHeight     =   newHeight;
            m_MapDepth      =   newDepth;

            //  古いアクセスを破棄
            Destroy( m_rGridShell.gameObject );

            //  アクセスを更新
            m_rGridShell    =   newShellTrans;
            m_rGridList     =   rNewList;

            //  関係情報設定
            m_GridConnectOK =   false;
            m_GridConnectOK =   SetGrid_Connection();

            //  グリッド初期化
            for( int y = 0; y < newHeight; y++ ){
            for( int x = 0; x < newWidth;  x++ ){
                GridPanel_Control   rControl    =   GetGrid_FromPoint( x, y, newDepth - 1 );
                rControl.SetActive_Recursion( false );
            }
            }

            //  床サイズ更新
            m_rMapFloor.localScale  =   new Vector3(
                wholeWidth,
                wholeHeight,
                1.0f
            );

            //  俯瞰調整
            float   wRatio  =   wholeWidth  / c_FrameWidth;
            float   hRatio  =   wholeHeight / c_FrameHeight;
            float   lRatio  =   ( wRatio > hRatio )? wRatio : hRatio;

            //  カメラ設定更新
            m_rECameraCtrl.m_OlthoMax       =   c_OlthoSize * lRatio;
            m_rECameraCtrl.m_OlthoMax       =   Mathf.Max( m_rECameraCtrl.m_OlthoMin, m_rECameraCtrl.m_OlthoMax );
            m_rEditCamera.orthographicSize  =   m_rECameraCtrl.m_OlthoMax;
        }
    }

    //  子をすべて移動
    void    MoveChildren( Transform _rParent, Vector3 _Offset )
    {
        for( int i = 0; i < _rParent.childCount; i++ ){
            _rParent.GetChild( i ).position +=  _Offset;
        }
    }

    //  ブラシに応じてペイント
    void    PaintGrid( GridPanel_Control _rPanel )
    {
        //  モードに応じた処理を行う
        PaintGrid( _rPanel, m_EditMode );
    }
    void    PaintGrid( GridPanel_Control _rPanel, EditMode _EditMode )
    {
        //  モードに応じた処理を行う
        switch( _EditMode ){                        
            case    EditMode.Wall:          SetWall( _rPanel );         break;
            case    EditMode.Base:          SetBase( _rPanel );         break;
            case    EditMode.SpawnPoint:    SetSpawnPoint( _rPanel );   break;
            case    EditMode.LaunchPoint:   SetLaunchPoint( _rPanel );  break;
            case    EditMode.Empty:         SetEmptyBox( _rPanel );     break;
            //case    EditMode.NavMesh:       break;
            case    EditMode.Select:        SetFocus( _rPanel, true );  break;
        }
    }

    //  選択部分を塗る
    void    PaintGrid_Focus( EditMode _EditMode )
    {
        //  あとで一段上を取得するために現在のフォーカス位置を取得しておく
        List< Vector3 > rFocusPoint =   new List< Vector3 >();
        for( int i = 0; i < m_rFocusList.Count; i++ ){
            rFocusPoint.Add( m_rFocusList[ i ].m_GridPoint );
        }

        //  塗る
        for( int i = 0; i < m_rFocusList.Count; i++ ){
            PaintGrid( m_rFocusList[ i ], _EditMode );
        }

        //  フォーカス解除
        SetFocus_All( false );

        //  一段上をフォーカス
        for( int i = 0; i < rFocusPoint.Count; i++ ){
            int     upHeight    =   ( _EditMode == EditMode.Base )? 4 : 1;
            Vector3 point       =   rFocusPoint[ i ] + new Vector3( 0, 0, upHeight );
                    point.z     =   Mathf.Min( point.z, m_MapDepth - 1 );
        
            //  フォーカスをセット
            SetFocus( GetGrid_FromPoint( point ), true );
        }
    }

    //  補間して塗る
    void    LerpPaint( Vector3 _PrevPos, Vector3 _CurPos )
    {
        float   interval    =   5.0f;
        Vector3 vToCur      =   _CurPos - _PrevPos;
        float   toCurDist   =   vToCur.magnitude;
        int     numCheck    =   ( int )( toCurDist / interval );
        for( int i = 1; i < numCheck; i++ ){
            //  触れているパネルを取得
            GridPanel_Control   rControl    =   GetHitPanel_InGrid( _PrevPos + vToCur.normalized * interval * i );
            if( rControl
            &&  CheckOverlapWindow() == false ){
                //  前回塗ったマスには塗らない
                if( rControl.m_GridPoint.x != m_PrevPoint.x
                ||  rControl.m_GridPoint.y != m_PrevPoint.y ){
                    if( Input.GetMouseButton( 0 ) ) PaintGrid( rControl );
                    if( Input.GetMouseButton( 1 ) ) Erase( rControl );
                }
        
                m_PrevPoint =   rControl.m_GridPoint;
            }
        }
    }

    //  拠点の配置
    void    SetBase( GridPanel_Control _rPanel )
    {
        //  既に拠点が配置されている場合は処理を行わない
        if( _rPanel.m_GridType == GridPanel_Control.GridType.Base ) return;

        //  オブジェクト作成
        GameObject  rObj    =   Instantiate( c_Base );
        Transform   rTrans  =   rObj.transform;

        //  親の設定
        rTrans.parent       =   m_rBaseShell;
        
        //  座標設定
        Vector3 panelPos    =   _rPanel.transform.position;
        rTrans.position     =   new Vector3(
            panelPos.x,
            m_WallHeight * _rPanel.m_GridPoint.z,
            panelPos.z
        );

        //  パラメータ更新
        int     baseHeigh   =   ( int )( 12 / m_WallHeight );
        Vector3 bottom      =   _rPanel.m_GridPoint;
        for( int h = 0; h < baseHeigh; h++ ){
            GridPanel_Control   rControl    =   GetGrid_FromPoint( bottom + new Vector3( 0, 0, h ) );
            if( !rControl )     continue;
            rControl.m_GridType  =   GridPanel_Control.GridType.Base;
            rControl.SetRelatedObject( rObj );
        }
    }
    void    SetBase_All()
    {
        for( int i = 0; i < m_rGridShell.childCount; i++ ){
            GridPanel_Control   rControl    =   m_rGridShell.GetChild( i ).GetComponent< GridPanel_Control >();

            //  拠点をセット
            SetBase( rControl );
        }
    }

    //  出現地点の配置
    void    SetSpawnPoint( GridPanel_Control _rPanel )
    {
        //  既に拠点が配置されている場合は処理を行わない
        if( _rPanel.m_GridType == GridPanel_Control.GridType.SpawnPoint )   return;

        //  オブジェクト作成
        GameObject  rObj    =   Instantiate( c_SpawnPoint );
        Transform   rTrans  =   rObj.transform;
        
        //  親の設定
        rTrans.parent       =   m_rSpawnPointShell;

        //  座標設定
        Vector3 panelPos    =   _rPanel.transform.position;
        rTrans.position     =   new Vector3(
            panelPos.x,
            rTrans.localScale.y * 0.5f + m_WallHeight * _rPanel.m_GridPoint.z,
            panelPos.z
        );

        //  パラメータ更新
        _rPanel.m_GridType  =   GridPanel_Control.GridType.SpawnPoint;
        _rPanel.SetRelatedObject( rObj );
    }
    void    SetSpawnPoint_All()
    {
        for( int i = 0; i < m_rGridShell.childCount; i++ ){
            GridPanel_Control   rControl    =   m_rGridShell.GetChild( i ).GetComponent< GridPanel_Control >();

            //  出現地点をセット
            SetSpawnPoint( rControl );
        }
    }

    //  出撃地点の配置
    void    SetLaunchPoint( GridPanel_Control _rPanel )
    {
        //  既に拠点が配置されている場合は処理を行わない
        if( _rPanel.m_GridType == GridPanel_Control.GridType.LaunchPoint )  return;

        //  オブジェクト作成
        GameObject  rObj    =   Instantiate( c_LaunchPoint );
        Transform   rTrans  =   rObj.transform;
        
        //  親の設定
        rTrans.parent       =   m_rLaunchPointShell;

        //  座標設定
        Vector3 panelPos    =   _rPanel.transform.position;
        rTrans.position     =   new Vector3(
            panelPos.x,
            m_WallHeight * _rPanel.m_GridPoint.z,
            panelPos.z
        );

        //  パラメータ更新
        _rPanel.m_GridType  =   GridPanel_Control.GridType.LaunchPoint;
        _rPanel.SetRelatedObject( rObj );
    }

    //  壁の配置
    void    SetWall( GridPanel_Control _rPanel )
    {
        //  既に壁が配置されている場合は処理を行わない
        if( _rPanel.m_GridType == GridPanel_Control.GridType.Wall ) return;

        //  オブジェクト作成
        GameObject  rObj    =   Instantiate( m_rUseBlock );
        Transform   rTrans  =   rObj.transform;

        //  親の設定
        rTrans.parent       =   ( m_rUseBlock.name.IndexOf( "EB_" ) != -1 )? m_rBoxShell : m_rWallShell;
        
        //  サイズ設定
        rTrans.localScale   =   new Vector3(
            c_GridWidth,
            m_WallHeight,
            c_GridWidth
        );

        //  向き設定
        rTrans.eulerAngles  =   new Vector3( 0.0f, m_BlockAngle, 0.0f );

        //  座標設定
        Vector3 panelPos    =   _rPanel.transform.position;
        rTrans.position     =   new Vector3(
            panelPos.x,
            m_WallHeight * 0.5f + m_WallHeight * _rPanel.m_GridPoint.z,
            panelPos.z
        );

        //  パラメータ更新
        _rPanel.m_GridType  =   GridPanel_Control.GridType.Wall;
        _rPanel.SetRelatedObject( rObj );
        _rPanel.SetActive_Recursion( true );
    }
    void    SetWall_All()
    {
        for( int i = 0; i < m_rGridShell.childCount; i++ ){
            GridPanel_Control   rControl    =   m_rGridShell.GetChild( i ).GetComponent< GridPanel_Control >();

            //  壁をセット
            SetWall( rControl );
        }
    }

    //  空のボックスを配置
    void    SetEmptyBox( GridPanel_Control _rPanel )
    {
        //  既に配置されている場合は処理を行わない
        if( _rPanel.m_GridType == GridPanel_Control.GridType.Empty )    return;

        //  オブジェクト作成
        GameObject  rObj    =   Instantiate( c_EmptyBlock );
        Transform   rTrans  =   rObj.transform;
        
        //  親の設定
        rTrans.parent       =   m_rEmptyShell;

        //  サイズ設定
        rTrans.localScale   =   new Vector3(
            c_GridWidth,
            m_WallHeight,
            c_GridWidth
        );

        //  座標設定
        Vector3 panelPos    =   _rPanel.transform.position;
        rTrans.position     =   new Vector3(
            panelPos.x,
            m_WallHeight * 0.5f + m_WallHeight * _rPanel.m_GridPoint.z,
            panelPos.z
        );

        //  パラメータ更新
        _rPanel.m_GridType  =   GridPanel_Control.GridType.Empty;
        _rPanel.SetRelatedObject( rObj );
        _rPanel.SetActive_Recursion( true );

        //  可視設定
        Renderer    rRenderer   =   rObj.GetComponent< Renderer >();
        rRenderer.enabled       =   m_VisibleEmpty;
    }
    void    SetEmptyBox_Visible( bool _Visible )
    {
        for( int i = 0; i < m_rEmptyShell.childCount; i++ ){
            Transform   rTrans      =   m_rEmptyShell.GetChild( i );
            Renderer    rRenderer   =   rTrans.GetComponent< Renderer >();

            rRenderer.enabled       =   _Visible;
        }
    }

    //  斜面の配置
    void    SetSlope( List< GridPanel_Control > _rPanelList )
    {
        //  連続するパネルごとにリストを分割
        List< List< GridPanel_Control >  >  rGridListList   =   new List< List< GridPanel_Control > >();
        {
            //  最初のリストを用意
            rGridListList.Add( new List< GridPanel_Control >() );

            //  現在使用中のリストが何番目か
            int     curChain    =   0;

            //  チェック開始
            float   prevAngle   =   0;
            bool    newChain    =   true;
            for( int i = 1; i < _rPanelList.Count; i++ ){
                GridPanel_Control   rPrev   =   _rPanelList[ i - 1 ];
                GridPanel_Control   rCur    =   _rPanelList[ i - 0 ];
                float               angle   =   CheckAngle_GridPoint( rPrev.m_GridPoint, rCur.m_GridPoint );

                //  連続の初めは角度判定を無視
                if( newChain ){
                    prevAngle   =   angle;
                    newChain    =   false;
                }

                //  連続している場合はリストに登録
                if( CheckWhetherVertical_GridContact( rPrev.m_GridPoint, rCur.m_GridPoint )
                &&  Mathf.Abs( angle - prevAngle ) < 0.01f ){
                    rGridListList[ curChain ].Remove( rPrev );
                    rGridListList[ curChain ].Add( rPrev );
                    rGridListList[ curChain ].Add( rCur );
                }
                //  連続が途切れたら次の連続リストを生成
                else{
                    rGridListList.Add( new List< GridPanel_Control >() );
                    
                    newChain    =   true;
                    curChain    =   curChain + 1;
                }

                //  角度を保存
                prevAngle   =   angle;
            }
        }

        //  連続するリスト数分ループ
        for( int i = 0; i < rGridListList.Count; i++ ){
            List< GridPanel_Control >   rCurList    =   rGridListList[ i ];
            if( rCurList.Count < 2 )    continue;

            //  終点が開始地点より高ければ向きを逆にする
            if( rCurList[ rCurList.Count - 1 ].m_GridPoint.z
            <   rCurList[ 0 ].m_GridPoint.z ){
                rCurList.Reverse();
            }

            //  連続するパネルから向きと長さを計算
            GridPanel_Control   rFirst      =   rCurList[ 0 ];
            GridPanel_Control   rLast       =   rCurList[ rCurList.Count - 1 ];
            GridPanel_Control   rSecLast    =   rCurList[ rCurList.Count - 2 ];
            int     length      =   rCurList.Count - 1;
            float   angle       =   CheckAngle_GridPoint( rFirst.m_GridPoint, rLast.m_GridPoint );
            
            //  高さを計算
            int     height      =   Mathf.Max( 1, ( int )( rLast.m_GridPoint.z - rFirst.m_GridPoint.z ) );
            if( rLast.m_rUP == null
            &&  m_MapDepth  >= 2    )   ++height;
            Vector3 objPos      =   rFirst.m_GridPos + ( rSecLast.m_GridPos - rFirst.m_GridPos ) * 0.5f;
                    objPos.y    =   rFirst.m_GridPos.y + m_WallHeight * height * 0.5f;


            //  生成
            {
                //  オブジェクト作成
                GameObject  rObj    =   Instantiate( c_SlopeBlock );
                Transform   rTrans  =   rObj.transform;

                //  親の設定
                rTrans.parent       =   m_rSlopShell;
                
                //  サイズ設定
                rTrans.localScale   =   new Vector3(
                    c_GridWidth,
                    m_WallHeight * height,
                    c_GridWidth * length
                );

                //  向き設定
                rTrans.eulerAngles  =   new Vector3( 0.0f, angle + 180.0f, 0.0f );

                //  座標設定
                rTrans.position     =   objPos;

                //  パラメータ更新
                int bottomHeight    =   ( int )rFirst.m_GridPoint.z;
                for( int p = 0; p < rCurList.Count - 1; p++ ){
                    //  上方向のマスも高さ分だけ埋める
                    Vector3 bottom      =   rCurList[ p ].m_GridPoint;
                            bottom.z    =   bottomHeight;
                    for( int h = 0; h < height; h++ ){
                        GridPanel_Control   rControl    =   GetGrid_FromPoint( bottom + new Vector3( 0, 0, h ) );
                        if( !rControl )     continue;

                        rControl.m_GridType    =   GridPanel_Control.GridType.Slope;
                        rControl.SetRelatedObject( rObj );
                    }
                }
            }
        }
    }
    //  グリッド座標が隣接しているかどうか
    bool    CheckWhetherVertical_GridContact( Vector3 _PointA, Vector3 _PointB )
    {
        //  距離が離れていないかチェック
        if( Mathf.Abs( ( int )( _PointA.x - _PointB.x ) ) > 1 ) return  false;
        if( Mathf.Abs( ( int )( _PointA.y - _PointB.y ) ) > 1 ) return  false;

        //  垂直かどうかをチェック
        if( _PointA.x == _PointB.x )                            return  true;
        if( _PointA.y == _PointB.y )                            return  true;

        //  斜め
        return  false;
    }
    float   CheckAngle_GridPoint( Vector3 _PointA, Vector3 _PointB )
    {
        Vector2 vToB    =   _PointA - _PointB;
                vToB    =   vToB.normalized;

        return  Mathf.Atan2( vToB.x, vToB.y ) * Mathf.Rad2Deg;
    }
    
    //  削除
    void    Erase( GridPanel_Control _rPanel )
    {
        //  フォーカス解除
        SetFocus( _rPanel, false );

        //  対象へのアクセス
        GridPanel_Control   rTarget =   _rPanel;
        //  このマスが空なら下を対象にする
        if( !_rPanel.m_rRelatedObj && _rPanel.m_rDOWN ) rTarget =   _rPanel.m_rDOWN;

        //  パラメータ更新
        rTarget.m_GridType  =   GridPanel_Control.GridType.None;
        rTarget.SetRelatedObject( null );

        //  最前面まで降りる
        rTarget.SetActive_Recursion( false );
    }
    void    Erase_Focus()
    {
        //  現在のフォーカス位置を取得しておく
        List< Vector3 > rFocusPoint =   new List< Vector3 >();
        for( int i = 0; i < m_rFocusList.Count; i++ ){
            rFocusPoint.Add( m_rFocusList[ i ].m_GridPoint );
        }

        //  削除
        int     loopCount   =   m_rFocusList.Count;
        for( int i = 0; i < loopCount; i++ ){
            Erase( m_rFocusList[ 0 ] );
        }

        //  フォーカス解除
        SetFocus_All( false );

        //  直下にある最前面のグリッドにフォーカスをセット
        for( int i = 0; i < rFocusPoint.Count; i++ ){
            Vector3 point       =   rFocusPoint[ i ];
            int     checkCount  =   ( int )point.z;
            for( int h = 0; h <= checkCount; h++ ){
                GridPanel_Control   rGrid   =   GetGrid_FromPoint( point - new Vector3( 0, 0, h ) );
                if( !rGrid.m_rDOWN
                ||   rGrid.m_rDOWN.m_rRelatedObj ){
                    SetFocus( rGrid, true );
                    break;
                }
            }
        }
    }
    void    Erase_Vertical( GridPanel_Control _rPanel )
    {
        Vector3 bottom      =   _rPanel.m_GridPoint;
                bottom.z    =   0;
        for( int  h = 0; h < m_MapDepth; h++ ){
            GridPanel_Control   rControl    =   GetGrid_FromPoint( bottom + new Vector3( 0, 0, h ) );
            if( !rControl )     continue;

            Erase( rControl );
        }
    }
    void    Erase_All()
    {
        for( int i = 0; i < m_rGridShell.childCount; i++ ){
            GridPanel_Control   rControl    =   m_rGridShell.GetChild( i ).GetComponent< GridPanel_Control >();

            //  削除
            Erase( rControl );
        }
    }

    //  塗りつぶし
    void    FillGrid()
    {
        switch( m_EditMode ){
            case    EditMode.Wall:          SetWall_All();          break;
            case    EditMode.Base:          SetBase_All();          break;
            case    EditMode.SpawnPoint:    SetSpawnPoint_All();    break;
            //case    EditMode.NavMesh:       break;
            case    EditMode.Select:        break;
        }
    }
    void    FillGrid_Vertical( GridPanel_Control _rPanel )
    {
        FillGrid_Vertical( _rPanel, m_EditMode );
    }
    void    FillGrid_Vertical( GridPanel_Control _rPanel, EditMode _EditMode )
    {
        //  選択モードでは使用しない
        if( _EditMode == EditMode.Select )  return;

        Vector3 bottom      =   _rPanel.m_GridPoint;
                bottom.z    =   0;
        for( int h = 0; h < m_MapDepth; h++ ){
            GridPanel_Control   rControl    =   GetGrid_FromPoint( bottom + new Vector3( 0, 0, h ) );
            if( !rControl )     continue;

            PaintGrid( rControl, _EditMode );
        }
    }

    //  フォーカス操作
    void    SetFocus( GridPanel_Control _rPanel, bool _Focus )
    {
        if( !_rPanel )  return;
        
        //  フォーカス追加
        if( _Focus ){
            //  同じ要素がすでに登録されている場合は削除
            m_rFocusList.Remove( _rPanel );

            //  アクセスを追加
            m_rFocusList.Add( _rPanel );

            //  フォーカス色を設定
            _rPanel.SetColor( c_FocusColor );
        }
        //  フォーカス解除
        else{
            //  通常色を設定
            _rPanel.SetColor( c_DefaultColor );

            //  アクセスを削除
            m_rFocusList.Remove( _rPanel );
        }
    }
    void    SetFocus_All( bool _Focus )
    {
        //  フォーカス追加
        if( _Focus ){
            //  すべてのグリッドを選択
            for( int i = 0; i < m_rGridShell.childCount; i++ ){
                GridPanel_Control   rControl    =   m_rGridShell.GetChild( i ).GetComponent< GridPanel_Control >();

                //  フォーカスをセット
                SetFocus( rControl, true );
            }
        }
        //  フォーカス解除
        else{
            int loopCount   =   m_rFocusList.Count;
            for( int i = 0; i < loopCount; i++ ){
                //  フォーカス解除
                SetFocus( m_rFocusList[ 0 ], false );
            }
        }
    }

    //  ゲームオブジェクト作成
    Transform   CreateEmptyObject( string _ParentName, string _Name )
    {
        GameObject  rObj    =   new GameObject( _Name );
        Transform   rTrans  =   rObj.transform;
        GameObject  rParent =   GameObject.Find( _ParentName );
        if( rParent ){
            rTrans.SetParent( rParent.transform );
        }

        return  rTrans;
    }

    //  配置する向きを変更
    void    AddPlaceBlockAngle( int _AddAngle )
    {
        m_BlockAngle    +=  _AddAngle;
        m_BlockAngle    =   m_BlockAngle % 360;
    }
    //  選択のサイズを計算
    Vector3 CalcFocusSize( List< GridPanel_Control > _rList )
    {
        //  項目がなければサイズは０
        if( _rList.Count == 0 ) return  Vector3.zero;

        //  各軸の両端を調べる
        Vector3 pointMin    =   new Vector3( m_MapWidth, m_MapHeight, m_MapDepth );
        Vector3 pointMax    =   -Vector3.one;
        for( int i = 0; i < _rList.Count; i++ ){
            Vector3 curPoint    =   _rList[ i ].m_GridPoint;

            pointMin.x  =   Mathf.Min( curPoint.x, pointMin.x );
            pointMin.y  =   Mathf.Min( curPoint.y, pointMin.y );
            pointMin.z  =   Mathf.Min( curPoint.z, pointMin.z );

            pointMax.x  =   Mathf.Max( curPoint.x, pointMax.x );
            pointMax.y  =   Mathf.Max( curPoint.y, pointMax.y );
            pointMax.z  =   Mathf.Max( curPoint.z, pointMax.z );
        }

        //  サイズを計算
        Vector3 size        =   pointMax - pointMin + Vector3.one;

        return  size;
    }
    //  拡張子を消す
    string  EraseExtension( string _String )
    {
        //  拡張子がついている場合は消す
        if( _String.IndexOf( '.' ) != -1 ){
            string  ext =   Path.GetExtension( _String );
            _String     =   _String.Substring( 0, _String.Length - ext.Length );
        }

        return  _String;
    }

//===========================================================================
//      プレイテスト関連
//===========================================================================
    //  テスト開始
    void    PT_StartPlayTest()
    {
#if     UNITY_EDITOR
        //  ナビメッシュ作成
        NavMeshBuilder.BuildNavMesh();
#endif

        //  プレイテスト用データ作成
        {
            //  古いデータを破棄
            if( m_rPlayTestShell ){
                Destroy( m_rPlayTestShell.gameObject );
            }

            //  作成
            GameObject  rPTShellObj =   Instantiate( c_PlayTestShell );
            m_rPlayTestShell    =   rPTShellObj.transform;
        }

        //  敵の出現地点設定
        PT_SetEnemySpawnPoint( m_rPlayTestShell );
        //  拠点を設定
        PT_SetHomeBase( m_rPlayTestShell );

        //  プレイ開始
        m_rNetworkManager.StartHost();

        //  グリッド無効化 
        m_rGridShell.gameObject.SetActive( false );

        //  RTSグリッドの調整
        {
            Transform   rRITrans =   m_rPlayTestShell.FindChild( "ResourceInformation" );
            if( rRITrans ){
                rRITrans.GetComponent< ResourceInformation >().m_GridOffset -=  new Vector3(
                    ( m_MapWidth  % 2 ) * 1.5f,
                    0.0f,
                    ( m_MapHeight % 2 ) * 1.5f
                 );
            }
        }

        //  パラメータ更新
        m_PlayTest  =   true;
    }
    //  テスト終了
    void    PT_EndPlayTest()
    {
        //  プレイヤーが出撃している場合は戻す
        {
            Transform   rLinkTrans      =   m_rPlayTestShell.FindChild( "LinkManager" );
            LinkManager rLinkManager    =   rLinkTrans.GetComponent< LinkManager >();

            rLinkManager.m_rLocalNPControl.ChangeToCommander( false );
        }

        //  ネットワーク終了
        m_rNetworkManager.StopHost();

        //  テスト用データ破棄
        if( m_rPlayTestShell ){
            Destroy( m_rPlayTestShell.gameObject );
        }

        //  グリッド有効化
        m_rGridShell.gameObject.SetActive( true );

        //  パラメータ更新
        m_PlayTest  =   false;
    }

    //  敵の出現地点を設定
    void    PT_SetEnemySpawnPoint( Transform _rPlayTestShell )
    {
        //  スポーン地点の親へのアクセス
        Transform   rShellTrans     =   _rPlayTestShell.FindChild( "EnemySpawnRoot" );
        if( !rShellTrans )      return;

        //  現在の出現地点を破棄
        foreach( Transform rChild in rShellTrans ){
            Destroy( rChild.gameObject );
        }

        //  出現地点を設定
        List< Transform >   rSpawnPointList =   new List< Transform >();
        for( int i = 0; i < m_rSpawnPointShell.childCount; i++ ){
            //  編集データの出現地点
            Transform   rEditSP =   m_rSpawnPointShell.GetChild( i );

            //  プレイテスト用の出現地点
            GameObject  rObj    =   Instantiate( c_EnemySpawnPoint );
            Transform   rTrans  =   rObj.transform;

            rTrans.parent       =   rShellTrans;
            rTrans.position     =   rEditSP.position;
            rTrans.rotation     =   rEditSP.rotation;

            //  リストに登録
            rSpawnPointList.Add( rTrans );
        }

        //  スポーン地点更新
        EnemyGenerator  rGenerator  =   rShellTrans.GetComponent< EnemyGenerator >();
        rGenerator.InitializeSpawnPoint( rSpawnPointList );
    }
    //  拠点を設定
    void    PT_SetHomeBase( Transform _rPlayTestShell )
    {
        //  生成済みの拠点を削除
        {
            //  削除リスト作成
            List< GameObject >  eraseList   =   new List< GameObject >();
            for( int i = 0; i < _rPlayTestShell.childCount; i++ ){
                Transform   rTrans      =   _rPlayTestShell.GetChild( i );
                if( rTrans.name.IndexOf( "Homebase" ) == -1 )   continue;
            
                eraseList.Add( rTrans.gameObject );
            }

            //  削除
            for( int i = 0; i < eraseList.Count; i++ ){
                Destroy( eraseList[ i ] );
            }
        }

        //  ベース配置
        for( int i = 0; i < m_rBaseShell.childCount; i++ ){
            //  編集データのベース
            Transform   rEditBase   =   m_rBaseShell.GetChild( i );
        
            //  拠点を生成
            {
                GameObject  rObj    =   Instantiate( c_HomeBase );
                Transform   rTrans  =   rObj.transform;
        
                rTrans.parent       =   FunctionManager.GetAccessComponent< Transform >( "PlayTest_Shell" );
                rTrans.name         =   "Homebase";

                rTrans.position     =   rEditBase.position;
                rTrans.GetChild( 0 ).localScale
                    =   rEditBase.GetChild( 0 ).localScale;

                //  エディタ用設定
                BaseHealth  rBase   =   rTrans.GetComponent< BaseHealth >();
                if( rBase ){
                    rBase.m_UseEdit =   true;
                }

                //  アクセスを登録
                Transform   rShellTrans =   _rPlayTestShell.FindChild( "EnemySpawnRoot" );
                if( rShellTrans ){
                    rShellTrans.GetComponent< ReferenceWrapper >().m_home_base  =   rObj;
                }
            }
        }
    }
}
