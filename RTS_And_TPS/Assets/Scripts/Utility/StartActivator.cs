using UnityEngine;
using System.Collections;

public class StartActivator : MonoBehaviour {

    public  GameObject[]    c_Targets       =   null;
    public  bool            c_SetActive     =   false;

	// Use this for initialization
	void    Start()
    {
	    for( int i = 0; i < c_Targets.Length; i++ ){
            c_Targets[ i ].SetActive( c_SetActive );
        }
	}
}
