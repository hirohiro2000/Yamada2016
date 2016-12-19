using UnityEngine;
using System.Collections;

public class EmissionChild : MonoBehaviour {

    public  GameObject  c_Emission  =   null;

    void    Awake()
    {
        if( !c_Emission )   return;

        GameObject  rObj    =   Instantiate( c_Emission );
        Transform   rTrans  =   rObj.transform;
        rTrans.position     =   transform.position;
        rTrans.parent       =   transform;
    }
}
