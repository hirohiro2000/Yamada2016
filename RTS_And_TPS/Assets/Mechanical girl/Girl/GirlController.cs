using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GirlController : MonoBehaviour 
{
	public	GameObject			m_resActionBar	= null;
	private GameObject			m_actionBar		= null;

	private const KeyCode		m_putKey		= KeyCode.I;
	private const KeyCode		m_createKey		= KeyCode.K;
	private const KeyCode		m_levelUpKey	= KeyCode.L;

	
	private int					m_itemKindMax	= 0;
	private int					m_curItemFocus	= 0;


	private enum ActionState
	{
		Common,
		PutResource,
		CreateResource,
		LevelUpResource,
	}
	private ActionState			m_actionState	= ActionState.Common;


	// Use this for initialization
	void Start ()
	{
		m_actionBar						= Instantiate( m_resActionBar );
		m_actionBar.transform.SetParent	( GameObject.Find("Canvas").transform );

		ChangeActionBarState( false );

		m_itemKindMax					= GameObject.Find("ResourceInformation").transform.childCount;
	}
	

	//	Write to the FixedUpdate if including physical behavior
	void Update () 
	{
		switch( m_actionState )
		{
		case ActionState.Common:			UpdateCommon();		break;
		case ActionState.PutResource:		PutResource();		break;
		case ActionState.CreateResource:	CreateResource();	break;
		case ActionState.LevelUpResource:	LevelUpResource();	break;
		}
	}
	void FixedUpdate () 
	{
		switch( m_actionState )
		{
		case ActionState.Common:			MovedByKey();		break;
		}
	}


	//
	void MovedByKey()
	{		
		//	move
		float v = Input.GetAxis ("Vertical");
		float h = Input.GetAxis ("Horizontal");

		Vector3 cameraForward 	= Vector3.Scale( Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
		Vector3 direction 		= cameraForward * v + Camera.main.transform.right * h;
		direction.Normalize();

		float axis				= ( Mathf.Abs(v) > Mathf.Abs(h) )? Mathf.Abs(v) : Mathf.Abs(h);
		float moveSpeed 		= 20.0f;
		transform.localPosition += direction * axis * moveSpeed * Time.fixedDeltaTime;
		//transform.localPosition += direction * moveSpeed * Time.fixedDeltaTime;

		
		//	rotate
		Vector3 animDir 		= direction;
		animDir.y 				= 0;

		if ( animDir.sqrMagnitude > 0.001f )
		{
			Vector3 newDir 		= Vector3.RotateTowards( transform.forward, animDir, 10.0f*Time.fixedDeltaTime, 0f );
			transform.rotation 	= Quaternion.LookRotation( newDir );
		}		
	}
	void UpdateCommon()
	{
		//
		if( Input.GetKeyDown( KeyCode.Space ) )
		{
			m_curItemFocus++;
			m_curItemFocus %= m_itemKindMax;
		}


		//	change state
		if( Input.GetKeyDown( m_putKey ) &&
			!GetComponent<ResourceCreator>().CheckExistResource() )
		{
			m_actionState = ActionState.PutResource;
			m_actionBar.GetComponent<ValueSlider>().SetColor( Color.green );
			ChangeActionBarState( true );
		}
		if( Input.GetKeyDown( m_createKey ) )
		{
			m_actionState = ActionState.CreateResource;
			m_actionBar.GetComponent<ValueSlider>().SetColor( Color.cyan );
			ChangeActionBarState( true );
		}
		if( Input.GetKeyDown( m_levelUpKey ) &&
			GetComponent<ResourceCreator>().CheckExistResource() )
		{
			m_actionState = ActionState.LevelUpResource;
			m_actionBar.GetComponent<ValueSlider>().SetColor( Color.yellow );
			ChangeActionBarState( true );
		}
	}
	void PutResource()
	{
		ValueSlider slider = m_actionBar.GetComponent<ValueSlider>();
		slider.m_pos = transform.position;
		slider.m_cur += 1.0f;//kari


		//	change state
		if( slider.GetValue() >= 1.0f )
		{
			m_actionState = ActionState.Common;
			GetComponent<ResourceCreator>().AddResource();
			ChangeActionBarState( false );
			return;
		}
		if( Input.GetKeyUp( m_putKey ) )
		{
			m_actionState = ActionState.Common;
			ChangeActionBarState( false );
			return;
		}
	}
	void CreateResource()
	{
		ValueSlider slider = m_actionBar.GetComponent<ValueSlider>();
		slider.m_pos = transform.position;
		slider.m_cur += 1.0f;//kari


		//	change state
		if( slider.GetValue() >= 1.0f )
		{
			m_actionState = ActionState.Common;
			GetComponent<ItemController>().CreateItem();
			ChangeActionBarState( false );
			return;
		}
		if( Input.GetKeyUp( m_createKey ) )
		{
			m_actionState = ActionState.Common;
			ChangeActionBarState( false );
			return;
		}
	}
	void LevelUpResource()
	{
		ValueSlider slider = m_actionBar.GetComponent<ValueSlider>();
		slider.m_pos = transform.position;
		slider.m_cur += 1.0f;//kari


		//	change state
		if( slider.GetValue() >= 1.0f )
		{
			m_actionState = ActionState.Common;
			GetComponent<ResourceCreator>().LevelUpResource();
			ChangeActionBarState( false );
			return;
		}
		if( Input.GetKeyUp( m_levelUpKey ) )
		{
			m_actionState = ActionState.Common;
			ChangeActionBarState( false );
			return;
		}
	}


	//
	void ChangeActionBarState( bool enable )
	{
		m_actionBar.GetComponent<ValueSlider>().m_cur	= 0.0f;
		m_actionBar.SetActive( enable );
	}


	//
	public int GetItemFocus()
	{
		return m_curItemFocus;
	}
}