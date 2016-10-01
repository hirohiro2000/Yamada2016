using UnityEngine;
using System.Collections;

public class TPSJumpController : MonoBehaviour {

	[SerializeField]
	TPSLandingChecker landingChecker;

	[SerializeField]
	float Power;

	[SerializeField]
	float maxEnableTime = 0.1f;

	float cntEnableTime;

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
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		cntEnableTime -= Time.deltaTime;
		if(cntEnableTime < .0f)
		{
			cntEnableTime = .0f;
		}

        if (Input.GetButtonDown("Jump"))
		{
			cntEnableTime = maxEnableTime;
		}

		if(cntEnableTime > .0f)
		{

            if (landingChecker.isLanding == true)
			{
				rigidBody.velocity = Vector3.up * Power;
				cntEnableTime = .0f;
            }
		}
    }
}
