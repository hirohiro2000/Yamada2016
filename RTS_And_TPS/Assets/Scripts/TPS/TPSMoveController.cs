
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

    // ダッシュ用
  	float drivingPower      = 3.0f;
	float maxDrivingPower   = 15.0f;
	float dampRate          = 3.0f;
    Vector3 drivingForce    = Vector3.zero;
    float inputInterval     = 0.0f;
    int   dashState         = 0;

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


        // ダッシュ処理( 押下中 => 離す => 押下(インターバルで行えるかの判定) => 押下中(離すor少し時間が過ぎたら) => ０に戻る )
        switch (dashState)
        {
            case 0:
                {
                    if (Input.GetKeyDown(KeyCode.LeftShift))
                    {
                        //
                        drivingForce += inputDir.normalized * speed * drivingPower;
                        if (drivingForce.sqrMagnitude > maxDrivingPower * maxDrivingPower)
                            drivingForce = drivingForce.normalized * maxDrivingPower;
                       
                        inputInterval = 0.0f;
                        ++dashState;
                    }
                }
                break;
            case 1:
                {
                    inputInterval += Time.deltaTime;
                    if (inputInterval > 0.1f)
                    {
                        dashState = 0;
                    }
                }
                break;
            default: break;
        }

        if (drivingForce.sqrMagnitude > 0.5f * 0.5f)
        {
            characterMover.AddSpeed(drivingForce);
            drivingForce = Vector3.Lerp(drivingForce, Vector3.zero, dampRate * Time.deltaTime);
        }


	}
}
