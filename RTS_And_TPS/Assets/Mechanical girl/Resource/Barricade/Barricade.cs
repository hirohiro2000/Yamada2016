using UnityEngine;
using System.Collections;

public class Barricade : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{	
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void OnCollision( Collision collision )
	{
		if ( collision.gameObject.tag != "RTSEnemy" )
			return;

		var a = collision.gameObject.GetComponent<CollisionParam>();
		var d = GetComponent<CollisionParam>();

		CollisionParam.ComputeDamage( a, ref d, false );

		if( d.m_hp <= 0 )
			Destroy( gameObject );
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
