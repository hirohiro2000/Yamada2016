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
	private int					m_resourcePoint = 999;

	public struct Info
	{
		public int num;
	}
	private	List<Info>			m_infoList		= null;


	// Use this for initialization
	void Start ()
	{
		m_resourceInformation	= GameObject.Find("ResourceInformation").transform;
		m_kindMax				= m_resourceInformation.childCount;

		m_frameList = new List<GameObject>();

		for( int i=0; i<m_kindMax; ++i )
		{
			GameObject add			= Instantiate( m_frame );
			add.transform.parent	= GameObject.Find("Canvas").transform;
			add.transform.position	= new Vector3( i*50 + 100, 50, 0 );
			add.transform.GetChild(0).GetComponent<Text>().text = "0";

			m_frameList.Add( add );
		}


		m_infoList = new List<Info>();

		for( int i=0; i<m_kindMax; ++i )
		{
			Info add;
			add.num = 0;

			m_infoList.Add( add );
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
	public void CreateItem()
	{
		//	make an item using decided costs
		Info i;
		i.num = m_infoList[ m_curForcus ].num + 1;
		m_infoList[ m_curForcus ] = i;

		//	add num
		m_frameList[ m_curForcus ].transform.GetChild(0).GetComponent<Text>().text = i.num.ToString();

		//
		m_resourcePoint -= m_resourceInformation.GetChild( m_curForcus ).GetComponent<BaseResource>().m_createCost;
	}
	bool CheckWhetherTheCostIsEnough()
	{
		return m_resourcePoint >= m_resourceInformation.GetChild( m_curForcus ).GetComponent<BaseResource>().m_createCost;
	}
	bool CheckWhetherNumberIsEnough()
	{
		return  m_infoList[ m_curForcus ].num > 0;
	}

	//
	void OnGUI ()
	{
		Transform		g = GameObject.Find("ResourceInformation").transform.GetChild( m_curForcus );
		BaseResource	b = g.GetComponent<BaseResource>();

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
			"\n　作成時間　" + b.m_createTime.ToString() + "秒" +
			"\n　設置時間　" + b.m_puttingTime.ToString() + "秒",
			style );
    }
}
