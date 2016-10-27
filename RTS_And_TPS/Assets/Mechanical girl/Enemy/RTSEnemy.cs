
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class RTSEnemy : NetworkBehaviour
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

        //  クライアント側ではナビメッシュを使用しない
        if( !isServer ){
            GetComponent< NavMeshAgent >().enabled  =   false;
        }
	}
    public  override    void    OnStartServer()
    {
        transform.parent    =   GameObject.Find( "EnemyFactory" ).transform;
    }
    public  override    void    OnStartClient()
    {
        transform.parent    =   GameObject.Find( "EnemyFactory" ).transform;
    }
	
	// Update is called once per frame
	void Update ()
	{
	}

	//	↓仮
	void CollisionCommon( GameObject obj )
	{
        //  当たり判定はサーバーでのみ行う
        if( !isServer ) return;

		var a = GameObjectExtension.GetComponentInParentAndChildren<CollisionParam>( obj.gameObject );
		var d = GetComponent<CollisionParam>();

		CollisionParam.ComputeDamage( a, ref d, true );

		if( d.m_hp <= 0 )
		{
			//	test
			int getcost = 5;

            ItemController  rItem   =   FunctionManager.GetAccessComponent< ItemController >( "RTSPlayer_Net(Clone)" );
            if( rItem ) rItem.AddResourceCost( getcost );
			//GameObject.Find( "MechanicalGirl" ).GetComponent<ItemController>().AddResourceCost( getcost );
			GameObject.Find("NumberEffectFactory").GetComponent<NumberEffectFactory>().Create( transform.position, getcost, Color.green );

			Destroy( gameObject );
		}
	}
	void OnCollisionEnter( Collision collision )
	{
		if ( collision.gameObject.tag == "ResourceOn")
			CollisionCommon( collision.gameObject );
	}
	void OnCollisionStay( Collision collision )
	{
		if ( collision.gameObject.tag == "ResourceStay")
			CollisionCommon(collision.gameObject);
	}
	void OnTriggerEnter( Collider collision )
	{
		if ( collision.gameObject.tag == "ResourceOn" )
			CollisionCommon( collision.gameObject );
	}
	void OnTriggerStay( Collider collision )
	{
		if ( collision.gameObject.tag == "ResourceStay" )
			CollisionCommon( collision.gameObject );
	}
}
