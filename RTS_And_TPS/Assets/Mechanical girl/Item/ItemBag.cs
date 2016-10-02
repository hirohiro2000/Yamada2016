using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ItemBag : MonoBehaviour
{
	public	GameObject			m_frame			= null;

	private	List<GameObject>	m_frameList		= null;
	private int					m_kindMax		= 8;
	private int					m_curForcus		= 0;
	private int					m_resourcePoint = 999;

	// Use this for initialization
	void Start ()
	{
		m_frameList = new List<GameObject>();

		for( int i=0; i<m_kindMax; ++i )
		{
			GameObject add			= Instantiate( m_frame );
			add.transform.parent	= GameObject.Find("Canvas").transform;
			add.transform.position	= new Vector3( i*50 + 100, 100, 0 );

			m_frameList.Add( add );
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		UpdateScale();
		UpdateForcus();
	}

	void UpdateForcus()
	{
		if( Input.GetKeyUp( KeyCode.Space ) )
		{
			m_curForcus++;
			m_curForcus %= m_kindMax;
		}
	}

	void UpdateScale ()
	{
		const float forcus	= 1.1f;
		const float basic	= 0.8f;
		
		for( int i=0; i<m_kindMax; ++i )
		{
			m_frameList[i].transform.localScale = (m_curForcus==i)? new Vector3( forcus,forcus,forcus ):new Vector3( basic,basic,basic );
		}
	}

	void OnGUI ()
	{
        // Make a label that uses the "box" GUIStyle.
        GUI.Label ( new Rect (0,0,200,30), "debug-resource point->"+m_resourcePoint.ToString(), "box");
		GUI.Label ( new Rect (0,30,200,30), "debug-resource forcus->"+m_curForcus.ToString(), "box");
    }
}
