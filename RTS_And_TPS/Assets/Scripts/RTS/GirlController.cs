
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

public class GirlController : NetworkBehaviour
{
    enum    PlaceState{
        None    =   -1,
        Drum,
        TimeBomb,
        C4,
    };

    public  GameObject          c_Drum							= null;
    private float               c_DrumCost						= 15.0f;

    public  GameObject          c_TimeBomb						= null;
    private float               c_TimeBombCost					= 25.0f;

    public  GameObject          c_C4							= null;
    private float               c_C4Cost						= 35.0f;

    private UIGirlTaskSelect    m_uiGirlTask    				= null;

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

	private const KeyCode		m_okKey							= KeyCode.Z;
	private const KeyCode		m_cancelKey						= KeyCode.X;
	private const KeyCode		m_breakKey						= KeyCode.C;

	public float				m_moveSpeed						= 1.0f;
    public float                m_LiftingForce					= 1.0f;
    public float                m_JumpForce                     = 0.0f;

    private GameObject          m_rRideButton                   = null;
    private GameObject          m_rGetOffButton                 = null;
    private GameObject          m_rBombButtonShell              = null;

    private Transform           m_rPlaceCircle                  = null;
    private Transform           m_rPlaceCircle_B                = null;
    private Transform           m_rPlaceCircle_Place            = null;
    private Transform           m_rPlaceCircle_RayTarget        = null;

    public  bool                c_PlaceRideOnly                 = true;
    private float[]             c_PlaceRange                    = {     12.0f,   12.0f,   12.0f     };
    private float[]             c_BombRange                     = {     12.0f,    9.0f,    4.0f,    };
    private PlaceState          m_PlaceState                    = PlaceState.None;

	private enum ActionState
	{
		Common,
		Drone,
		Ride,
	}
	private ActionState			m_actionState	= ActionState.Common;
    
    private bool                m_isMove        = false;

    private GameObject          m_ridingVehicle = null;

	// Use this for initialization
	void Start ()
	{
        Transform   rHUD                      = GameObject.Find( "Canvas" ).transform.FindChild("RTS_HUD");
        m_uiGirlTask                          = rHUD.GetComponent<UIGirlTaskSelect>();
        m_uiGirlTask.Initialize( this );

        m_rRideButton                   = rHUD.FindChild( "RideButton" ).gameObject;
        m_rGetOffButton                 = rHUD.FindChild( "GetOffButton" ).gameObject;
        m_rBombButtonShell              = rHUD.FindChild( "BombButton_Shell" ).gameObject;

		m_resourceInformation			= GameObject.Find("ResourceInformation").GetComponent<ResourceInformation>();
		m_resourceCreator				= GameObject.Find("ResourceCreator").GetComponent<ResourceCreator>();
        m_rGameManager                  = GameObject.Find("GameManager").GetComponent<GameManager>();
        m_rLinkManager                  = GameObject.Find("LinkManager").GetComponent<LinkManager>();
        m_rC4Shell                      = GameObject.Find("C4_Shell").GetComponent<C4Shell_Control>();
		m_itemCntroller					= GetComponent<ItemController>();
  		m_cameraSwitcher				= GetComponent<GirlDroneSwitcher>();
		m_rRigid                        = GetComponent< Rigidbody >();
        m_rPlayerHP                     = GetComponent< TPSPlayer_HP >();

        m_rPlaceCircle                          = transform.FindChild( "PlaceCircle" );
        m_rPlaceCircle_B                        = transform.FindChild( "PlaceCircle_Range" );
        m_rPlaceCircle_Place                    = transform.FindChild( "PlaceCircle_Place" );
        m_rPlaceCircle_RayTarget                = transform.FindChild( "PlaceCircle_RayTarget" );
        m_rPlaceCircle_RayTarget.localScale     = new Vector3( 1000.0f, 1000.0f, 0.001f );   
		        
        m_animationController = GetComponent< RTS_PlayerAnimationController >();

        //  クローン側での処理
        if( !isLocalPlayer ){
            m_rPlaceCircle_Place.gameObject.SetActive( false );
            m_rPlaceCircle_B.gameObject.SetActive( false );
            m_rPlaceCircle.gameObject.SetActive( false );
        }
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
        
        //  ジャンプする
        if( Input.GetKeyDown( KeyCode.Space )
        &&  Mathf.Abs( m_rRigid.velocity.y ) < 0.01f ){
            m_rRigid.AddForce( Vector3.up * m_JumpForce, ForceMode.Impulse );
        }
        //　乗れるロボットの検索
        if( Input.GetKeyDown( KeyCode.V )
        &&  m_rPlayerHP.m_CurHP > 0 )
        {
            float       nearDistanceSq   = 4.5f;
            GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < playerList.Length; i++)
            {               
                if (this.gameObject == playerList[i])                                                           continue;
                if (playerList[i].GetComponent<GirlController>() != null)                                       continue;

                float distanceSq = ( transform.position - playerList[i].transform.position ).sqrMagnitude;
                if ( distanceSq > nearDistanceSq )    continue; 

                CmdRidingVehicle( gameObject.GetComponent<NetworkIdentity>().netId, playerList[i].GetComponent<NetworkIdentity>().netId, true );
            }
        }

