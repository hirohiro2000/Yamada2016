using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Tragetor : NetworkBehaviour
{
	private EnemyShell_Control  m_rEnemyShell       =   null;
	private	ResourceParameter	m_resourceParam		= null;

	// Use this for initialization
	void Start()
	{
        //  サーバーでのみ処理を行う
        if( !isServer ) return;

		m_rEnemyShell   =   GameObject.Find( "Enemy_Shell" ).GetComponent< EnemyShell_Control >();
		m_resourceParam = GetComponent<ResourceParameter>();
	}
	
	// Update is called once per frame
	void Update ()
	{
        //  サーバーでのみ処理を行う
        if( !isServer ) return;
	}

    public  void    UpdateRotation()
    {
        Transform trs = null;
        if( m_rEnemyShell.GetNearEnemyTransform( ref trs, transform.position, m_resourceParam.GetCurLevelParam().range ) )
        {
            Vector3 forward = trs.position - transform.position;

            forward.y   =   0.0f;
            forward.Normalize();
        
            transform.rotation  =   Quaternion.LookRotation( forward );
        }
    }
}
