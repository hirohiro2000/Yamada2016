using UnityEngine;
using System.Collections;

public class DefendedTower : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

	void OnCollisionEnter( Collision collision )
	{
		if( collision.gameObject.tag == "RTSEnemy" )
		{
			var a = collision.gameObject.GetComponent<CollisionParam>();
			var d = GetComponent<CollisionParam>();
			
			CollisionParam.ComputeDamage( a, ref d, true );
			Destroy( collision.gameObject );
		}
	}
}
