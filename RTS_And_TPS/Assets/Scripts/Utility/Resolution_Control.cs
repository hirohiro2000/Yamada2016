using UnityEngine;
using System.Collections;

public class Resolution_Control : MonoBehaviour {

    public  bool    c_OnStart   =   true;

	// Use this for initialization
	void    Start()
    {
	    //  画面設定 
        if( c_OnStart ){
            Screen.SetResolution( 1024, 576, false );
        }
	}
    void    Update()
    {
        if( Input.GetKey( KeyCode.LeftControl )
        &&  Input.GetKey( KeyCode.LeftShift )
        &&  Input.GetKeyDown( KeyCode.R ) ){
            Screen.SetResolution( 1024, 576, false );
        }
    }
}
