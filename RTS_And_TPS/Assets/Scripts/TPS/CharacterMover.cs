
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class CharacterMover : MonoBehaviour {

    private NetworkIdentity m_rIdentity =   null;

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

	bool IsGrounded = false;
	float jumpLockTime = .0f;


	Vector3 totalSpeed;
	Vector3 lastTotalSpeed;


    bool                m_IsFly             =   false;
    float               m_FlyTimer          =   0.0f;
    TPSJumpController   m_rJumpControl      =   null;

	// Use this for initialization
	void    Start()
    {
	    m_rIdentity         =   GetComponent< NetworkIdentity >();   
        m_rJumpControl      =   GetComponent< TPSJumpController >();
	}


	public void AddSpeed(Vector3 speed)
	{
		totalSpeed += speed;
    }
    public  Vector3  GetTotalSpeed()
    {
        return  totalSpeed;
    }
	public void JumpLock()
	{
		jumpLockTime = 0.3f;
		IsGrounded = false;
    }

	public bool isGrounded
	{
		get
		{
			//return characterController.isGrounded;
			return IsGrounded;
        }
	}

	// Update is called once per frame
	void Update ()
	{
        //  自分のキャラクター以外は処理を行わない
        if( !m_rIdentity.isLocalPlayer )    return;

		//rigidBody.velocity = new Vector3(totalSpeed.x, rigidBody.velocity.y, totalSpeed.z); 
		//totalSpeed.y = .0f;
		//transform.position = transform.position + totalSpeed * Time.deltaTime; 

        m_FlyTimer  -=  Time.deltaTime;
        m_FlyTimer  =   Mathf.Max( m_FlyTimer, 0.0f );

        if( IsGrounded
        &&  m_FlyTimer <= 0 ){
            //if( m_IsFly )   Debug.Log( "Down!" );
            SetFlay( false );
        }

		jumpLockTime -= Time.deltaTime;

		IsGrounded = false;
		lastTotalSpeed = totalSpeed;

		totalSpeed = Vector3.zero;
    }

	void FixedUpdate()
	{
        //  吹っ飛び中
        if( m_IsFly ){ 
            transform.position  +=  lastTotalSpeed * Time.fixedDeltaTime;
        }
        //  通常時
        else{
            transform.position = transform.position + lastTotalSpeed * Time.fixedDeltaTime + new Vector3(rigidBody.velocity.x,.0f, rigidBody.velocity.z) * -Time.fixedDeltaTime;
            rigidBody.velocity = new Vector3(.0f, rigidBody.velocity.y, .0f);
        }
	}

	void OnCollisionStay(Collision col)
	{
		if (!m_rIdentity.isLocalPlayer) return;

		if (jumpLockTime >= .0f) return;
		if (col.contacts[0].normal.y > 0.5f)
		{
			IsGrounded = true;
        }
	}

    //  吹っ飛ぶ
    public  void    SetFlay( bool _Enable )
    {
        if( m_IsFly == _Enable )    return;

        //  吹っ飛び開始
        if( _Enable ){
            m_IsFly                 =   true;
            m_FlyTimer              =   0.5f;
            IsGrounded              =   false;

            _rigidBody.useGravity   =   true;
            m_rJumpControl.enabled  =   false;
        }
        //  吹っ飛び終了
        else{
            m_IsFly                 =   false;

            _rigidBody.useGravity   =   false;
            m_rJumpControl.enabled  =   true;
        }
    }
}
