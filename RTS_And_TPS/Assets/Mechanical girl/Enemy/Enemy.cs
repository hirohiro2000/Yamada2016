using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public Vector3 m_targert;


	//test
	public int m_hp = 10;



	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		Vector3 v = m_targert - transform.position;
		v.Normalize();

		float speed = 1.0f;
		transform.position += v * speed * Time.deltaTime;
	}

	//↓仮
	void CollisionCommon( GameObject obj )
	{
		int power = obj.GetComponent<AttackParam>().m_attack;

		m_hp -= power;		
		GameObject.Find("NumberEffectFactory").GetComponent<NumberEffectFactory>().Create( transform.position, power );
		
		if( m_hp <= 0 )
			Destroy( gameObject );
	}
	void OnCollisionEnter( Collision collision )
	{
		if( collision.gameObject.name == "Tower")
			Destroy( gameObject );

		if( collision.gameObject.tag == "ResourceOn" )
			CollisionCommon( collision.gameObject );	
	}
	void OnCollisionStay( Collision collision )
	{
		if( collision.gameObject.tag == "ResourceStay" )
			CollisionCommon( collision.gameObject );
	}
}