        //  爆弾のショートカットキー
        if( m_actionState   == ActionState.Ride
        ||  c_PlaceRideOnly == false ){
            if( Input.GetKeyDown( KeyCode.Alpha1 ) )    PlaceDrum();
            if( Input.GetKeyDown( KeyCode.Alpha2 ) )    PlaceTimeBomb();
            if( Input.GetKeyDown( KeyCode.Alpha3 ) )    PlaceC4();
            if( Input.GetKeyDown( KeyCode.Alpha4 ) )    ExplodingC4();

            if( Input.GetKeyDown( KeyCode.Alpha1 ) )    SoundController.PlayNow( "UI_FocusChange", 0.0f, 0.05f, 1.0f, 1.0f );
            if( Input.GetKeyDown( KeyCode.Alpha2 ) )    SoundController.PlayNow( "UI_FocusChange", 0.0f, 0.05f, 1.0f, 1.0f );
            if( Input.GetKeyDown( KeyCode.Alpha3 ) )    SoundController.PlayNow( "UI_FocusChange", 0.0f, 0.05f, 1.0f, 1.0f );
            if( Input.GetKeyDown( KeyCode.Alpha4 ) )    SoundController.PlayNow( "UI_Click", 0.0f, 0.05f, 1.0f, 1.0f );
        }

        switch ( m_actionState )
		{
		case ActionState.Common:			UpdateCommon();		break;
		case ActionState.Drone:				UpdateDrone();		break;
		case ActionState.Ride:				UpdateVehicle();    break;
		}

        UpdateAnimation();

