using UnityEngine;
using System.Collections;

public class ReflectionBox_Control : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void    Update () {
	
	}

    void    OnTriggerEnter( Collider _rCollider )
    {
        Rigidbody   rRigid  =   _rCollider.GetComponent< Rigidbody >();
        if( !rRigid )   return;

        rRigid.velocity     =   -rRigid.velocity;
    }
}
