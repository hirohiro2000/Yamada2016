using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NumberEffectFactory : MonoBehaviour
{
	public GameObject	m_text;
	public float		m_numUpDpeed	= 1.0f;
	public int			m_numLife		= 60;
	
	class Data
	{
		public GameObject	obj;
		public int			life;
	}
	List<Data> m_data = null;

	
	// Use this for initialization
	void Start ()
	{
		m_data = new List<Data>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		//if( Input.GetKeyDown( KeyCode.Return ) )
		//{
		//	Create( new Vector3( 0,0,0 ), 5 );
		//}

		for( int i=0; i<m_data.Count; ++i )
		{
			if( --m_data[i].life <= 0 )
			{
				Destroy( m_data[i].obj );
				m_data.Remove( m_data[i] );
			}
			else
			{
				Vector3 c = m_data[i].obj.transform.position;
				m_data[i].obj.transform.position = new Vector3( c.x, c.y+m_numUpDpeed, c.z );
			}
		}
	}

	public void Create( Vector3 pos, int num )
	{
		float range = 20.0f;

		GameObject txt			= Instantiate( m_text );
		txt.transform.SetParent( GameObject.Find("Canvas").transform );
		txt.transform.position  = RectTransformUtility.WorldToScreenPoint ( Camera.main, pos );
		txt.transform.position += new Vector3( Random.Range(-range,range),Random.Range(-range,range),Random.Range(-range,range));
		txt.GetComponent<Text>().text = num.ToString();
		
		Data data	= new Data();
		data.obj	= txt;
		data.life	= m_numLife;
		
		m_data.Add( data );
	}
}
