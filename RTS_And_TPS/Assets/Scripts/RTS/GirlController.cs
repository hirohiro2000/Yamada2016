
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
    public GameObject           m_symbolPivot					= null;
	public GameObject		    m_routingError					= null;

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
        m_uiGirlTask.m_resourceInformation    = GameObject.Find("ResourceInformation").GetComponent<ResourceInformation>();
        m_uiGirlTask.m_resourceCreator        = GameObject.Find("ResourceCreator").GetComponent<ResourceCreator>();
        m_uiGirlTask.m_itemController         = GetComponent<ItemController>();
        m_uiGirlTask.Reset();

		m_resourceInformation			= GameObject.Find("ResourceInformation").GetComponent<ResourceInformation>();
		m_resourceCreator				= GameObject.Find("ResourceCreator").GetComponent<ResourceCreator>();
        m_rGameManager                  = GameObject.Find("GameManager").GetComponent<GameManager>();
        m_rLinkManager                  = GameObject.Find("LinkManager").GetComponent<LinkManager>();
        m_rC4Shell                      = GameObject.Find("C4_Shell").GetComponent<C4Shell_Control>();
		m_itemCntroller					= GetComponent<ItemController>();
  		m_cameraSwitcher				= GetComponent<GirlDroneSwitcher>();
		m_rRigid                        = GetComponent< Rigidbody >();
        m_rPlayerHP                     = GetComponent< TPSPlayer_HP >();
		        
        m_animationController = GetComponent< RTS_PlayerAnimationController >();
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
        //  ジャンプする
        if( Input.GetKeyDown( KeyCode.M )
        &&  Mathf.Abs( m_rRigid.velocity.y ) < 0.01f ){
            m_rRigid.AddForce( Vector3.up * m_JumpForce, ForceMode.Impulse );
        }
        //　乗れるロボットの検索
        if ( Input.GetKeyDown(KeyCode.Space) )
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

        switch ( m_actionState )
		{
		case ActionState.Common:			UpdateCommon();		break;
		case ActionState.Drone:				UpdateDrone();		break;
		case ActionState.Ride:				UpdateVehicle();    break;
		}

        UpdateAnimation();
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
            case UIGirlTaskSelect.RESULT.eOK:       Create(editTarget,m_itemCntroller.GetForcus());    break;
            case UIGirlTaskSelect.RESULT.eLevel:    Convert(editTarget);                                break;
            case UIGirlTaskSelect.RESULT.eBreak:    Break(editTarget);                                  break;
            default: break;
        }
        if ( task != UIGirlTaskSelect.RESULT.eNone )
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
        if (Input.GetKeyDown(KeyCode.Space))
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
        
        m_itemCntroller.AddResourceCost(-forcusParam.GetCreateCost());
        
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

	//---------------------------------------------------------------------
	//      アクセサ
	//---------------------------------------------------------------------   
	public  void    SetActiveButton( bool _IsActive )
    {
        m_uiGirlTask.m_buttonOk.SetActive( _IsActive );
        m_uiGirlTask.m_buttonCancel.SetActive( _IsActive );
        m_uiGirlTask.m_buttonLevel.SetActive( _IsActive );
        m_uiGirlTask.m_buttonBreak.SetActive( _IsActive );
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
        GetComponent<Rigidbody>().useGravity        = !isEnable;

        if (isEnable)
        {
            transform.localRotation = Quaternion.identity;
            transform.localPosition = Vector3.zero;
        }
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
 
    [ Command ]
    void    CmdRidingVehicle( NetworkInstanceId _NetID, NetworkInstanceId _TargetNetID, bool _Enable )
    {
        RpcRidingVehicle( _NetID, _TargetNetID, _Enable );
    }
    [ Command ]
    public void    CmdPlaceDrone( NetworkInstanceId _NetID )
    {
        RpcPlaceDrone( _NetID );
    }
    
    [ ClientRpc ]
    void    RpcRidingVehicle( NetworkInstanceId _NetID, NetworkInstanceId _TargetNetID, bool _Enable )
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