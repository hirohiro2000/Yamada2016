using UnityEngine;
using System.Collections;

public class DeviationCalculator : MonoBehaviour
{
	private Transform	m_eye = null;
	private Vector3		m_cur;
	private Vector3		m_prev;
	private Vector3		m_vel;

	// Use this for initialization
	void Start ()
	{
		m_prev	= m_cur = transform.position;
		m_eye	= transform.FindChild("Eye");
		m_vel	= Vector3.zero;
	}
	
	// Update is called once per frame
	void Update ()
	{
		m_prev	= m_cur;
		m_cur	= ( m_eye == null )? transform.position : m_eye.position;
		m_vel	= m_cur - m_prev;
	}

	public Vector3 Get()
	{
		return (( m_eye == null )? transform.position : m_eye.position ) + m_vel;
	}
}