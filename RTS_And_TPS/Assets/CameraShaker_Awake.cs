using UnityEngine;
using System.Collections;

public class CameraShaker_Awake : MonoBehaviour {

	// Use this for initialization
	void    Start()
    {
	    //  カメラシェイク 
        {
            Shaker_Control  rShaker     =   Camera.main.GetComponent< Shaker_Control >();
            if( rShaker ){
                Transform   rCamTrans   =   rShaker.transform;
                Vector3     vToCamera   =   ( Camera.main.transform.position - transform.position );
                Vector3     vShake      =   vToCamera.normalized;
                            vShake      =   rCamTrans.InverseTransformDirection( vShake );
                float       shakePower  =   20 / vToCamera.magnitude;
                            shakePower  =   Mathf.Min( shakePower, 1.0f );
                
                rShaker.SetShake( vShake, 1.0f, 0.25f, shakePower );
                rShaker.SetShake( Vector3.up, 1.5f, 0.25f, shakePower * 0.5f );
                rShaker.SetShake( Vector3.right, 2.5f, 0.25f, shakePower * 0.5f );
            }
        }
	}
}
