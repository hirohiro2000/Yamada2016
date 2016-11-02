
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class ItemController : NetworkBehaviour
{
	public	GameObject			m_itemFrame				= null;
	public  int					m_resourcePoint			= 100;

	private ResourceCreator		m_resourceCreator		= null;
	private	List<GameObject>	m_frameList				= null;
	private int					m_kindMax				= 0;
	private int					m_curForcus				= 0;

	// Use this for initialization
	void Start ()
	{
        //  自分のキャラクターの場合のみ処理を行う
        if( !isLocalPlayer ) return;

		m_resourceCreator		= GameObject.Find("ResourceCreator").GetComponent<ResourceCreator>();
		m_kindMax				= m_resourceCreator.m_resources.Length;

		m_frameList = new List<GameObject>();

		for( int i=0; i<m_kindMax; ++i )
		{
			GameObject  add			= Instantiate( m_itemFrame );
            float       screenRatio = Screen.width / 1280.0f;

			add.transform.SetParent( GameObject.Find("Canvas").transform );
			add.transform.position	= new Vector3( ( i*80 + 72 ) * screenRatio, 130 * screenRatio, 0 );
			add.transform.GetChild(0).GetComponent<Text>().text = "";//"0";

			m_frameList.Add( add );
		}
	}
	public  override    void    OnNetworkDestroy()
	{
        base.OnNetworkDestroy();

        if( m_frameList == null )   return;
        for( int i = 0; i < m_frameList.Count; i++ ){
            Destroy( m_frameList[ i ].gameObject );
        }
	}

	// Update is called once per frame
	void Update()
	{
        //  自分のキャラクターの場合のみ処理を行う
        if( !isLocalPlayer ) return;

		{
			const float forcus	= 1.1f;
			const float basic	= 0.8f;
		
			for( int i=0; i<m_kindMax; ++i )
			{
				m_frameList[i].transform.localScale = ( m_curForcus==i )? new Vector3( forcus,forcus,forcus ):new Vector3( basic,basic,basic );
			}
		}
		{
			if( Input.GetKeyDown( KeyCode.Space ) )
			{
				m_curForcus++;
				m_curForcus %= m_kindMax;
			}
		}
		{
			var cr = GameObject.Find("Canvas");
			var tx = GameObjectExtension.GetComponentInParentAndChildren<Text>( cr );
			tx.text = "Resource  " + m_resourcePoint.ToString();
		}
	}
	

	//------------------------------------------------------------
	//	get
	//------------------------------------------------------------
	public int GetHaveCost()
	{
		return m_resourcePoint;
	}
	public int GetForcus()
	{
		return m_curForcus;
	}
	public ResourceParameter GetForcusResourceParam()
	{
		return m_resourceCreator.m_resources[ m_curForcus ].GetComponent<ResourceParameter>();
	}
	public bool CheckWhetherTheCostIsEnough()
	{
		return m_resourcePoint >= GetForcusResourceParam().m_createCost;
	}


	//------------------------------------------------------------
	//	set
	//------------------------------------------------------------	
	public void AddResourceCost( int cost )
	{
		m_resourcePoint += cost;
	}
}
