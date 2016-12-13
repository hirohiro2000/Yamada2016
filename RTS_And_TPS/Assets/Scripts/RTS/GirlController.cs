
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class GirlController : NetworkBehaviour 
{

    public  GameObject          c_Drum                      = null;
    private float               c_DrumCost                  = 15.0f;

    public  GameObject          c_TimeBomb                  = null;
    private float               c_TimeBombCost              = 25.0f;

    public  GameObject          c_C4                        = null;
    private float               c_C4Cost                    = 35.0f;

    private UIGirlTaskSelect    m_uiGirlTaskSelect          = null;
    private GameObject          m_symbolShell               = null;

	private ResourceInformation	m_resourceInformation		= null;
	private ResourceCreator		m_resourceCreator			= null;
	private ItemController		m_itemCntroller				= null;
    private Rigidbody           m_rRigid                    = null;
    private TPSPlayer_HP        m_rPlayerHP                 = null;
    private RTS_PlayerAnimationController m_animationController = null;

	private GirlDroneSwitcher	m_cameraSwitcher			= null;

    private GameManager         m_rGameManager              = null;
    private LinkManager         m_rLinkManager              = null;
    private C4Shell_Control     m_rC4Shell                  = null;

	private const KeyCode		m_okKey						= KeyCode.J;
	private const KeyCode		m_cancelKey					= KeyCode.L;
	private const KeyCode		m_breakKey					= KeyCode.K;

	public float				m_moveSpeed					= 1.0f;
    public float                m_LiftingForce              = 1.0f;
    public GameObject           m_symbolPivot               = null;

	private enum ActionState
	{
		Common,
		CreateResource,
		ConvertResource,
		Drone,
	}
	private ActionState			m_actionState	= ActionState.Common;
    
    private bool                m_isMoveByKey   = false;
    private NavMeshAgent        m_navAgent      = null;

    
    
	// Use this for initialization
	void Start ()
	{
        Transform   rHUD                            = GameObject.Find( "Canvas" ).transform.FindChild("RTS_HUD");
        m_uiGirlTaskSelect                          = rHUD.GetComponent<UIGirlTaskSelect>();
        m_uiGirlTaskSelect.m_rResourceInformation   = GameObject.Find("ResourceInformation").GetComponent<ResourceInformation>();
        m_uiGirlTaskSelect.m_itemCntroller          = GetComponent<ItemController>();
        m_uiGirlTaskSelect.Clear();
        m_symbolShell = new GameObject();

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
    }

	//	Write to the FixedUpdate if including physical behavior
	void Update () 
	{
		//GameWorldParameterで強制的に書き換える
		{
			m_moveSpeed = GameWorldParameter.instance.RTSPlayer.WalkSpeed;
		}
        //  自分のキャラクターの場合のみ処理を行う
        if( !isLocalPlayer )        return;
        //  瀕死状態では処理を行わない
        if( m_rPlayerHP.m_IsDying ) return;

        //  座標調整（いまだけ）
        {
            //  飛ぶ
            //if (Input.GetKey( KeyCode.M ))
            //{
            //    m_rRigid.AddForce(Vector3.up * m_LiftingForce * Time.deltaTime * 60.0f);
            //    if (m_navAgent.enabled)
            //    {
            //        m_navAgent.ResetPath();
            //        m_navAgent.enabled = false;
            //    }
            //}

            //  浮いてる間はグリッドを表示しない
            if (Mathf.Abs(m_rRigid.velocity.y) < 0.1f || m_navAgent.enabled)
            {
                m_resourceInformation.m_gridSplitSpacePlane.SetActive(true);
            }
            else
            {
                m_resourceInformation.m_gridSplitSpacePlane.SetActive(false);
            }
        }
        
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
		case ActionState.CreateResource:	CreateResource();	break;
		case ActionState.ConvertResource:	ConvertResource();	break;
		case ActionState.Drone:				UpdateDrone();		break;
		}
	}
	void FixedUpdate () 
	{
        //  自分のキャラクターの場合のみ処理を行う
        if( !isLocalPlayer ) return;

		switch( m_actionState )
		{
		case ActionState.Common:    Move();		break;
		}
	}
    void Move()
    {
        MovedByKey();
        MoveByNavMeshAgent();
        

        // アニメーション
        float   speedSq         = 0.0f;
        float   expValue        = 0.1f;

        // 速度計算
        if ( m_isMoveByKey )
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
	void MovedByKey()
	{		
		//	move
		float v = Input.GetAxis ("Vertical");
		float h = Input.GetAxis ("Horizontal");

		Vector3 cameraForward 	= Vector3.Scale( Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
		Vector3 direction 		= cameraForward * v + Camera.main.transform.right * h;

		direction.Normalize();

		float   axis		    =   ( Mathf.Abs(v) > Mathf.Abs(h) )? Mathf.Abs(v) : Mathf.Abs(h);
        Vector3 moveAmount      =   direction * axis * m_moveSpeed * Time.deltaTime;
            //  瀕死状態なら減速 
            if( m_rPlayerHP.m_IsDying ) moveAmount  =   moveAmount * 0.2f;
		transform.localPosition +=  moveAmount;

		
		//	rotate
		Vector3 animDir 		= direction;
		animDir.y 				= 0;

		if ( animDir.sqrMagnitude > 0.001f )
		{
			Vector3 newDir 		= Vector3.RotateTowards( transform.forward, animDir, 10.0f*Time.deltaTime, 0f );
			transform.rotation 	= Quaternion.LookRotation( newDir );
		}
        
        // Did a player move by a key?
        float   expValue        = 0.1f;
        m_isMoveByKey = ( axis > expValue );
	}
    void MoveByNavMeshAgent()
    {
        if (m_isMoveByKey == true && m_navAgent.enabled )
        {
            m_navAgent.ResetPath();
            m_navAgent.enabled = false;
            return;
        }

        //  速度設定
        if( m_rPlayerHP.m_IsDying ) m_navAgent.speed    =   m_moveSpeed * 0.2f;
        else                        m_navAgent.speed    =   m_moveSpeed;

        if (m_navAgent.enabled )
        {
            if ( m_navAgent.hasPath == false )
            {
                m_navAgent.enabled = false;
            }
            else if ( m_navAgent.hasPath && m_navAgent.remainingDistance < m_navAgent.stoppingDistance)
            {
                m_navAgent.ResetPath();
                m_navAgent.enabled = false;
            }
        }


        if ( RTSCursor.m_curMode == RTSCursor.MODE.eNone && Input.GetMouseButton(0))
        {
            RaycastHit  hit             = new RaycastHit();
            int         layerMask       = LayerMask.GetMask( "Field" );
            Vector3     mousePosition   = Input.mousePosition;
            Ray         ray             = Camera.main.ScreenPointToRay(mousePosition);

            if ( Physics.Raycast(ray, out hit, float.MaxValue, layerMask) )
            {                            
                m_navAgent.enabled      = true;
                m_navAgent.speed        = m_moveSpeed;

                NavMeshPath path = new NavMeshPath();
                if ( m_navAgent.CalculatePath( m_resourceInformation.ComputeGridPosition( hit.point ), path ))
                {
                    m_navAgent.SetPath(path);
                }
            }
        }

    }


	void UpdateCommon()
	{
        m_uiGirlTaskSelect.Clear();
		m_resourceCreator.SetGuideVisibleDisable();

        if ( RTSCursor.m_curMode == RTSCursor.MODE.eNone && Input.GetKeyDown(KeyCode.LeftControl))
        {
            RaycastHit  hit             = new RaycastHit();
            int         layerMask       = LayerMask.GetMask( "Field" );
            Vector3     mousePosition   = Input.mousePosition;
            Ray         ray             = Camera.main.ScreenPointToRay(mousePosition);

            if ( Physics.Raycast(ray, out hit, float.MaxValue, layerMask) )
            {                            
                GameObject pivot = Instantiate( m_symbolPivot );
                pivot.transform.position = hit.point;
                pivot.transform.SetParent( m_symbolShell.transform );
            }
        }

		//	change state
		if( !Input.GetKeyDown( m_okKey ) && !Input.GetMouseButtonDown(1) )
			return;

        if (m_navAgent.enabled)
        {
            m_navAgent.ResetPath();
            m_navAgent.enabled = false;
        }

		if( m_resourceInformation.CheckExistResourceFromPosition( transform.position ) )
		{
			m_actionState = ActionState.ConvertResource;
		}
		else
		{
			m_actionState = ActionState.CreateResource;
		}
	}
	void CreateResource()
	{
		var forcusID	= m_itemCntroller.GetForcus();
		if ( forcusID != -1 )
        {
    		//	リソースの範囲表示更新
		    m_resourceCreator.UpdateGuideResource( forcusID, transform.position );
		    m_resourceCreator.UpdateGuideRange( forcusID, transform.position );
        }


		//	ステート更新
        UIGirlTaskSelect.RESULT uiResult = m_uiGirlTaskSelect.ToSelectTheCreateResource();
		if( ( Input.GetKeyDown( m_okKey )  || uiResult == UIGirlTaskSelect.RESULT.eOK ) &&
			m_itemCntroller.CheckWhetherTheCostIsEnough() )
		{
			var obj = m_resourceCreator.AddResource( forcusID );

            var forcusParam = m_itemCntroller.GetForcusResourceParam();
            m_itemCntroller.AddResourceCost( -forcusParam.GetCreateCost());
			
            //m_actionState = ActionState.Common;


			//	置かれたのがドローンだったらドローン操作に切り替え
			const int droneID = 8;
			if( forcusID == droneID )
			{
				m_cameraSwitcher.On( obj );
				m_actionState = ActionState.Drone;
			}
			else
			{
				m_actionState = ActionState.Common;
			}
			return;
		}
		if( Input.GetKeyDown( m_cancelKey ) || uiResult == UIGirlTaskSelect.RESULT.eCancel || Input.GetMouseButtonDown(1)  )
		{
			m_actionState = ActionState.Common;
			return;
		}
	}
	void ConvertResource()
	{
		var param = m_resourceInformation.GetResourceParamFromPosition( transform.position );

		//	今いるマスにリソースがなかった
        if( param == null )
		{
            m_actionState   =   ActionState.Common;
            return;
        }

		//	リソースの範囲表示更新
		m_resourceCreator.UpdateGuideRange( transform.position );

		//	リソースのUI設定
	    m_uiGirlTaskSelect.m_buttonLevel.transform.FindChild("Point").GetComponent<Text>().text = "-" + param.GetCurLevelParam().GetUpCost().ToString();
		m_uiGirlTaskSelect.m_buttonBreak.transform.FindChild("Point").GetComponent<Text>().text = "+" + param.GetBreakCost().ToString();

		//	ステート更新
        UIGirlTaskSelect.RESULT uiResult = m_uiGirlTaskSelect.ToSelectTheConvertAction();

		if( ( Input.GetKeyDown( m_okKey )  || uiResult == UIGirlTaskSelect.RESULT.eOK ) &&
			m_resourceInformation.CheckIfCanUpALevel( transform.position, m_itemCntroller.GetHaveCost() ))
		{
			m_actionState = ActionState.Common;
    		m_itemCntroller.AddResourceCost( -param.GetCurLevelParam().GetUpCost());
			CmdLevelUpResource( transform.position );
			return;
		}
		if( Input.GetKeyDown( m_cancelKey ) || uiResult == UIGirlTaskSelect.RESULT.eCancel || Input.GetMouseButtonDown(1)  )
		{
			m_actionState = ActionState.Common;
            return;
		}
		if( ( Input.GetKeyDown( m_breakKey ) || uiResult == UIGirlTaskSelect.RESULT.eBreak ) && m_itemCntroller.GetForcus() != -1 )
		{
			m_actionState = ActionState.Common;
			m_itemCntroller.AddResourceCost( m_itemCntroller.GetForcusResourceParam().GetBreakCost() );
            CmdBreakResource( transform.position );
			return;
		}
	}
	void UpdateDrone()
	{
		m_uiGirlTaskSelect.Clear();

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