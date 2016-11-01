﻿using UnityEngine;
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

        CollisionParam  rParam  =   GetComponent< CollisionParam >();
        if( !rParam )   rParam  =   transform.parent.GetComponentInParent< CollisionParam >();
        if( !rParam )                   return;

        //  ダメージを与える（サーバーのみ）
        if( m_rLinkManager.isServer ){
            rEnemy.GiveDamage( rParam.m_attack );
        }
	}
}