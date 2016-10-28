using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResourceCommon : MonoBehaviour
{
	private CollisionParam	m_param	= null;

	// Use this for initialization
	void Start ()
	{
		m_param	= GetComponent<CollisionParam>();
	}

	//
	void OnDestroy()
	{
		var g = GameObject.Find("ResourceInformation");

		if ( g )
			g.GetComponent<ResourceInformation>().SetGridInformation( null, transform.position, false );
	}
	
	// Update is called once per frame
	void Update ()
	{
	}


	//----------------------------------------------------------------------------
	//
	//----------------------------------------------------------------------------
	void OnCollision( Collision collision )
	{
		if ( collision.gameObject.tag != "RTSEnemy" )
			return;

		var a = collision.gameObject.GetComponent<CollisionParam>();
		var d = m_param;

		CollisionParam.ComputeDamage( a, ref d, false );

		if( d.m_hp <= 0 )
		{ 
			Destroy( gameObject );
		}
	}
	void OnCollisionEnter( Collision collision )
	{
		OnCollision( collision );
	}
	void OnCollisionStay( Collision collision )
	{
		OnCollision( collision );
	}
}
