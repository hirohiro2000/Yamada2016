
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

public class GirlController : NetworkBehaviour
{

    public  GameObject          c_Drum							= null;
    private float               c_DrumCost						= 15.0f;

    public  GameObject          c_TimeBomb						= null;
    private float               c_TimeBombCost					= 25.0f;

    public  GameObject          c_C4							= null;
    private float               c_C4Cost						= 35.0f;

    private UIGirlTaskSelect    m_uiGirlTaskSelect				= null;

	private ResourceInformation	m_resourceInformation			= null;
	private ResourceCreator		m_resourceCreator				= null;
	private ItemController		m_itemCntroller					= null;
    private Rigidbody           m_rRigid						= null;
    private TPSPlayer_HP        m_rPlayerHP						= null;
    private RTS_PlayerAnimationController m_animationController = null;

	private GirlDroneSwitcher	m_cameraSwitcher				= null;

    private GameManager         m_rGameManager					= null;
    private LinkManager         m_rLinkManager					= null;
    private C4Shell_Control     m_rC4Shell						= null;

	private const KeyCode		m_okKey							= KeyCode.J;
	private const KeyCode		m_cancelKey						= KeyCode.L;
	private const KeyCode		m_breakKey						= KeyCode.K;

	public float				m_moveSpeed						= 1.0f;
    public float                m_LiftingForce					= 1.0f;
    public GameObject           m_symbolPivot					= null;
	public GameObject		    m_routingError					= null;

	private enum ActionState
	{
		Common,
		Drone,
	}
	private ActionState			m_actionState	= ActionState.Common;
    
    private NavMeshAgent        m_navAgent      = null;
    private bool                m_isEditMode    = false;
    private Vector3             m_editTarget    = Vector3.zero; 
    
    interface IMoveToTargetCallback
    {
        void OnStart( Vector3 requestPosition );
        void OnStop( Vector3 requestPosition );
        void OnPositionEnter( Vector3 requestPosition );
    }
    class MoveSubContractor
    {
        class TargetData
        {
            public bool     m_userInput   { get; set; }
            public Vector3  m_targetPoint { get; set; }
            public IMoveToTargetCallback m_callback { get; set; }
        }
        GirlController  m_controller            { get; set; }
        Stack           m_targetStack           { get; set; }
        NavMeshAgent    m_agent                 { get { return m_controller.m_navAgent; } }
        public bool            m_isMoveByKey    { get; private set; }

        public MoveSubContractor(GirlController controller)
        {
            this.m_isMoveByKey  = false;
            this.m_controller   = controller;
            m_targetStack       = new Stack();
        }

        public  void Update()
        {
            UpdateAgent( m_isMoveByKey );
        }
        public  void FixedUpdate()
        {
            m_isMoveByKey = MoveByKey();
        }
        private void UpdateAgent( bool keyMove )
        {
            // キーボードによるプレイヤーの操作が行われているか？
            if ( keyMove && m_agent.enabled )
            {
                m_agent.ResetPath();
                m_agent.enabled = false;
                return;
            }    
                        
            
            // 左クリックによる移動の更新
            MoveByMouse();
            

            // 動作していないかの判定
            if ( m_agent.enabled     == false )   return;
            if ( m_targetStack.Count == 0 )       return;
            
            // 目標地点に到達したかによる判定
            bool arrivedOnTarget  = ( m_agent.hasPath == false );
                 arrivedOnTarget |= ( m_agent.remainingDistance <= m_agent.stoppingDistance );
               
            if ( arrivedOnTarget == false ) return;

            // 目標に到達のでコールバックを呼び出します
            TargetData current = (TargetData)m_targetStack.Peek();
            if ( current.m_callback != null )
            {
                current.m_callback.OnStop( current.m_targetPoint );
                current.m_callback.OnPositionEnter( current.m_targetPoint );
            }

            //　次の目標へ設定し直します
            ToTheNextTargetPosition();

            return;
        }

