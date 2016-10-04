using UnityEngine;
using System.Collections;

public class TurretBullet : MonoBehaviour
{
	public float	m_spped = 1.0f;
	public Vector3	m_direction;

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.position += m_direction * m_spped;
	}
}
