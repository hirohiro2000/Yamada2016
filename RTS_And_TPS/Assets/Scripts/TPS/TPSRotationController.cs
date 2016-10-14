using UnityEngine;
using UnityEditor;
using System.Collections;

public class TPSRotationController : MonoBehaviour
{
	[SerializeField]
	Transform YAxisRotater;

	[SerializeField]
	Transform XAxisRotater;

	[SerializeField, Range(1.0f, 100.0f)]
	float YAxisRotSpeed;

	[SerializeField, Range(1.0f, 100.0f)]
	float XAxisRotSpeed;


	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if(Time.timeScale != .0f)
		{
			XAxisRotater.rotation *= Quaternion.AngleAxis(-Input.GetAxis("Mouse Y") * XAxisRotSpeed, Vector3.right);

			YAxisRotater.rotation *= Quaternion.AngleAxis( Input.GetAxis("Mouse X") * YAxisRotSpeed ,Vector3.up);

		}

	}
}