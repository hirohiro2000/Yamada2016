using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Razer_HitControl : MonoBehaviour {

    private LinkManager         m_rLinkManager  =   null;

	// Use this for initialization
	void    Start()
    {
	    m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );
	}
	
	// Update is called once per frame
	void    Update()
    {
	    
	}

    void    OnTriggerEnter( Collider _Collider )
	{
        Transform   rTrans  =   _Collider.transform;
        if( rTrans.tag != "Enemy" )     return;

        TPS_Enemy   rEnemy  =   rTrans.GetComponent< TPS_Enemy >();
        if( !rEnemy )   rEnemy  =   rTrans.GetComponentInParent< TPS_Enemy >();
        if( !rEnemy )                   return;

        ResourceParameter  rParam  =   GetComponent< ResourceParameter >();
        if( !rParam )   rParam  =   transform.parent.GetComponentInParent< ResourceParameter >();
        if( !rParam )                   return;

        //  ダメージを与える（サーバーのみ）
        if( NetworkServer.active ){
            rEnemy.GiveDamage( rParam.GetCurLevelParam().power );
        }
	}
}
