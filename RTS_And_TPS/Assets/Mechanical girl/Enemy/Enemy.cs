using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public Vector3 m_targert;


	//test
	private int m_hp = 10;



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

	void OnCollisionEnter( Collision collision )
	{
		if( collision.gameObject.name == "Tower")
			Destroy( gameObject );

		if( collision.gameObject.tag == "TurretBullet")
		{
			m_hp--;
			 
			if( m_hp <= 0 )
				Destroy( gameObject );
		}
	}

}
