using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MechanocalGirl : MonoBehaviour 
{
	private GameObject			m_putResourceGuide	= null;

	public	GameObject			m_resActionBar	= null;
	private GameObject			m_actionBar		= null;

	private const KeyCode		m_putKey		= KeyCode.Z;
	private const KeyCode		m_createKey		= KeyCode.X;

	private enum ActionState
	{
		Common,
		PutResource,
		CreateResource,
	}
	private ActionState			m_actionState = ActionState.Common;


	// Use this for initialization
	void Start ()
	{
		m_putResourceGuide				= GameObject.Find("PutResourceGuide");

		m_actionBar						= Instantiate( m_resActionBar );
		m_actionBar.transform.parent	= GameObject.Find("Canvas").transform;

		ChangeActionBarState( false );
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		switch( m_actionState )
		{
		case ActionState.Common:			IsMovedByKey(); break;
		case ActionState.PutResource:		PutResource();	break;
		case ActionState.CreateResource:	CreateResource();break;
		}
	}

	//
	void IsMovedByKey()
	{		
		//	move
		float v = Input.GetAxis ("Vertical");
		float h = Input.GetAxis ("Horizontal");

		Vector3 cameraForward 	= Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
		Vector3 direction 		= cameraForward * v + Camera.main.transform.right * h;
		direction.Normalize();

		float axis				= ( Mathf.Abs(v) > Mathf.Abs(h) )? Mathf.Abs(v) : Mathf.Abs(h);
		float moveSpeed 		= 20.0f;
		transform.localPosition += direction * axis * moveSpeed * Time.fixedDeltaTime;

		
		//	rotate
		Vector3 animDir 		= direction;
		animDir.y 				= 0;

		if ( animDir.sqrMagnitude > 0.001f )
		{
			Vector3 newDir 		= Vector3.RotateTowards( transform.forward, animDir, 10.0f*Time.fixedDeltaTime, 0f );
			transform.rotation 	= Quaternion.LookRotation( newDir );
		}


		//	change state
		if( Input.GetKeyDown( m_putKey ) )
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
			m_putResourceGuide.GetComponent<PutResourceGuide>().AddResource();
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

	//
	void ChangeActionBarState( bool enable )
	{
		m_actionBar.GetComponent<ValueSlider>().m_cur	= 0.0f;
		m_actionBar.active								= enable;
	}
}