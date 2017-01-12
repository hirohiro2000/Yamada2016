
using UnityEngine;
using System.Collections;

public class ParentSetter : MonoBehaviour {

    public  string  c_ParentName    =   "";

	// Use this for initialization
    void    Awake()
    {
        //  親を探す
        GameObject  rObj    =   GameObject.Find( c_ParentName );
        if( !rObj ) return;

        //  親を設定
        transform.SetParent( rObj.transform );
    }
}
