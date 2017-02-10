using UnityEngine;
using System.Collections;

//右クリック中にマウスで回転
[RequireComponent(typeof(RTSCamera))]
public class RTSCameraRotater : MonoBehaviour {

	RTSCamera rtsCamera;

    public  Vector2     m_Sensitivity   =   Vector2.one;

	// Use this for initialization
	void    Awake()
    {
		rtsCamera = GetComponent <RTSCamera>();
	}
	
	// Update is called once per frame
	void    Update()
    {
		//  ホイールでズーム
		rtsCamera.m_targetDistance -= Input.GetAxis("Mouse ScrollWheel") * 30.0f;
		rtsCamera.m_targetDistance = Mathf.Clamp(rtsCamera.m_targetDistance, 3.0f, 100.0f);

        //マウス移動で回転
        if( Input.GetMouseButton( 1 ) ){
            rtsCamera.m_dir.Normalize();

            Vector3 rotateVector = Quaternion.Euler(Input.GetAxis("Mouse Y") * m_Sensitivity.y, Input.GetAxis("Mouse X") * m_Sensitivity.x, .0f) * Vector3.forward;

            rtsCamera.m_dir = (Quaternion.LookRotation(rtsCamera.m_dir) * rotateVector).normalized;

            //dirで制御
            {


            }
        }


        //
        while (rtsCamera.m_dir.y > 0.9f)
        { 
            rtsCamera.m_dir.y = 0.9f;
            rtsCamera.m_dir.Normalize();
        }
        while (rtsCamera.m_dir.y < -0.9f)
        {
            rtsCamera.m_dir.y = -0.9f;
            rtsCamera.m_dir.Normalize();
        }
         
	}
}
