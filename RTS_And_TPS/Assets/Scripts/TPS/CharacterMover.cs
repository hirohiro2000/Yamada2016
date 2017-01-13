
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
	// Use this for initialization
	void    Start()
    {
	    m_rIdentity =   GetComponent< NetworkIdentity >();   
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

		jumpLockTime -= Time.deltaTime;

		IsGrounded = false;
		lastTotalSpeed = totalSpeed;

		totalSpeed = Vector3.zero;
    }

	void FixedUpdate()
	{
		transform.position = transform.position + lastTotalSpeed * Time.fixedDeltaTime + new Vector3(rigidBody.velocity.x,.0f, rigidBody.velocity.z) * -Time.fixedDeltaTime;
		rigidBody.velocity = new Vector3(.0f, rigidBody.velocity.y, .0f);
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
}
