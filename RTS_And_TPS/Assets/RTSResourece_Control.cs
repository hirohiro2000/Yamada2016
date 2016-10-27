
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class RTSResourece_Control : NetworkBehaviour {

	// Use this for initialization
	void    Start()
    {
	    
	}
	public  override    void    OnStartServer()
    {
        transform.parent    =   GameObject.Find( "FieldResources" ).transform;
    }
    public  override    void    OnStartClient()
    {
        transform.parent    =   GameObject.Find( "FieldResources" ).transform;
    }


	// Update is called once per frame
	void    Update()
    {
	    
	}
}
