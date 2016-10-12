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
			transform.GetChild(1).gameObject.SetActive( !transform.GetChild(1).gameObject.activeInHierarchy );
		}

		UpdateLevel();
	}

	void UpdateLevel()
	{	
		int			level		= GetComponent<ResourceParam>().m_level;
		float		addScale	= 0.5f * ( level-1 );
		Transform	beam		= transform.GetChild(1);

		beam.localScale = new Vector3( 0.1f+addScale, beam.localScale.y, 0.1f+addScale );
	}
}
