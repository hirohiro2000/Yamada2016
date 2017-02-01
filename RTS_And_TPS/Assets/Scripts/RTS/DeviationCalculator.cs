using UnityEngine;
using System.Collections;

public class DeviationCalculator : MonoBehaviour
{
	private Transform	m_eye = null;
	private Vector3		m_cur;
	private Vector3		m_prev;
	private Vector3		m_vel;

    private Vector3     m_PerSecond;
    private float       m_AverageSpeed;

	// Use this for initialization
	void Start ()
	{
		m_prev	= m_cur = transform.position;
		m_eye	= transform.FindChild("Eye");
		m_vel	= Vector3.zero;

        m_PerSecond     =   Vector3.zero;
        m_AverageSpeed  =   0.0f;
	}
	
	// Update is called once per frame
	void Update ()
	{
		m_prev	= m_cur;
		m_cur	= ( m_eye == null )? transform.position : m_eye.position;
		m_vel	= m_cur - m_prev;

        m_PerSecond     =   m_vel / Time.deltaTime;
        m_AverageSpeed  =   ( m_AverageSpeed + ( m_PerSecond / 60.0f ).magnitude ) / 2.0f;
	}

	public Vector3 Get()
	{
		return (( m_eye == null )? transform.position : m_eye.position ) + m_vel;
	}
    public  Vector3 GetCorrectionPoint( float _BulletSpeed, float _Distance )
    {
        float   arrivalTime =   _Distance / _BulletSpeed;
        Vector3 curPos      =   ( ( !m_eye )? transform.position : m_eye.position );
        Vector3 expect      =   curPos + m_vel.normalized * m_AverageSpeed * 60.0f * arrivalTime;

        return  expect;
    }
}