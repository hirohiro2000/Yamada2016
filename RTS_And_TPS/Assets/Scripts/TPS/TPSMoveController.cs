
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

    // アニメーション用
    TPS_AnimationController m_animationController = null;

    // 加速用
  	float   avoidStepPower  = 15.0f;
  	float   boosterPower    = 3.0f;
	float   maxDrivingPower = 45.0f;
    Vector3 impluseForce    = Vector3.zero;
	float   dampRate        = 2.0f;         

    // ２回連続に押下したかの判定用
    class DoublePress
    {
        public enum STATE { eIdle, ePress, eNotPress, eDoublePressDown, eDoublePress }

        KeyCode m_key;
        float   m_timer;
        STATE   m_state;
        public DoublePress( KeyCode key )
        {
            m_key = key;
            m_timer = 0.0f;
            if ( Input.GetKey(m_key) )  m_state = STATE.ePress;
            else                        m_state = STATE.eIdle;
        }

        public  void Update()
        {
            switch (m_state)
            {
                case STATE.eIdle:               IdleProc();             break;
                case STATE.ePress:              PressProc();            break;
                case STATE.eNotPress:           NotPressProc();         break;
                case STATE.eDoublePressDown:    DoublePressDownProc();  break;
                case STATE.eDoublePress:        DoublePressProc();      break;
                default:break;
            }
        }
        private void IdleProc()
        {
            if ( Input.GetKey(m_key) )
            {
                m_state = STATE.ePress;
                m_timer = 0.0f;
            }
        }
        private void PressProc()
        {
            m_timer += Time.deltaTime;
            if ( Input.GetKeyUp(m_key) )
            {
                if (m_timer > 0.2f)
                {
                    m_state = STATE.eIdle;
                }
                else
                {
                    m_state = STATE.eNotPress;
                }

                m_timer = 0.0f;
            }
        }
        private void NotPressProc()
        {
            if (Input.GetKey(m_key))
            {
                m_state = STATE.eDoublePressDown;
            }

            m_timer += Time.deltaTime;
            if (m_timer > 0.2f)
            {
                m_state = STATE.eIdle;
            }

        }
        private void DoublePressDownProc()
        {
            m_state = STATE.eDoublePress;
        }
        private void DoublePressProc()
        {
            if ( Input.GetKey(m_key) == false )
            {
                m_state = STATE.eIdle;
            }
        }

        public STATE GetState()         { return m_state;   }
        public KeyCode GetKeyCode()     { return m_key;     }

    }
    DoublePress[] m_doublePressKeys;

	// Use this for initialization
	void Start()
	{
        m_rIdentity =   GetComponent< NetworkIdentity >();

        m_doublePressKeys = new DoublePress[4];
        m_doublePressKeys[0] = new DoublePress( KeyCode.W );
        m_doublePressKeys[1] = new DoublePress( KeyCode.S );
        m_doublePressKeys[2] = new DoublePress( KeyCode.A );
        m_doublePressKeys[3] = new DoublePress( KeyCode.D );

        m_animationController = GetComponent< TPS_AnimationController >();

    }

	// Update is called once per frame
	void Update()
	{
        //  自分のキャラクター以外は処理を行わない
        if( !m_rIdentity.isLocalPlayer )    return;

        if ( Camera.main == null )          return;
        Transform camTransform = Camera.main.transform;

		Vector3 forward = ( transform.position - camTransform.position );
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
                
        // 加速処理（ブースト＆ステップ）
        //UpdateAdjustMoveForce( forward, right );

        // アニメーション
        if ( inputDir.sqrMagnitude > 0.0f )
        {
            m_animationController.ChangeStateMove();
        }
        else
        {
            m_animationController.ChangeStateIdle();
        }

    }

    void UpdateAdjustMoveForce( Vector3 forward, Vector3 right )
    {
        // キーの更新
        foreach (var key in m_doublePressKeys)
        {
            key.Update();
        }

        // 加速方向（上下左右）
        Vector3[] directions = {
            forward,
            -forward,
            -right,
            right
        };

        // ステップによる力取得
        Vector3 impluse = AvoidPower( directions );

        // 
        impluseForce += impluse;

        // 上限を超えないようclamp処理
        if (impluseForce.sqrMagnitude > maxDrivingPower * maxDrivingPower)
        {
            impluseForce = impluseForce.normalized * maxDrivingPower;
        }

        // ブーストダッシュによる力取得
        Vector3 velocity = BoostPower( directions );

        // 移動処理
        Vector3 totalForce = impluseForce + velocity;   // インパルス + 等加速運動
        float   minPower   = 0.5f*0.5f;                 // ※適当な値を設定しています
        if (totalForce.sqrMagnitude > minPower )
        {
            characterMover.AddSpeed(totalForce);
        }

        // ステップ力の減速処理
        if (impluseForce.sqrMagnitude > 0.5f * 0.5f)
        {
            impluseForce = Vector3.Lerp(impluseForce, Vector3.zero, dampRate * Time.deltaTime);
        }


    }
    Vector3 AvoidPower(Vector3[] directions)
    {
        Vector3 outVec = Vector3.zero;
        for (int i = 0; i < m_doublePressKeys.Length; i++)
        {
            DoublePress key = m_doublePressKeys[i];

            if (key.GetState() == DoublePress.STATE.eDoublePressDown)
            {
                outVec += directions[i] * avoidStepPower;
            }
        }
        return outVec;
    }
    Vector3 BoostPower(Vector3[] directions)
    {
        Vector3 outVec = Vector3.zero;

        for (int i = 0; i < m_doublePressKeys.Length; i++)
        {
            DoublePress key = m_doublePressKeys[i];

            if (key.GetState() == DoublePress.STATE.eDoublePress)
            {
                outVec += directions[i] * boosterPower;
            }
        }

        return outVec;

    }
    
    	//敵との衝突用
	public void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (hit.gameObject.layer == LayerMask.NameToLayer("Field"))
			return;
		if(Mathf.Abs(hit.normal.y) < 0.5f )
		{
			Vector3 dir = hit.normal;
			dir.y = .0f;
			dir.Normalize();
			transform.position = transform.position + dir * 0.2f;
        }
    }

}
