
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class GirlController : NetworkBehaviour 
{
//	private	GameObject			m_buttonOk					= null;
//	private	GameObject			m_buttonCancel				= null;
//	private	GameObject			m_buttonLevel				= null;
//	private	GameObject			m_buttonBreak				= null;
//	private	GameObject			m_towerInfoPanel			= null;
    private UIGirlTaskSelect    m_uiGirlTaskSelect          = null;

	private ResourceInformation	m_resourceInformation		= null;
	private ResourceCreator		m_resourceCreator			= null;
	private ItemController		m_itemCntroller				= null;
    private Rigidbody           m_rRigid                    = null;
    private RTS_PlayerAnimationController m_animationController = null;

	private const KeyCode		m_okKey						= KeyCode.J;
	private const KeyCode		m_cancelKey					= KeyCode.L;
	private const KeyCode		m_breakKey					= KeyCode.K;

	public float				m_moveSpeed					= 1.0f;
    public float                m_LiftingForce              = 1.0f;

	private enum ActionState
	{
		Common,
		CreateResource,
		ConvertResource,
	}
	private ActionState			m_actionState	= ActionState.Common;
    
    private bool                m_isMoveByKey   = false;
    private NavMeshAgent        m_navAgent      = null;
    
	// Use this for initialization
	void Start ()
	{
		GameObject g = GameObject.Find("RTS_HUD");
        m_uiGirlTaskSelect				= g.GetComponent<UIGirlTaskSelect>();
        m_uiGirlTaskSelect.m_rGirl                   = this.gameObject;
        m_uiGirlTaskSelect.m_rResourceInformation    = GameObject.Find("ResourceInformation").GetComponent<ResourceInformation>();
        m_uiGirlTaskSelect.m_rItemCntroller          = GetComponent<ItemController>();
        m_uiGirlTaskSelect.Clear();

//		m_buttonOk						= GameObject.Find("Canvas").transform.FindChild("Button_OK").gameObject;
//		m_buttonCancel					= GameObject.Find("Canvas").transform.FindChild("Button_Cancel").gameObject;
//		m_buttonLevel					= GameObject.Find("Canvas").transform.FindChild("Button_Level").gameObject;
//		m_buttonBreak					= GameObject.Find("Canvas").transform.FindChild("Button_Break").gameObject;
//		m_towerInfoPanel				= GameObject.Find("Canvas").transform.FindChild("Tower_Info").gameObject;
//		UserLog.Kawaguchi(m_towerInfoPanel);

		m_resourceInformation			= GameObject.Find("ResourceInformation").GetComponent<ResourceInformation>();
		m_resourceCreator				= GameObject.Find("ResourceCreator").GetComponent<ResourceCreator>();
		m_itemCntroller					= GetComponent<ItemController>();
        m_rRigid                        = GetComponent< Rigidbody >();

        // 適当に
        m_navAgent                      = gameObject.AddComponent<NavMeshAgent>();
        m_navAgent.acceleration = float.MaxValue;
        m_navAgent.angularSpeed = float.MaxValue;
        m_navAgent.stoppingDistance = 1.0f;
        m_navAgent.Warp( transform.position );
        m_navAgent.ResetPath();
        
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
        if( !isLocalPlayer ) return;

        //  座標調整（いまだけ）
        {
            //  飛ぶ
            if (Input.GetKey(KeyCode.M))
            {
                m_rRigid.AddForce(Vector3.up * m_LiftingForce * Time.deltaTime * 60.0f);
                if (m_navAgent.enabled)
                {
                    m_navAgent.ResetPath();
                    m_navAgent.enabled = false;
                }
            }

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
        


		switch( m_actionState )
		{
		case ActionState.Common:			UpdateCommon();		break;
		case ActionState.CreateResource:	CreateResource();	break;
		case ActionState.ConvertResource:	ConvertResource();	break;
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

		float axis				= ( Mathf.Abs(v) > Mathf.Abs(h) )? Mathf.Abs(v) : Mathf.Abs(h);
		transform.localPosition += direction * axis * m_moveSpeed * Time.deltaTime;

		
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
            Vector3     mousePosition   = Input.mousePosition;
            Ray         ray             = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit  hit             = new RaycastHit();
            int         layerMask       = LayerMask.GetMask( "Field" );

            if ( Physics.Raycast(ray, out hit, float.MaxValue, layerMask) )
            {                            
                m_navAgent.enabled      = true;
                m_navAgent.speed        = m_moveSpeed;

                NavMeshPath path = new NavMeshPath();
                if ( m_navAgent.CalculatePath( m_resourceInformation.ComputeGridPosition( hit.point ), path )) ;
                {
                    m_navAgent.SetPath(path);
                }
            }
        }

    }


	void UpdateCommon()
	{
//		m_buttonOk.SetActive(false);
//		m_buttonLevel.SetActive(false);
//		m_buttonBreak.SetActive(false);
//		m_buttonCancel.SetActive(false);
//		m_towerInfoPanel.SetActive(false);
        m_uiGirlTaskSelect.Clear();
		m_resourceCreator.SetGuideVisibleDisable();

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
//			m_buttonLevel.SetActive(true);
//			m_buttonBreak.SetActive(true);
//			m_buttonCancel.SetActive(true);
			m_actionState = ActionState.ConvertResource;
		}
		else
		{
//			m_buttonOk.SetActive( true );
//			m_buttonCancel.SetActive( true );
//			m_towerInfoPanel.SetActive(true);
			m_actionState = ActionState.CreateResource;
		}
	}
	void CreateResource()
	{
		var forcusID	= m_itemCntroller.GetForcus();
		var forcusParam = m_itemCntroller.GetForcusResourceParam();

//		//	リソースのUI設定
//		m_buttonOk.transform.FindChild("Point").GetComponent<Text>().text = "-" + forcusParam.m_createCost.ToString();
//		m_towerInfoPanel.transform.FindChild("Kind").GetComponent<Text>().text = "種類:　　　" + forcusParam.m_name;
//		m_towerInfoPanel.transform.FindChild("Summary").GetComponent<Text>().text = "概要:　　　" + forcusParam.m_summary;
//		m_towerInfoPanel.transform.FindChild("Power").GetComponent<Text>().text = "攻撃力:　　" + forcusParam.GetLevelParam(0).power;
//		m_towerInfoPanel.transform.FindChild("Interval").GetComponent<Text>().text = "発射間隔:　" + forcusParam.GetLevelParam(0).interval + "秒/発";
		
		//	リソースの範囲表示更新
		m_resourceCreator.UpdateGuideResource( forcusID, transform.position );
		m_resourceCreator.UpdateGuideRange( forcusID, transform.position );

		//	ステート更新
        UIGirlTaskSelect.RESULT uiResult = m_uiGirlTaskSelect.ToSelectTheCreateResource();
		if( ( Input.GetKeyDown( m_okKey )  || uiResult == UIGirlTaskSelect.RESULT.eOK ) &&
			m_itemCntroller.CheckWhetherTheCostIsEnough() )
		{
			m_resourceCreator.AddResource( forcusID );
			m_itemCntroller.AddResourceCost( -forcusParam.GetCreateCost());
			m_actionState = ActionState.Common;
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
        if( !param ){
            m_actionState   =   ActionState.Common;
            return;
        }

//		//	リソースのUI設定
//		m_buttonLevel.transform.FindChild("Point").GetComponent<Text>().text = "-" + param.GetCurLevelParam().upCost.ToString();
//		m_buttonBreak.transform.FindChild("Point").GetComponent<Text>().text = "+" + param.m_breakCost.ToString();

		//	リソースの範囲表示更新
		m_resourceCreator.UpdateGuideRange( transform.position );

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
		if( Input.GetKeyDown( m_breakKey ) || uiResult == UIGirlTaskSelect.RESULT.eBreak )
		{
			m_actionState = ActionState.Common;
			m_itemCntroller.AddResourceCost( m_itemCntroller.GetForcusResourceParam().GetBreakCost() );
            CmdBreakResource( transform.position );
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
}