using UnityEngine;
using System.Collections;

public class TPSJumpController : MonoBehaviour {

	[SerializeField]
	TPSLandingChecker landingChecker;

	[SerializeField]
	float Power;

	[SerializeField]
	float maxEnableTime = 0.1f;

	[SerializeField]
	float maxHoverTime = 0.1f;

	[SerializeField]
	float HoverPower = 0.1f;

	float cntHoverTime;

	bool isCanHover;

	float cntEnableTime;

	float fallPower;

	bool isJumped;

	bool beforeIsGrounded;

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


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		cntEnableTime -= Time.deltaTime;
		if(cntEnableTime < .0f)
		{
			cntEnableTime = .0f;
		}
		if(characterController.isGrounded == true || (characterController.collisionFlags & CollisionFlags.Below) != 0)
		{
			fallPower = 2.0f;
			isJumped = false;
			isCanHover = false;
        }
		else
		{
			fallPower += Time.deltaTime;
        }
		
        if (Input.GetButtonDown("Jump"))
		{
			//cntEnableTime = maxEnableTime;
			if (characterController.isGrounded == true || (characterController.collisionFlags & CollisionFlags.Below) != 0)
			{
				fallPower = -Power;
				cntHoverTime = maxHoverTime;
				isJumped = true;
            }

		}
		if (Input.GetButton("Jump") == false && characterController.isGrounded == false)
		{
			isCanHover = true;
        }

		if (Input.GetButton("Jump") == true && characterController.isGrounded == false && isCanHover == true)
		{
			if(cntHoverTime > .0f)
			{
				cntHoverTime -= Time.deltaTime;
				fallPower -= Time.deltaTime * HoverPower;
            }

		}

		//if(cntEnableTime > .0f)
		//{

		//          if (landingChecker.isLanding == true)
		//	{
		//		rigidBody.velocity = Vector3.up * Power;
		//		cntEnableTime = .0f;
		//          }
		//}

		//characterController.Move(Physics.gravity * fallPower * Time.deltaTime);
		characterMover.AddSpeed(Physics.gravity * fallPower);

		if ((characterController.isGrounded == false) && (beforeIsGrounded == true) && isJumped == false)
		{
			fallPower = .0f;
			characterMover.AddSpeed(Vector3.up * 2.0f);
		}
		beforeIsGrounded = characterController.isGrounded || (characterController.collisionFlags & CollisionFlags.Below) != 0;
    }


	void OnGUI()
	{
		GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "fallPower:" + fallPower);
	}

}
