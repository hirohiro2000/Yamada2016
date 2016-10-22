using UnityEngine;
using System.Collections;

public class CharacterMover : MonoBehaviour {


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

	Vector3 totalSpeed;
	// Use this for initialization
	void Start () {
	
	}


	public void AddSpeed(Vector3 speed)
	{
		totalSpeed += speed;
    }
	// Update is called once per frame
	void Update ()
	{
		characterController.Move(totalSpeed * Time.deltaTime);
		totalSpeed = Vector3.zero;
    }
}
