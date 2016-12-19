﻿using UnityEngine;
using System.Collections;

public class HitSound_Control : MonoBehaviour {

    public  GameObject[]    c_Emission  =   null;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void    OnCollisionEnter( Collision _rCollision )
    {
        if( c_Emission == null )        return;
        if( c_Emission.Length == 0 )    return;

        for( int i = 0; i < c_Emission.Length; i++ ){
            GameObject  rEmission   =   c_Emission[ i ];
            if( !rEmission )    continue;

            GameObject  rObj        =   Instantiate( rEmission );
            Transform   rTrans      =   rObj.transform;
            rTrans.position         =   transform.position;
        }
    }
}
