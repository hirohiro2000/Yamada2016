
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

	Vector3 totalSpeed;
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
		totalSpeed.y = .0f;
        transform.position = transform.position + totalSpeed * Time.deltaTime;

        if (Physics.SphereCast(new Ray(transform.position, Vector3.down), 1.5f, 0.5f))
        {
			IsGrounded = true;
        }
		else
		{
			IsGrounded = false;
        }
		totalSpeed = Vector3.zero;
    }
}