        private bool MoveByKey()
    	{		
    		//	move
    		float v = Input.GetAxis ("Vertical");
    		float h = Input.GetAxis ("Horizontal");
    
    		Vector3 cameraForward 	= Vector3.Scale( Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
    		Vector3 direction 		= cameraForward * v + Camera.main.transform.right * h;
    
    		direction.Normalize();
    
    		float axis				= ( Mathf.Abs(v) > Mathf.Abs(h) )? Mathf.Abs(v) : Mathf.Abs(h);
    		m_controller.transform.localPosition += direction * axis * m_controller.m_moveSpeed * Time.deltaTime;
    
    		
    		//	rotate
    		Vector3 animDir 		= direction;
    		animDir.y 				= 0;
    
    		if ( animDir.sqrMagnitude > 0.001f )
    		{
    			Vector3 newDir 		            = Vector3.RotateTowards( m_controller.transform.forward, animDir, 10.0f*Time.deltaTime, 0f );
    			m_controller.transform.rotation 	= Quaternion.LookRotation( newDir );
    		}
            
            // Did a player move by a key?
            float   expValue        = 0.1f;
            return ( axis > expValue );
    	}
        private void MoveByMouse()
        {
            if ( RTSCursor.m_curMode == RTSCursor.MODE.eNone && Input.GetMouseButton(0))
            {                
                Vector3 hitPoint;
                if (RayToField( out hitPoint ) )
                {
                    AddNewTargetPosition( hitPoint, true, null );
                }
            }

        }      
        
        // [isUserData]が[true]の場合呼ばれません
        public  bool AddNewTargetPosition( Vector3 target, bool isUserData, IMoveToTargetCallback callback )
        {
            if (m_targetStack.Count > 0)
            {
                TargetData data = (TargetData)m_targetStack.Peek();
                if ( data.m_userInput == false && data.m_callback != null )
                {
                    data.m_callback.OnStop( data.m_targetPoint );
                }
            }

            //　エージェントの起動と設定
            m_agent.enabled   = true;
            NavMeshPath path = new NavMeshPath();
            if ( m_agent.CalculatePath( target, path ) )
            {
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    m_agent.SetPath(path);

                    if (callback != null)
                    {
                        callback.OnStart(target);
                    }

                    // 最上部が左クリックによる移動かつ追加しようとしているデータも左クリックによるものである
                    if (isUserData && PeekIsUserInput())
                    {
                        TargetData currentData = (TargetData)m_targetStack.Peek();
                        currentData.m_targetPoint = target;
                    }
                    else
                    {
                        // ターゲット情報を追加
                        TargetData newData = new TargetData();
                        newData.m_userInput = isUserData;
                        newData.m_targetPoint = target;
                        newData.m_callback = callback;
                        m_targetStack.Push(newData);
                    }
                    return true;
                }
            }
            return false;
        }
        private void ToTheNextTargetPosition()
        {
            m_targetStack.Pop();
            
            if ( m_targetStack.Count > 0 )
            {
                // 次のターゲットを繰り上げて設定する
                TargetData nextTarget = (TargetData)m_targetStack.Peek();
                NavMeshPath path = new NavMeshPath();
                if (m_agent.CalculatePath(nextTarget.m_targetPoint, path))
                {
                    //　エージェントの起動と設定
                    m_agent.enabled = true;
                    m_agent.SetPath(path);

                    if (nextTarget.m_callback != null)
                    {
                        nextTarget.m_callback.OnStart(nextTarget.m_targetPoint);
                    }

                }
            }
            else
            {
                m_agent.ResetPath();
                m_agent.enabled = false;
            }
        }

        private bool PeekIsUserInput()
        {
            // 最上部は左クリックによる操作か？
            if ( m_targetStack.Count > 0 )
            {
                TargetData currentData = (TargetData)m_targetStack.Peek();
                return currentData.m_userInput;
            }
            return false;
        }
        private bool RayToField(out Vector3 hitPoint)
        {
            RaycastHit hit = new RaycastHit();
            int layerMask = LayerMask.GetMask("Field");
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out hit, float.MaxValue, layerMask))
            {
                hitPoint = hit.point;
                return true;
            }

