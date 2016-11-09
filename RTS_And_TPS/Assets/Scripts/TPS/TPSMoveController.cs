
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
    Vector3 prePoll         = Vector3.zero;     // 前回入力があった際の傾き
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
                    float axis = inputDir.sqrMagnitude;
                    if (axis > 2.0f)
                    {
                        ++dashState;
                        inputInterval = 0.0f;
                        prePoll = inputDir;
                    }
                }
                break;
            case 1:
                {
                    inputInterval += Time.deltaTime;
                    float axis = inputDir.sqrMagnitude;
                    if (axis < 1.0f)
                    {
                        ++dashState;
                    }
                }

                break;
            case 2:
                {
                    inputInterval += Time.deltaTime;
                    float axis = inputDir.sqrMagnitude;

                    if (axis > 3.0f && inputInterval < 1.5f && Vector3.Dot(inputDir, prePoll) > 0.0f)
                    {
                        ++dashState;
                        inputInterval = 0.0f;

                        //
                        drivingForce += inputDir.normalized * speed * drivingPower;
                        if (drivingForce.sqrMagnitude > maxDrivingPower * maxDrivingPower)
                            drivingForce = drivingForce.normalized * maxDrivingPower;
                    }
                    if (inputInterval > 1.5f)
                    {
                        dashState = 0;
                        inputInterval = 0.0f;
                    }

                }
                break;
            case 3:
                {
                    inputInterval += Time.deltaTime;
                    float axis = inputDir.sqrMagnitude;
                    if (axis < 1.0f || inputInterval > 0.5f)
                    {
                        dashState = 0;
                        inputInterval = 0.0f;
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
