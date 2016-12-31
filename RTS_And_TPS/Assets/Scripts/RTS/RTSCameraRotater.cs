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
	void Update () {

		if (Input.GetKey(KeyCode.C) == false)
			return;
		//ホイールでズーム
		rtsCamera.m_targetDistance -= Input.GetAxis("Mouse ScrollWheel") * 30.0f;

		//マウス移動で回転
		rtsCamera.m_dir.Normalize();

//		Vector3 rotateVector = Quaternion.Euler(Input.GetAxis("Mouse Y") * 2.0f, Input.GetAxis("Mouse X") * 2.0f, .0f) * Vector3.forward;
		Vector3 rotateVector = Quaternion.Euler(0.0f, Input.GetAxis("Mouse X") * 2.0f, 0.0f) * Vector3.forward;

		rtsCamera.m_dir = (Quaternion.LookRotation(rtsCamera.m_dir) * rotateVector).normalized;
	}
}
