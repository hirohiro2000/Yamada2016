using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Tragetor : NetworkBehaviour
{
	private EnemyShell_Control  m_rEnemyShell       =   null;
	public	float			    m_forcusRange		=   30.0f;
	public	float			    m_targettingSpeed	=   0.5f;

	// Use this for initialization
	void Start()
	{
        //  サーバーでのみ処理を行う
        if( !isServer ) return;

		m_rEnemyShell   =   GameObject.Find( "Enemy_Shell" ).GetComponent< EnemyShell_Control >();
	}
	
	// Update is called once per frame
	void Update ()
	{
        //  サーバーでのみ処理を行う
        if( !isServer ) return;

        //Transform trs = null;
        //if( m_enemyFactory.GetNearEnemyTransform( ref trs, transform.position, m_forcusRange ) )
        //{
        //    Vector3 forward = trs.position - transform.position;
        //    forward.Normalize();

        //    transform.rotation = Quaternion.Slerp ( transform.rotation, Quaternion.LookRotation( forward ), m_targettingSpeed*Time.deltaTime );
        //}
	}

    public  void    UpdateRotation()
    {
        Transform trs = null;
        if( m_rEnemyShell.GetNearEnemyTransform( ref trs, transform.position, m_forcusRange ) )
        {
            Vector3 forward = trs.position - transform.position;

            forward.y   =   0.0f;
            forward.Normalize();
        
            transform.rotation  =   Quaternion.LookRotation( forward );
        }
    }
}
