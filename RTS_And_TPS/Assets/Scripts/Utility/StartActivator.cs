using UnityEngine;
using System.Collections;

public class StartActivator : MonoBehaviour {

    public  enum    Mode{
        Awake,
        Start,
    };

    public  GameObject[]    c_Targets       =   null;
    public  bool            c_SetActive     =   false;
    public  Mode            c_Mode          =   Mode.Start;

	// Use this for initialization
    void    Awake()
    {
        if( c_Mode == Mode.Awake ){
            SetActive_Targets( c_SetActive );
            this.enabled    =   false;
        }
    }
	void    Start()
    {
	    if( c_Mode == Mode.Start ){
            SetActive_Targets( c_SetActive );
            this.enabled    =   false;
        }
	}
    void    SetActive_Targets( bool _IsActive )
    {
        for( int i = 0; i < c_Targets.Length; i++ ){
            c_Targets[ i ].SetActive( _IsActive );
        }
    }
}
