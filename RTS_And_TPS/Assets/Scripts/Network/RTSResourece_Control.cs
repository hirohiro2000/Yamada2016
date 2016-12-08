
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class RTSResourece_Control : NetworkBehaviour {
    [ SyncVar ]
    public  int     c_OwnerID   =   0;

	public  override    void    OnStartServer()
    {
        base.OnStartServer();

        transform.parent    =   GameObject.Find( "FieldResources" ).transform;
    }
    public  override    void    OnStartClient()
    {
        base.OnStartClient();

        transform.parent    =   GameObject.Find( "FieldResources" ).transform;
    }
}
