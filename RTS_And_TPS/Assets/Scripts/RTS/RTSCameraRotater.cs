﻿using UnityEngine;
using System.Collections;

//右クリック中にマウスで回転
[RequireComponent(typeof(RTSCamera))]
[RequireComponent(typeof(Camera))]
public class RTSCameraRotater : MonoBehaviour {

	RTSCamera rtsCamera;
	Camera camera;
	// Use this for initialization
	void Awake () {
		rtsCamera = GetComponent <RTSCamera>();
		camera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKey(KeyCode.Mouse1) == false)
			return;
		//ホイールでズーム
		rtsCamera.m_targetDistance -= Input.GetAxis("Mouse ScrollWheel") * 30.0f;

		//マウス移動で回転
		rtsCamera.m_dir.Normalize();

		Vector3 rotateVector = Quaternion.Euler(Input.GetAxis("Mouse Y") * 2.0f, Input.GetAxis("Mouse X") * 2.0f, .0f) * Vector3.forward;

		rtsCamera.m_dir = (Quaternion.LookRotation(rtsCamera.m_dir) * rotateVector).normalized;
	}
}
