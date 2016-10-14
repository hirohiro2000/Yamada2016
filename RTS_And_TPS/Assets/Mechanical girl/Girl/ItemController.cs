using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ItemController : MonoBehaviour
{
	public	GameObject			m_frame			= null;

	private Transform			m_resourceInformation;
	private	List<GameObject>	m_frameList		= null;
	private int					m_kindMax		= 0;
	private int					m_curForcus		= 0;
	public  int					m_resourcePoint = 100;

	// Use this for initialization
	void Start ()
	{
		m_resourceInformation	= GameObject.Find("ResourceInformation").transform;
		m_kindMax				= m_resourceInformation.childCount;

		m_frameList = new List<GameObject>();

		for( int i=0; i<m_kindMax; ++i )
		{
			GameObject add			= Instantiate( m_frame );
			add.transform.SetParent( GameObject.Find("Canvas").transform );
			add.transform.position	= new Vector3( i*50 + 100, 50, 0 );
			add.transform.GetChild(0).GetComponent<Text>().text = "0";

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
		m_curForcus = GetComponent<GirlController>().GetItemFocus();
	}
	void UpdateScale ()
	{
		const float forcus	= 1.1f;
		const float basic	= 0.8f;
		
		for( int i=0; i<m_kindMax; ++i )
		{
			m_frameList[i].transform.localScale = ( m_curForcus==i )? new Vector3( forcus,forcus,forcus ):new Vector3( basic,basic,basic );
		}
	}


	//
	public int GetHaveCost()
	{
		return m_resourcePoint;
	}
	public void AddResourceCost( int cost )
	{
		m_resourcePoint += cost;
	}
	public void UseResourceCost()
	{
		m_resourcePoint -= m_resourceInformation.GetChild( m_curForcus ).GetComponent<ResourceParam>().m_createCost;
	}
	public bool CheckWhetherTheCostIsEnough()
	{
		return m_resourcePoint >= m_resourceInformation.GetChild( m_curForcus ).GetComponent<ResourceParam>().m_createCost;
	}


	//
	void OnGUI ()
	{
		Transform		g = GameObject.Find("ResourceInformation").transform.GetChild( m_curForcus );
		ResourceParam	b = g.GetComponent<ResourceParam>();

		GUIStyle		style = new GUIStyle();
		style.alignment = TextAnchor.MiddleLeft;

		GUIStyleState	state = new GUIStyleState();
		state.textColor = new Color( 1,1,1,1 );
		
		style.normal = state;
		
		GUI.Label ( new Rect (700,400,200,100), 
			"",
			"box");

        GUI.TextArea ( new Rect (700,400,200,100), 
			"　資源残量　" + m_resourcePoint.ToString() +
			"\n　作成費用　" + b.m_createCost.ToString() +
			"\n　作成時間　" + b.m_createTime.ToString() + "秒",
			style );
    }
}
