using UnityEngine;
using System.Collections;

public class AutoStageCreator : MonoBehaviour
{
	public float		m_range = 100.0f;
	public int			m_num	= 100;
	public float		m_gridSplitSpaceScale = 10.0f;
	public GameObject	m_wallA;
	public GameObject	m_wallB;
	public GameObject	m_wallC;

	// Use this for initialization
	void Start ()
	{
		Create();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if( Input.GetKeyDown( KeyCode.Return ))
		{
			for( int i=0; i<transform.childCount; ++i )
			{
				Destroy( transform.GetChild(i).gameObject );
			}

			Create();
		}
	}

	void Create()
	{
		GameObject[] wall = new GameObject[3]{ m_wallA, m_wallB, m_wallC };

		for ( int i=0; i<m_num; ++i )
		{
			GameObject g = Instantiate( wall[ Random.Range(0,3) ] );
			g.transform.SetParent( transform );
			g.transform.position = new Vector3( Random.Range(-10,10)*m_gridSplitSpaceScale, g.transform.position.y, Random.Range(-10,10)*m_gridSplitSpaceScale );
			//g.transform.position = new Vector3( Random.Range(-m_range,m_range), g.transform.position.y, Random.Range(-m_range,m_range) );
			g.transform.rotation = Quaternion.AngleAxis( 90.0f*Random.Range(0,3+1), Vector3.up );
		}
	}
}
