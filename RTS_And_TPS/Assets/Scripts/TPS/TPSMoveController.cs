﻿
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class TPSMoveController : MonoBehaviour
{
    private NetworkIdentity m_rIdentity =   null;

	[SerializeField]
	float speed = .0f;

	//[SerializeField, Range(0.1f, 1.0f)]
	//float sensitivity = 0.1f;

	Vector3 inputDir;


	Rigidbody _rigidBody;
	Rigidbody rigidBody
	{
		get
		{
			if (_rigidBody == null)
			{
				_rigidBody = GetComponent<Rigidbody>();
			}
			return _rigidBody;
		}
	}

	CharacterMover _characterMover;
	CharacterMover characterMover
	{
		get
		{
			if (_characterMover == null)
			{
				_characterMover = GetComponent<CharacterMover>();
			}
			return _characterMover;
		}
	}

	CharacterController _characterController;
	CharacterController characterController
	{
		get
		{
			if (_characterController == null)
			{
				_characterController = GetComponent<CharacterController>();
			}
			return _characterController;
		}
	}


	// Use this for initialization
	void Start()
	{
        m_rIdentity =   GetComponent< NetworkIdentity >();
	}

	// Update is called once per frame
	void Update()
	{
        //  自分のキャラクター以外は処理を行わない
        if( !m_rIdentity.isLocalPlayer )    return;

		Vector3 forward = transform.forward;
		forward.y = .0f;
		forward.Normalize();

		Vector3 right = Vector3.Cross(Vector3.up, forward);
		right.Normalize();


		inputDir = Vector3.zero;

		inputDir += right * (Input.GetAxis("Horizontal") * speed);
		inputDir += forward * (Input.GetAxis("Vertical") * speed);

		//characterController.Move(inputDir * Time.deltaTime);
		//characterController.SimpleMove(inputDir);
		characterMover.AddSpeed(inputDir);

		//Vector3 addDir = new Vector3();
		//addDir.x = (inputDir.x - rigidBody.velocity.x) * sensitivity;
		//addDir.z = (inputDir.z - rigidBody.velocity.z) * sensitivity;

		//rigidBody.velocity += addDir;
	}
}
