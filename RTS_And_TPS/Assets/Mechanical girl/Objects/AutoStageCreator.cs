using UnityEngine;
using System.Collections;

public class AutoStageCreator : MonoBehaviour
{
	public float		m_range = 100.0f;
	public int			m_num = 100;
	public GameObject	m_wallA;
	public GameObject	m_wallB;
	public GameObject	m_wallC;

	// Use this for initialization
	void Start ()
	{
		GameObject[] wall = new GameObject[3]{ m_wallA, m_wallB, m_wallC };

		for( int i=0; i<m_num; ++i )
		{
			GameObject g = Instantiate( wall[ Random.Range(0,2) ] );
			g.transform.SetParent( transform );
			g.transform.position = new Vector3( Random.Range(-m_range,m_range), g.transform.position.y, Random.Range(-m_range,m_range) );
			g.transform.rotation = Quaternion.AngleAxis( 360.0f/Random.Range(1,8), Vector3.up );
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	}
}
