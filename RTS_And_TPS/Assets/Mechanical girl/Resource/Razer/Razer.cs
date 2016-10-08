using UnityEngine;
using System.Collections;

public class Razer : MonoBehaviour
{
	public  int m_interval	= 100;
	private int m_time		= 0;

	// Use this for initialization
	void Start ()
	{	
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if( ++m_time > m_interval )
		{
			m_time = 0;

			//	1はビーム
			transform.GetChild(1).gameObject.active = !transform.GetChild(1).gameObject.active;
		}
	}
}