            hitPoint = Vector3.zero;
            return false;
        }

    }
    MoveSubContractor m_moveContractor = null;

    class ActionCreate : IMoveToTargetCallback
    {
        int             resourceID   { get; set; }
        GirlController  controller   { get; set; }
        GameObject      targetSymbol { get; set; }
        public ActionCreate( int resourceID, GirlController controller )
        {
            this.resourceID = resourceID;
            this.controller = controller;
        }
        public void OnStart( Vector3 requestPosition )
        {
            if ( targetSymbol != null ) return;
            targetSymbol = Instantiate( controller.m_symbolPivot );
            targetSymbol.transform.position = requestPosition;
        }
        public void OnStop( Vector3 requestPosition )
        {
            if ( targetSymbol == null ) return;
            Destroy( targetSymbol );
        }
        public void OnPositionEnter(Vector3 requestPosition)
        {
            controller.m_resourceCreator.SetGuideResourcePosition( resourceID, requestPosition );

            var obj         = controller.m_resourceCreator.AddResource(resourceID);
            var forcusParam = controller.m_itemCntroller.GetForcusResourceParam();

            controller.m_itemCntroller.AddResourceCost(-forcusParam.GetCreateCost());

            //	置かれたのがドローンだったらドローン操作に切り替え
            const int droneID = 8;
            if (resourceID == droneID)
            {
                controller.m_cameraSwitcher.On(obj);
                controller.m_actionState = ActionState.Drone;
            }
            else
            {
                controller.m_actionState = ActionState.Common;
            }

        }
    }
    class ActionConvert : IMoveToTargetCallback
    {
        int             resourceID { get; set; }
        GirlController  controller { get; set; }
        GameObject      targetSymbol { get; set; }
        public ActionConvert( int resourceID, GirlController controller )
        {
            this.resourceID = resourceID;
            this.controller = controller;
        }
        public void OnStart( Vector3 requestPosition )
        {
            if ( targetSymbol != null ) return;
            targetSymbol = Instantiate( controller.m_symbolPivot );
            targetSymbol.transform.position = requestPosition;
        }
        public void OnStop( Vector3 requestPosition )
        {
            if ( targetSymbol == null ) return;
            Destroy( targetSymbol );
        }
        public void OnPositionEnter( Vector3 requestPosition )
        {
    		//	今いるマスにリソースがなかった
    		var param = controller.m_resourceInformation.GetResourceParamFromPosition( requestPosition );
            if( param == null )
    		{
                return;
            }
            controller.m_itemCntroller.AddResourceCost( -param.GetCurLevelParam().GetUpCost());
        	controller.CmdLevelUpResource( requestPosition );
        }
    }
    class ActionBreak : IMoveToTargetCallback
    {
        int             resourceID { get; set; }
        GirlController  controller { get; set; }
        GameObject      targetSymbol { get; set; }
        public ActionBreak( int resourceID, GirlController controller )
        {
            this.resourceID = resourceID;
            this.controller = controller;
        }
        public void OnStart( Vector3 requestPosition )
        {
            if ( targetSymbol != null ) return;
            targetSymbol = Instantiate( controller.m_symbolPivot );
            targetSymbol.transform.position = requestPosition;
        }
        public void OnStop( Vector3 requestPosition )
        {
            if ( targetSymbol == null ) return;
            Destroy( targetSymbol );
        }
        public void OnPositionEnter( Vector3 requestPosition )
        {
        	controller.m_itemCntroller.AddResourceCost( controller.m_itemCntroller.GetForcusResourceParam().GetBreakCost() );
            controller.CmdBreakResource( requestPosition );
        }
    }

	// Use this for initialization
	void Start ()
	{
        Transform   rHUD                            = GameObject.Find( "Canvas" ).transform.FindChild("RTS_HUD");
        m_uiGirlTaskSelect                          = rHUD.GetComponent<UIGirlTaskSelect>();
        m_uiGirlTaskSelect.m_rResourceInformation   = GameObject.Find("ResourceInformation").GetComponent<ResourceInformation>();
        m_uiGirlTaskSelect.m_itemCntroller          = GetComponent<ItemController>();
        m_uiGirlTaskSelect.Clear();

		m_resourceInformation			= GameObject.Find("ResourceInformation").GetComponent<ResourceInformation>();
		m_resourceCreator				= GameObject.Find("ResourceCreator").GetComponent<ResourceCreator>();
        m_rGameManager                  = GameObject.Find("GameManager").GetComponent<GameManager>();
        m_rLinkManager                  = GameObject.Find("LinkManager").GetComponent<LinkManager>();
        m_rC4Shell                      = GameObject.Find("C4_Shell").GetComponent<C4Shell_Control>();
		m_itemCntroller					= GetComponent<ItemController>();
  		m_cameraSwitcher				= GetComponent<GirlDroneSwitcher>();
		m_rRigid                        = GetComponent< Rigidbody >();
        m_rPlayerHP                     = GetComponent< TPSPlayer_HP >();
		
        //  ローカルでのみエージェントを追加
        if( isLocalPlayer ){
            m_navAgent                      = gameObject.AddComponent<NavMeshAgent>();
            m_navAgent.acceleration			= float.MaxValue;
            m_navAgent.angularSpeed			= float.MaxValue;
            m_navAgent.stoppingDistance		= 1.0f;
            m_navAgent.Warp( transform.position );
            m_navAgent.ResetPath();
        }
        
        m_animationController = GetComponent< RTS_PlayerAnimationController >();

        m_moveContractor = new MoveSubContractor(this);
    }

	//	Write to the FixedUpdate if including physical behavior
	void Update () 
	{
		//GameWorldParameterで強制的に書き換える
		m_moveSpeed = GameWorldParameter.instance.RTSPlayer.WalkSpeed;
		
        //  自分のキャラクターの場合のみ処理を行う
        if( !isLocalPlayer )        return;
       
		//  瀕死状態では処理を行わない
        if( m_rPlayerHP.m_IsDying ) return;
        
        //  ドラム缶を置く
        if( Input.GetKeyDown( KeyCode.B )
        &&  m_rGameManager.GetResource() >= c_DrumCost ){
            CmdPlaceDrum( transform.position + transform.forward * 2.0f, new Vector3( 0.0f, transform.eulerAngles.y, 0.0f ) );
            m_rLinkManager.m_rLocalNPControl.CmdAddResource( -c_DrumCost );
        }
        //  時限爆弾を置く
        if( Input.GetKeyDown( KeyCode.N )
        &&  m_rGameManager.GetResource() >= c_TimeBombCost ){
            CmdPlaceTimeBomb( transform.position + transform.forward * 2.0f + Vector3.up * 2.0f, new Vector3( 0.0f, transform.eulerAngles.y, 0.0f ) );
            m_rLinkManager.m_rLocalNPControl.CmdAddResource( -c_TimeBombCost );
        }
        //  C4爆弾を置く
        if( Input.GetKeyDown( KeyCode.Comma ) 
        &&  m_rGameManager.GetResource() >= c_C4Cost ){
            m_rLinkManager.m_rLocalNPControl.CmdAddResource( -c_C4Cost );
            CmdPlaceC4( transform.position + transform.forward * 2.0f + Vector3.up * 2.0f, transform.forward );
        }
        //  C4爆弾を起動する
        if( Input.GetKeyDown( KeyCode.Period ) ){
            CmdExplodingC4();
        }

		switch( m_actionState )
		{
		case ActionState.Common:			UpdateCommon();		break;
		case ActionState.Drone:				UpdateDrone();		break;
		}

        UpdateAnimation();
    }
	void FixedUpdate () 
	{
        //  自分のキャラクターの場合のみ処理を行う
        if( !isLocalPlayer ) return;
        m_moveContractor.FixedUpdate();
	}
    void UpdateAnimation()
    {
        // アニメーション
        float   speedSq         = 0.0f;
        float   expValue        = 0.1f;

        // 速度計算
        if ( m_moveContractor.m_isMoveByKey )
        {
            speedSq = m_moveSpeed*m_moveSpeed;
        }
        else if ( m_navAgent.enabled )
        {
            Vector2 walkVelocity    = new Vector2( m_navAgent.velocity.x, m_navAgent.velocity.z );
            speedSq = walkVelocity.sqrMagnitude;
        }

        // モーション決定
        if ( speedSq > expValue )
        {
            m_animationController.ChangeStateMove(speedSq);
        }
        else
        {
            m_animationController.ChangeStateIdle();
        }


    }

  
	//---------------------------------------------------------------------
    //      すてーと
    //---------------------------------------------------------------------   	
	void UpdateCommon()
	{
		m_moveContractor.Update();

        //
        if ( m_isEditMode == false )
        {
            bool    anyEditKey = false;

			//	キークリックでイベント開始判定
            if ( Input.GetKeyDown( m_okKey ) )
            {
                anyEditKey  = true;
                m_editTarget = m_resourceInformation.ComputeGridPosition( transform.position );
            }
    
			//	マウスでイベント開始判定
            if ( RTSCursor.m_curMode == RTSCursor.MODE.eNone && Input.GetMouseButtonDown(1))
            {
                RaycastHit  hit             = new RaycastHit();
                int         layerMask       = LayerMask.GetMask( "Field" );
                Vector3     mousePosition   = Input.mousePosition;
                Ray         ray             = Camera.main.ScreenPointToRay(mousePosition);
            
                if ( Physics.Raycast(ray, out hit, float.MaxValue, layerMask) )
                {      
                    m_editTarget = m_resourceInformation.ComputeGridPosition( hit.point );

                    bool navEnabled = m_navAgent.enabled;
                    
                    m_navAgent.enabled = true;
                    NavMeshPath path = new NavMeshPath();

                    if ( m_navAgent.CalculatePath( m_editTarget, path ) && path.status == NavMeshPathStatus.PathComplete )
                    {
                        anyEditKey  = true;
                    }
                    else if (m_resourceInformation.CheckExistResourceFromPosition(m_editTarget))
                    {
                        anyEditKey = true;
                    }
                    

                    if (path.status == NavMeshPathStatus.PathPartial)
                    {
                        // 選択されたグリッドの位置に女の子は行けません
                        GameObject obj = Instantiate(m_routingError);
                        obj.transform.position = m_editTarget;
                        obj.transform.position += new Vector3(0, 0.1f, 0);
                    }

                    m_navAgent.enabled = navEnabled;
               }
            }

			//	イベントが開始されたら
            if ( anyEditKey )
            {
                m_isEditMode = true; 
    
                m_uiGirlTaskSelect.Clear();

                // グリッドにオブジェクトが存在するのか
                if (m_resourceInformation.CheckExistResourceFromPosition(m_editTarget))
                {
                    m_uiGirlTaskSelect.ToSelectTheConvertAction();
					
					//	リソースのUI設定
					var param = m_resourceInformation.GetResourceParamFromPosition( transform.position );
					m_uiGirlTaskSelect.m_buttonLevel.transform.FindChild("Point").GetComponent<Text>().text = "-" + param.GetCurLevelParam().GetUpCost().ToString();
					m_uiGirlTaskSelect.m_buttonBreak.transform.FindChild("Point").GetComponent<Text>().text = "+" + param.GetBreakCost().ToString();

                }
                else
                {
                    m_uiGirlTaskSelect.ToSelectTheCreateResource();

                    m_resourceInformation.m_gridSplitSpacePlane.GetComponent<Renderer>().enabled = true;
                    m_resourceInformation.m_gridSplitSpacePlane.transform.position = m_editTarget;
                    m_resourceInformation.m_gridSplitSpacePlane.transform.position += new Vector3(0, 0.04f, 0);
                }
            }
        }
        else
		{ 

    		var forcusID	= m_itemCntroller.GetForcus();
    		if ( forcusID != -1 )
            {
        		//	リソースの範囲表示更新
    		    m_resourceCreator.UpdateGuideResource( forcusID, m_editTarget );
    		    m_resourceCreator.UpdateGuideRange( forcusID, m_editTarget );
            }
            else
            {
                m_resourceCreator.SetGuideVisibleDisable();
            }
    
            bool isComp   = false;
            bool isCancel = false;
            IMoveToTargetCallback callback = null;

            UIGirlTaskSelect.RESULT result = m_uiGirlTaskSelect.result;
            if( result == UIGirlTaskSelect.RESULT.eCancel || Input.GetKeyDown( m_cancelKey ) || Input.GetMouseButtonDown(1) )
            {
                isCancel = true;
            }
            else if ( result == UIGirlTaskSelect.RESULT.eOK )
            {
                isComp   = true;
                callback = new ActionCreate( m_itemCntroller.GetForcus(), this );
            }
            else if ( result == UIGirlTaskSelect.RESULT.eLevel )
            {
                isComp   = true;
                callback = new ActionConvert( m_itemCntroller.GetForcus(), this );
            }
            else if ( result == UIGirlTaskSelect.RESULT.eBreak )
            {
                isComp   = true;
                callback = new ActionBreak( m_itemCntroller.GetForcus(), this );
            }

            if ( isComp || isCancel )
            {
                m_isEditMode = false;
                m_uiGirlTaskSelect.SetForcus( -1 );
                m_uiGirlTaskSelect.Clear();
                m_resourceCreator.SetGuideVisibleDisable();
                m_resourceInformation.m_gridSplitSpacePlane.GetComponent<Renderer>().enabled = false;

                if ( isComp )
                    m_moveContractor.AddNewTargetPosition( m_editTarget, false, callback );
            }
        }
	}
	void UpdateDrone()
	{
		if( Input.GetKeyDown( KeyCode.O ))
		{
			m_cameraSwitcher.Off();
			m_actionState = ActionState.Common;
            return;
		}
	}


	//---------------------------------------------------------------------
	//      アクセサ
	//---------------------------------------------------------------------   
	public  void    SetActiveButton( bool _IsActive )
    {
        m_uiGirlTaskSelect.m_buttonOk.SetActive( _IsActive );
        m_uiGirlTaskSelect.m_buttonCancel.SetActive( _IsActive );
        m_uiGirlTaskSelect.m_buttonLevel.SetActive( _IsActive );
        m_uiGirlTaskSelect.m_buttonBreak.SetActive( _IsActive );
    }


    //---------------------------------------------------------------------
	//      コマンド
	//---------------------------------------------------------------------
    [ Command ]
    void    CmdLevelUpResource( Vector3 _Position )
    {
        m_resourceInformation.LevelUpResource( _Position, connectionToClient.connectionId );
    }
    [ Command ]
    void    CmdBreakResource( Vector3 _Position )
    {
        m_resourceInformation.SetGridInformation( null, _Position, false );
    }
    [ Command ]
    void    CmdPlaceDrum( Vector3 _Position, Vector3 _Angle )
    {
        //  オブジェクト生成
        GameObject  rObj    =   Instantiate( c_Drum );
        Transform   rTrans  =   rObj.transform;

        //  配置設定
        rTrans.position     =   _Position;
        rTrans.eulerAngles  =   _Angle;

        //  ネットワーク上で共有
        NetworkServer.Spawn( rObj );
    }
    [ Command ]
    void    CmdPlaceTimeBomb( Vector3 _Position, Vector3 _Angle )
    {
        //  オブジェクト生成
        GameObject  rObj    =   Instantiate( c_TimeBomb );
        Transform   rTrans  =   rObj.transform;

        //  配置設定
        rTrans.position     =   _Position;
        rTrans.eulerAngles  =   _Angle;

        //  オーナー設定
        TimeBomb_Control    rTBControl  =   rObj.GetComponent< TimeBomb_Control >();
        DetonationObject    rDOControl  =   rObj.GetComponent< DetonationObject >();
        rTBControl.c_OwnerID        =   connectionToClient.connectionId;
        rDOControl.m_DestroyerID    =   connectionToClient.connectionId;

        //  ネットワーク上で共有
        NetworkServer.Spawn( rObj );
    }
    [ Command ]
    void    CmdPlaceC4( Vector3 _Position, Vector3 _Foward )
    {
        //  オブジェクト生成
        GameObject  rObj    =   Instantiate( c_C4 );
        Transform   rTrans  =   rObj.transform;

        //  配置設定
        rTrans.position     =   _Position;
        rTrans.eulerAngles  =   new Vector3( 90.0f, Mathf.Atan2( _Foward.x, _Foward.z ) * Mathf.Rad2Deg + 180.0f, 0.0f );
        
        //  オーナー設定
        C4_Control          rC4Control  =   rObj.GetComponent< C4_Control >();
        DetonationObject    rDOControl  =   rObj.GetComponent< DetonationObject >();
        rC4Control.c_OwnerID        =   connectionToClient.connectionId;
        rDOControl.m_DestroyerID    =   connectionToClient.connectionId;

        //  移動量設定 
        Vector3     vForce      =   _Foward.normalized    * 0.25f
                                +   Vector3.up.normalized * 1.0f;
                    vForce      *=  10.0f;
        rC4Control.m_StartForce =   vForce;

        //  ネットワーク上で共有
        NetworkServer.Spawn( rObj );        
    }
    [ Command ]
    void    CmdExplodingC4()
    {
        List< GameObject >  rC4List =   m_rC4Shell.m_rC4List;
        for( int i = 0; i < m_rC4Shell.m_rC4List.Count; i++ ){
            GameObject  rObj        =   rC4List[ i ];
            if( !rObj ) continue;

            C4_Control  rControl    =   rObj.GetComponent< C4_Control >();
            //  オーナーチェック
            if( rControl.c_OwnerID != connectionToClient.connectionId ) continue;

            //  起爆
            rControl.SetExploding();
        }
    }
}