        //  搭乗関係のUIを更新
        {
            bool    isAlive     =   m_rPlayerHP.m_CurHP > 0;

            if( m_actionState == ActionState.Common )   m_rRideButton.SetActive( FindAroundRobot( 4.5f ) && isAlive );
            else                                        m_rRideButton.SetActive( false );
            

            m_rGetOffButton.SetActive( m_actionState == ActionState.Ride );

            if( c_PlaceRideOnly ){
                m_rBombButtonShell.SetActive( m_actionState == ActionState.Ride );
                if( m_actionState != ActionState.Ride )     m_PlaceState    =   PlaceState.None;
            }

            //  配置関係の処理
            {
                //  表示を更新
                m_rPlaceCircle.gameObject.SetActive( m_PlaceState != PlaceState.None );
                m_rPlaceCircle_B.gameObject.SetActive( m_PlaceState != PlaceState.None );
                m_rPlaceCircle_Place.gameObject.SetActive( m_PlaceState != PlaceState.None );

                if( m_PlaceState != PlaceState.None ){
                    //  配置しようとしている爆弾のＩＤ
                    int     placeID =   ( int )m_PlaceState;

                    //  サイズ更新
                    m_rPlaceCircle.localScale   =   new Vector3( c_PlaceRange[ placeID ], c_PlaceRange[ placeID ], 1.0f ) * transform.lossyScale.x;
                    m_rPlaceCircle_B.localScale =   new Vector3( c_BombRange[ placeID ],  c_BombRange[ placeID ],  1.0f ) * transform.lossyScale.x;

                    //  カーソル位置計算
                    Vector3 cursorPoint;
                    bool    checkOK =   CheckCursorPoint_OnPCRayTarget( out cursorPoint );
                    if( checkOK ){
                        Vector3 placePoint      =   ClampInHorizontalRange( m_rPlaceCircle.position, cursorPoint, c_PlaceRange[ placeID ] * 1.1f );
                                placePoint.y    =   CheckHeight_OnField( placePoint + Vector3.up * 1000.0f ) + 0.35f;

                        m_rPlaceCircle_B.position       =   placePoint;
                        m_rPlaceCircle_Place.position   =   placePoint;
                    }

                    //  表示を更新 
                    m_rPlaceCircle_B.gameObject.SetActive( checkOK );
                    m_rPlaceCircle_Place.gameObject.SetActive( checkOK );
                }
            }
        }
    }
	void FixedUpdate () 
	{
        //  自分のキャラクターの場合のみ処理を行う
        if( !isLocalPlayer ) return;

        if (m_actionState == ActionState.Common)
        {
            m_isMove = MoveByKey();
        }
	}
    void UpdateAnimation()
    {
        // アニメーション
        float   speedSq         = 0.0f;
        float   expValue        = 0.1f;

        // 速度計算
        if (m_isMove)
        {
            speedSq = m_moveSpeed * m_moveSpeed;
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
    //      ステート
    //---------------------------------------------------------------------   	
    void UpdateCommon()
	{
        Vector3 editTarget = m_resourceInformation.ComputeGridPosition( transform.position );

        // 更新
        UIGirlTaskSelect.RESULT task = m_uiGirlTask.Select( editTarget );
        switch ( task )
        {
            case UIGirlTaskSelect.RESULT.eOK:           Create(m_uiGirlTask.m_editTargetPosition,m_itemCntroller.GetForcus());     break;
            case UIGirlTaskSelect.RESULT.eLevel:        Convert(m_uiGirlTask.m_editTargetPosition);                                break;
            case UIGirlTaskSelect.RESULT.eBreak:        Break(m_uiGirlTask.m_editTargetPosition);                                  break;
            case UIGirlTaskSelect.RESULT.eConfirming:   Confirming(editTarget);                                                    break;
            default: break;
        }
        if ( task != UIGirlTaskSelect.RESULT.eNone && task != UIGirlTaskSelect.RESULT.eConfirming )
        {
            m_uiGirlTask.Reset();
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
    void UpdateVehicle()
    {
        transform.localRotation = Quaternion.identity;
        transform.localPosition = Vector3.zero;
        if (Input.GetKeyDown(KeyCode.V))
        {
            CmdRidingVehicle(gameObject.GetComponent<NetworkIdentity>().netId, m_ridingVehicle.GetComponent<NetworkIdentity>().netId, false);
            return;
        }
    }

    //---------------------------------------------------------------------
    //      アクション
    //---------------------------------------------------------------------   	
    bool MoveByKey()
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
    	transform.position +=  moveAmount;
    	
    	//	rotate
    	Vector3 animDir 		= direction;
    	animDir.y 				= 0;
    
    	if ( animDir.sqrMagnitude > 0.001f )
    	{
    		Vector3 newDir 		            = Vector3.RotateTowards( transform.forward, animDir, 10.0f*Time.deltaTime, 0f );
    		transform.rotation 	= Quaternion.LookRotation( newDir );
    	}

        // Did a player move by a key?
        float   expValue = 0.1f;
        return ( axis > expValue );

    }
    bool Create( Vector3 targetPosition, int resourceID )
    {
        //	今いるマスにリソースが存在する
        var param = m_resourceInformation.GetResourceParamFromPosition( targetPosition );
        if( param != null )
        {
            return false;
        }

        m_resourceCreator.SetGuideResourcePosition( resourceID, targetPosition );
        
        var obj         = m_resourceCreator.AddResource(resourceID);
        var forcusParam = m_itemCntroller.GetForcusResourceParam();
        
        //m_itemCntroller.AddResourceCost(-forcusParam.GetCreateCost());
        GetComponent< NetPlayer_Control >().CmdAddResource( -forcusParam.GetCreateCost() );

        //  支出を通知
        m_rGameManager.SetAcqResource_Minus( forcusParam.GetBreakCost() );
        
        //	置かれたのがドローンだったらドローン操作に切り替え
        const int droneID = 8;
        if (resourceID == droneID)
        {
            m_cameraSwitcher.On(obj);
            m_actionState = ActionState.Drone;
        }
        else
        {
            m_actionState = ActionState.Common;
        }

        return true;
    }
    bool Convert( Vector3 targetPosition )
    {
        //	今いるマスにリソースがなかった
        var param = m_resourceInformation.GetResourceParamFromPosition( targetPosition );
        if( param == null )
        {
            return false;
        }

        m_itemCntroller.AddResourceCost( -param.GetCurLevelParam().GetUpCost());
        CmdLevelUpResource( targetPosition );

        // 効果音再生
        SoundController.PlayNow( "UI_LevelUP",  0.0f, 0.05f, 0.92f, 4.0f );
        //SoundController.PlayNow( "UI_LevelUP2", 0.0f, 0.1f, 1.0f, 2.0f );
        //SoundController.PlayNow( "UI_LevelUP3", 0, 0.05f, 1.2f, 2.0f );

        return true;
    }
    bool Break( Vector3 targetPosition )
    {
        var param = m_resourceInformation.GetResourceParamFromPosition( targetPosition );
        if( param == null )
        {
            return false;
        }

        m_itemCntroller.AddResourceCost( param.GetBreakCost() );
        CmdBreakResource( targetPosition );

        return true;
    }
    bool Confirming( Vector3 targetPosition )
    {
        Vector3 local = transform.position - m_uiGirlTask.m_editTargetPosition;

        float halfScale = m_resourceInformation.m_gridSplitSpaceScale*0.5f;

        if ( local.x > halfScale )
        {
            local.x = halfScale-0.01f;
        }
        if ( local.x < -halfScale )
        {
            local.x = -halfScale+0.01f;
        }
        if ( local.z > halfScale )
        {
            local.z = halfScale-0.01f;
        }
        if ( local.z < -halfScale )
        {
            local.z = -halfScale+0.01f;
        }

        Vector3 world = m_uiGirlTask.m_editTargetPosition + local;
        world.y = transform.position.y;

        transform.position = world;

        return true;
    }

	//---------------------------------------------------------------------
	//      アクセサ
	//---------------------------------------------------------------------   
	public  void    SetActiveButton( bool _IsActive )
    {
        m_uiGirlTask.m_uiConvert.SetActive( _IsActive );
    }

    //  ロボットに乗る
    public  void    RideToRobo()
    {
        float       nearDistanceSq   = 4.5f;
        GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < playerList.Length; i++)
        {               
            if (this.gameObject == playerList[i])                                                           continue;
            if (playerList[i].GetComponent<GirlController>() != null)                                       continue;
        
            float distanceSq = ( transform.position - playerList[i].transform.position ).sqrMagnitude;
            if ( distanceSq > nearDistanceSq )    continue; 
        
            CmdRidingVehicle( gameObject.GetComponent<NetworkIdentity>().netId, playerList[i].GetComponent<NetworkIdentity>().netId, true );
        }
    }
    //  ロボットから降りる
    public  void    GetOutOfTheRobo()
    {
        CmdRidingVehicle(gameObject.GetComponent<NetworkIdentity>().netId, m_ridingVehicle.GetComponent<NetworkIdentity>().netId, false);
    }

    //  ドラム缶を置く
    public  void    PlaceDrum()
    {
        if( m_rGameManager.GetResource() < c_DrumCost )     return;

        if( m_PlaceState == PlaceState.Drum )   m_PlaceState    =   PlaceState.None;
        else                                    m_PlaceState    =   PlaceState.Drum;
    }
    //  時限爆弾を置く
    public  void    PlaceTimeBomb()
    {
        if( m_rGameManager.GetResource() < c_TimeBombCost ) return;

        if( m_PlaceState == PlaceState.TimeBomb )   m_PlaceState    =   PlaceState.None;
        else                                        m_PlaceState    =   PlaceState.TimeBomb;
    }
    //  C4を置く
    public  void    PlaceC4()
    {
        if( m_rGameManager.GetResource() < c_C4Cost )       return;

        if( m_PlaceState == PlaceState.C4 ) m_PlaceState    =   PlaceState.None;
        else                                m_PlaceState    =   PlaceState.C4;
    }
    //  C4を起爆する
    public  void    ExplodingC4()
    {
        CmdExplodingC4();
    }

    //  配置決定
    public  void    PlaceOK()
    {
        if( m_PlaceState == PlaceState.None )   return;

        //  カーソル位置計算 
        Vector3 cursorPoint;
        bool    checkOK =   CheckCursorPoint_OnPCRayTarget( out cursorPoint );
        if( !checkOK )                          return;

        //  配置座標決定
        Vector3 placePoint      =   m_rPlaceCircle_B.position;
                placePoint.y    =   CheckHeight_OnField( placePoint + Vector3.up * 1000.0f );

        //  配置（有効な位置でマウスクリック）
        {
            //  配置
            switch( m_PlaceState ){
                case    PlaceState.Drum:        PlaceAction_Drum( placePoint );                         break;
                case    PlaceState.TimeBomb:    PlaceAction_TimeBomb( placePoint + Vector3.up * 4.0f ); break;
                case    PlaceState.C4:          PlaceAction_C4( placePoint + Vector3.up * 4.0f );       break;
            }
        
            //  配置状態解除
            m_PlaceState    =   PlaceState.None;
        }
    }

    //  搭乗中かどうか
    public  bool    IsRide()
    {
        return  m_actionState == ActionState.Ride;
    }

    //---------------------------------------------------------------------
    //      爆弾の配置関係
    //---------------------------------------------------------------------
    bool    CheckCursorPoint_OnPCRayTarget( out Vector3 _rHitPoint )
    {
        int         layerMask       =   LayerMask.GetMask( "PlaceCircle" );
        Vector3     mousePosition   =   Input.mousePosition;
        Ray         rRay            =   Camera.main.ScreenPointToRay( mousePosition );
        
        RaycastHit  rHitInfo;
        if( Physics.Raycast( rRay, out rHitInfo, float.MaxValue, layerMask ) ){
            _rHitPoint  =   rHitInfo.point;
            return  true;
        }

        _rHitPoint  =   Vector3.zero;
        return  false;
    }
    void    UpdatePlaceRangeCircle()
    {

    }
    Vector3 ClampInHorizontalRange( Vector3 _Origin, Vector3 _Source, float _ClampRange )
    {
        Vector3 toSource        =   _Source - _Origin;
        Vector3 toSource2D      =   new Vector3( toSource.x, 0.0f, toSource.z );
        float   horizonDist     =   toSource2D.magnitude;
        float   verticalDist    =   Mathf.Abs( toSource.y );
        float   hClampDist      =   Mathf.Min( horizonDist, _ClampRange );

        return  _Origin + Vector3.up * verticalDist + toSource2D.normalized * hClampDist;
    }
    float   CheckHeight_OnField( Vector3 _CheckPoint )
    {
        int         layerMask       =   LayerMask.GetMask( "Field" );
        Vector3     mousePosition   =   Input.mousePosition;
        Ray         rRay            =   new Ray( _CheckPoint, -Vector3.up );
        
        RaycastHit  rHitInfo;
        if( Physics.Raycast( rRay, out rHitInfo, float.MaxValue, layerMask ) ){
            return  rHitInfo.point.y;
        }

        return  float.MaxValue;
    }

    void    PlaceAction_Drum( Vector3 _Position )
    {
        if( m_rGameManager.GetResource() < c_DrumCost )     return;

        CmdPlaceDrum( _Position, new Vector3( 0.0f, transform.eulerAngles.y, 0.0f ) );
        m_rLinkManager.m_rLocalNPControl.CmdAddResource( -c_DrumCost );

        //  支出を通知
        m_rGameManager.SetAcqResource_Minus( c_DrumCost );
    }
    void    PlaceAction_TimeBomb( Vector3 _Position )
    {
        if( m_rGameManager.GetResource() < c_TimeBombCost ) return;

        CmdPlaceTimeBomb( _Position, new Vector3( 0.0f, transform.eulerAngles.y, 0.0f ) );
        m_rLinkManager.m_rLocalNPControl.CmdAddResource( -c_TimeBombCost );

        //  支出を通知
        m_rGameManager.SetAcqResource_Minus( c_TimeBombCost );
    }
    void    PlaceAction_C4( Vector3 _Position )
    {
        if( m_rGameManager.GetResource() < c_C4Cost )       return;

        CmdPlaceC4( _Position, new Vector3( 0.0f, transform.eulerAngles.y, 0.0f ) );
        m_rLinkManager.m_rLocalNPControl.CmdAddResource( -c_C4Cost );

        //  支出を通知
        m_rGameManager.SetAcqResource_Minus( c_C4Cost );
    }

    //---------------------------------------------------------------------
	//      
	//---------------------------------------------------------------------
    bool RaycastFromMouseToField( out Vector3 hitPoint )
    {
        RaycastHit  hit             = new RaycastHit();
        int         layerMask       = LayerMask.GetMask( "Field" );
        Vector3     mousePosition   = Input.mousePosition;
        Ray         ray             = Camera.main.ScreenPointToRay(mousePosition);
        
        if ( Physics.Raycast(ray, out hit, float.MaxValue, layerMask) )
        {
            hitPoint = hit.point;
            return true;
        }

        hitPoint = Vector3.zero;
        return false;

    }
    void RideVehicle( GameObject vehicle, bool isEnable )
    {
        m_actionState      = ( isEnable ) ? ActionState.Ride  : ActionState.Common;
        m_ridingVehicle    = ( isEnable ) ? vehicle           : null;
        transform.SetParent( ( isEnable ) ? vehicle.transform : null );

        GetComponent<CapsuleCollider>().enabled     = !isEnable;
        GetComponent<LerpSync_Position>().enabled   = !isEnable;
        GetComponent<LerpSync_Rotation>().enabled   = !isEnable;
        GetComponent<Rigidbody>().isKinematic       =  isEnable;

        //  モデルを表示 / 非表示
        transform.GetChild( 0 ).gameObject.SetActive( !isEnable );

        if (isEnable)
        {
            transform.localRotation = Quaternion.identity;
            transform.localPosition = Vector3.zero;
        }

        //  降りた場合は少し上に出る 
        if( isLocalPlayer ){
            if( !isEnable ) transform.position  =  transform.position + Vector3.up * 3.0f;
        }

        //  搭乗メッセージ 
        if( isLocalPlayer ){
            if( isEnable )  m_rGameManager.SetAcqRecord( "相棒に搭乗しました！", m_rLinkManager.m_LocalPlayerID );
            else{
                if( _ByMyself ) m_rGameManager.SetAcqRecord( "相棒から降りました！", m_rLinkManager.m_LocalPlayerID );
                else            m_rGameManager.SetAcqRecord( "相棒から発射されました！", m_rLinkManager.m_LocalPlayerID );
            }
        }
        else{
            if( isEnable )  m_rGameManager.SetAcqRecord( "相棒が搭乗しました！", m_rLinkManager.m_LocalPlayerID );
            else{
                if( _ByMyself ) m_rGameManager.SetAcqRecord( "相棒が降りました！", m_rLinkManager.m_LocalPlayerID );
                else            m_rGameManager.SetAcqRecord( "相棒を発射しました！", m_rLinkManager.m_LocalPlayerID );
            }
        }

        //  効果音再生
        if( isEnable )  SoundController.PlayNow( "RideOn", null, transform.position, 0.0f, 1.0f, 1.0f, 4.0f );
        else            SoundController.PlayNow( "GetOutOff", null, transform.position, 0.0f, 1.0f, 1.0f, 4.0f );

        //  ボイス再生
        if( isEnable )  SoundController.PlayNow( "Voice_G_Chest", transform, transform.position, 0.0f, 1.0f, 1.0f, 10.0f );
        else            SoundController.PlayNow( "Voice_G_Thanks2", transform, transform.position, 0.0f, 1.0f, 1.0f, 10.0f );
    }

    //  乗れるロボットが周囲に居るかどうか
    GameObject  FindAroundRobot( float _CheckDistance )
    {
        float           nearDistanceSq  =   _CheckDistance;
        GameObject[]    playerList      =   GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < playerList.Length; i++)
        {               
            if (this.gameObject == playerList[i])                           continue;
            if (playerList[i].GetComponent< TPSPlayer_Control >() == null)  continue;
        
            float distanceSq = ( transform.position - playerList[i].transform.position ).sqrMagnitude;
            if ( distanceSq > nearDistanceSq )    continue; 
        
            return  playerList[ i ];
        }

        return  null;
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
        //Vector3     vForce      =   _Foward.normalized    * 0.25f
        //                        +   Vector3.up.normalized * 1.0f;
        //            vForce      *=  10.0f;
        //rC4Control.m_StartForce =   vForce;

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
 
    [ Command ]
    void    CmdRidingVehicle( NetworkInstanceId _NetID, NetworkInstanceId _TargetNetID, bool _Enable )
    {
        RpcRidingVehicle( _NetID, _TargetNetID, _Enable );
    }
    [ Command ]
    public  void    CmdPlaceDrone( NetworkInstanceId _NetID )
    {
        RpcPlaceDrone( _NetID );
    }
    
    [ ClientRpc ]
    public  void    RpcRidingVehicle( NetworkInstanceId _NetID, NetworkInstanceId _TargetNetID, bool _Enable )
    {
        //  対象オブジェクトを探す
        GameObject rIdentity        =  ClientScene.FindLocalObject( _NetID );
        GameObject rTarget          =  ClientScene.FindLocalObject( _TargetNetID );
        
        rIdentity.GetComponent<GirlController>().RideVehicle( rTarget, _Enable );
                
    }
    [ ClientRpc ]
    void    RpcPlaceDrone( NetworkInstanceId _NetID )
    {
        GameObject org = GameObject.Find("SkillItemShell").transform.GetChild(0).gameObject;

        //  オブジェクト生成
        GameObject  rObj    =   Instantiate( org );
        Transform   rTrans  =   rObj.transform;
        GameObject  rGirl   =   ClientScene.FindLocalObject( _NetID );

        rObj.SetActive(true);
        rObj.transform.SetParent( rGirl.transform );

        //  配置設定
        rObj.transform.localPosition = new Vector3( 0.3f, 2.3f, -0.5f );
        rObj.transform.localRotation = Quaternion.identity;
    }


}