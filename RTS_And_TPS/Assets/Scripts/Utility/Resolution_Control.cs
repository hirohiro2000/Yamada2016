using UnityEngine;
using System.Collections;

public class Resolution_Control : MonoBehaviour {

	// Use this for initialization
	void    Start()
    {
	    //  画面設定 
        //if( Screen.width >= 1024 ){
        //    Screen.SetResolution( 1024, 576, false );
        //}
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
