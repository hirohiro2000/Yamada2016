using UnityEngine;
using System.Collections;

public class TPSMoveController : MonoBehaviour
{

	[SerializeField]
	float speed;

	[SerializeField, Range(0.1f, 1.0f)]
	float sensitivity;

	Vector3 velocity;

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

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{


		Vector3 forward = transform.forward;
		forward.y = .0f;
		forward.Normalize();

		Vector3 right = Vector3.Cross(Vector3.up, forward);
		right.Normalize();


		Vector3 inputDir = new Vector2();

		inputDir += right * (Input.GetAxis("Horizontal") * speed);
		inputDir += forward * (Input.GetAxis("Vertical") * speed);

		Vector3 addDir = new Vector3();
		addDir.x = (inputDir.x - /*rigidBody.*/velocity.x) * sensitivity;
		addDir.z = (inputDir.z - /*rigidBody.*/velocity.z) * sensitivity;

		/*rigidBody.*/
		velocity += addDir;

	}

	public void FixedUpdate()
	{
		velocity += Physics.gravity * Time.fixedDeltaTime;
		transform.position = transform.position + velocity * Time.fixedDeltaTime;
	}

	public void OnCollisionStay(Collision collision)
	{
		foreach( ContactPoint contact in collision.contacts)
		{
			if(contact.normal.y > 0.5f)
			{
				velocity.y = .0f;
			}
		}
	}


}
