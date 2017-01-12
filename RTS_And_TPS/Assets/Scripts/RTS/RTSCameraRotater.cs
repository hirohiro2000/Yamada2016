using UnityEngine;
using System.Collections;

//右クリック中にマウスで回転
[RequireComponent(typeof(RTSCamera))]
public class RTSCameraRotater : MonoBehaviour {

	RTSCamera rtsCamera;
	// Use this for initialization
	void Awake () {
		rtsCamera = GetComponent <RTSCamera>();
	}
	
	// Update is called once per frame
	void    Update()
    {
		//ホイールでズーム
		rtsCamera.m_targetDistance -= Input.GetAxis("Mouse ScrollWheel") * 30.0f;

		//マウス移動で回転
        if( Input.GetMouseButton( 2 ) ){
		    rtsCamera.m_dir.Normalize();

		    Vector3 rotateVector = Quaternion.Euler(Input.GetAxis("Mouse Y") * 2.0f, Input.GetAxis("Mouse X") * 2.0f, .0f) * Vector3.forward;

		    rtsCamera.m_dir = (Quaternion.LookRotation(rtsCamera.m_dir) * rotateVector).normalized;
        }
	}
}
