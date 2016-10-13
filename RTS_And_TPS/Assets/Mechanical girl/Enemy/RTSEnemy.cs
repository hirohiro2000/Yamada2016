using UnityEngine;
using System.Collections;

public class RTSEnemy : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
		GameObject esp = GameObject.Find("EnemySpawnPoints");
		transform.position = esp.transform.GetChild( Random.Range( 0, esp.transform.childCount )).transform.position;
		//transform.position += new Vector3( 0, -transform.localScale.y*0.5f, 0 );	

		GameObject dt = GameObject.Find("DefendedTower");
		GetComponent<NavMeshAgent>().Warp( transform.position );
        GetComponent<NavMeshAgent>().SetDestination( dt.transform.position );
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

	//↓仮
	void CollisionCommon( GameObject obj )
	{
		var a = GameObjectExtension.GetComponentInParentAndChildren< CollisionParam >( obj.gameObject );
		var d = GetComponent<CollisionParam>();

		CollisionParam.ComputeDamage( a, ref d, true );
	
		if ( d.m_hp <= 0 )
		{ 
			//	test
			int getcost = 10;
			GameObject.Find( "MechanicalGirl" ).GetComponent<ItemController>().AddResourceCost( getcost );
			GameObject.Find( "NumberEffectFactory" ).GetComponent<NumberEffectFactory>().Create( transform.position, getcost, Color.green );

			Destroy( gameObject );
		}
	}
	void OnCollisionEnter(Collision collision)
	{
		if ( collision.gameObject.tag == "ResourceOn")
			CollisionCommon( collision.gameObject );
	}
	void OnCollisionStay(Collision collision)
	{
		if ( collision.gameObject.tag == "ResourceStay" )
			CollisionCommon( collision.gameObject );
	}
}
