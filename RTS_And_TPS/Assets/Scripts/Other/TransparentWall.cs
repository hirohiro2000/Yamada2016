using UnityEngine;
using System.Collections;

public class TransparentWall : MonoBehaviour
{
	private GameObject	m_walls			= null;
	private GameObject	m_floors		= null;
	private GameObject	m_prev			= null;
	private const float m_height		= 10.0f;

	// Use this for initialization
	void Start ()
	{
		m_walls		= transform.FindChild("Block_ShellA").gameObject;
		m_floors	= transform.FindChild("Block_ShellB").gameObject;
	}
	
	// Update is called once per frame
	void Update()
	{
		var target	= GameObject.Find("RTSPlayer_Net(Clone)");
		if( target == null ) return;

		if( target.transform.position.y > m_height )
		{
			ChangeBlocksEnable( true );
			ChangeAlphaForRayCast();
		}
		else
		{
			ChangeBlocksEnable( false );
		}	
	}


	//-------------------------------------------------------------------
	//
	//-------------------------------------------------------------------
	void ChangeBlocksEnable( bool enable )
	{
		foreach ( var renderer in m_walls.transform.GetComponentsInChildren<Renderer>() )
		{
			renderer.enabled = enable;
		}
		foreach ( var renderer in m_floors.transform.GetComponentsInChildren<Renderer>() )
		{
			renderer.enabled = enable;
		}
	}


	//-------------------------------------------------------------------
	//
	//-------------------------------------------------------------------
	void ChangeAlphaForRayCast()
	{
		//	test
		var origin	= GameObject.Find("RTSCamera_Shell");
		var target	= GameObject.Find("RTSPlayer_Net(Clone)");

		if( origin == null ) return;
		if( target == null ) return;

		var vector	= target.transform.position - origin.transform.position;

        Vector3		dir			= transform.TransformDirection( vector.normalized );
		RaycastHit	info		= new RaycastHit();
        int         layerMask   = LayerMask.GetMask( "Field" );

        if( Physics.Raycast( origin.transform.position, dir, out info, vector.magnitude, layerMask ))
		{
			ResetPrevBlockState();

			//	表示しない場合
			//info.collider.gameObject.GetComponent<Renderer>().enabled = false;

			//	透過する場合
			var color = info.collider.gameObject.GetComponent<MeshRenderer>().material.color;
			info.collider.gameObject.GetComponent<MeshRenderer>().material.color = new Color( color.r, color.g, color.b, 0.5f );

			m_prev = info.collider.gameObject;
		}
		else
		{
			ResetPrevBlockState();
		}
	}
	void ResetPrevBlockState()
	{
		if( m_prev == null )
			return;

		//	表示
		//m_prev.GetComponent<Renderer>().enabled = true;

		//	色
		var color = m_prev.GetComponent<MeshRenderer>().material.color;
		m_prev.GetComponent<MeshRenderer>().material.color = new Color( color.r, color.g, color.b, 1.0f );

		m_prev = null;
	}
}