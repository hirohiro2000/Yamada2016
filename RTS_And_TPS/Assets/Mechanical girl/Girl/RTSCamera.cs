using UnityEngine;
using System.Collections;

public class RTSCamera : MonoBehaviour 
{
	private GameObject  m_mechanicalGirl = null;

	[SerializeField, Range(5, 50)]
    public  float       m_targetDistance = 20.0f;

	[SerializeField, Range(-30, 30)]
    public  float       m_heightAdjust = 0.0f;

	// Use this for initialization
	void Start ()
	{
		m_mechanicalGirl = GameObject.Find ("MechanicalGirl");
	}
	
	// Update is called once per frame
	void Update () 
	{
		ChaseTarget ();
	}

	private void ChaseTarget()
	{
		Vector3 offset = new Vector3 ( m_targetDistance, m_targetDistance+m_heightAdjust, m_targetDistance );
		transform.position = m_mechanicalGirl.transform.position + offset;
		transform.LookAt ( m_mechanicalGirl.transform.position );
	}
}
