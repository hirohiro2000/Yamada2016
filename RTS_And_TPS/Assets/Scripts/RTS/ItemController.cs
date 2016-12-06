
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class ItemController : NetworkBehaviour
{
	public	GameObject			m_itemFrame				= null;
	//public  int					m_resourcePoint			= 100;

	private ResourceCreator		m_resourceCreator		= null;
	private	List<GameObject>	m_frameList				= null;
	private	List<GameObject>	m_imageList				= null;
	private List<Text>			m_textList				= null;
	private List<ResourceParameter> m_resourceList = null;

	private int					m_kindMax				= 0;
	private int					m_curForcus				= 0;

    private GameManager         m_rGameManager          = null;

	// Use this for initialization
	void Start ()
	{
        //  自分のキャラクターの場合のみ処理を行う
        if( !isLocalPlayer ) return;

        m_rGameManager          = GameObject.Find("GameManager").GetComponent<GameManager>();
		m_resourceCreator		= GameObject.Find("ResourceCreator").GetComponent<ResourceCreator>();
		m_kindMax				= m_resourceCreator.m_resources.Length;

		m_frameList = new List<GameObject>();
		m_textList = new List<Text>();
		m_resourceList = new List<ResourceParameter>();
		m_imageList = new List<GameObject>();

		for( int i=0; i<m_kindMax; ++i )
		{
			GameObject  add			= Instantiate( m_itemFrame );
            float       screenRatio = Screen.width / 1280.0f;

			add.transform.SetParent( GameObject.Find("Canvas").transform );
			add.transform.position	= new Vector3( ( i*80 + 72 ) * screenRatio, 130 * screenRatio, 0 );
			Text text = add.transform.GetChild(0).GetComponent<Text>();
			ResourceParameter resource = m_resourceCreator.m_resources[i].GetComponent<ResourceParameter>();
            text.text = resource.GetCreateCost().ToString();
			text.transform.SetSiblingIndex(1);

			m_frameList.Add( add );

			m_textList.Add(text);
			m_resourceList.Add(resource);

			GameObject  image		= Instantiate( m_resourceCreator.m_textures[i] );
			image.transform.SetParent( GameObject.Find("Canvas").transform );
			image.transform.position = new Vector3( ( i*80 + 72 ) * screenRatio, 130 * screenRatio, 0 );

			m_imageList.Add( image );
		}
	}
	public  override    void    OnNetworkDestroy()
	{
        base.OnNetworkDestroy();

        if( m_frameList != null )
		{
			for( int i = 0; i < m_frameList.Count; i++ )
				Destroy( m_frameList[ i ].gameObject );
		}

		 if( m_imageList != null )
		{
			for( int i = 0; i < m_imageList.Count; i++ )
				Destroy( m_imageList[ i ].gameObject );
		}
	}

	// Update is called once per frame
	void Update()
	{
		//GameWorldParameterで強制的に書き換える
		{
			for (int i = 0; i < m_textList.Count; i++)
			{
				m_textList[i].text = m_resourceList[i].GetCreateCost().ToString();
			}
		}
        //  自分のキャラクターの場合のみ処理を行う
        if( !isLocalPlayer ) return;

		{
			const float forcus	= 1.1f;
			const float basic	= 0.8f;
		
			for( int i=0; i<m_kindMax; ++i )
			{
				m_frameList[i].transform.localScale = ( m_curForcus==i )? new Vector3( forcus,forcus,forcus ):new Vector3( basic,basic,basic );
				m_imageList[i].transform.localScale = ( m_curForcus==i )? new Vector3( forcus,forcus,forcus ):new Vector3( basic,basic,basic );
			}
		}
		{
			if( Input.GetKeyDown( KeyCode.Space ) )
			{
				m_curForcus++;
				m_curForcus %= m_kindMax;
			}
		}
        //{
        //    var cr = GameObject.Find("Canvas");
        //    var tx = GameObjectExtension.GetComponentInParentAndChildren<Text>( cr );
        //    tx.text = "Resource  " + ( ( int )m_rGameManager.GetResource() ).ToString();
        //}
	}
	

	//------------------------------------------------------------
	//	get
	//------------------------------------------------------------
	public int GetHaveCost()
	{
		return ( int )m_rGameManager.GetResource();
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
		return m_rGameManager.GetResource() >= GetForcusResourceParam().GetCreateCost();
	}


	//------------------------------------------------------------
	//	set
	//------------------------------------------------------------	
	public void AddResourceCost( int cost )
	{
        m_rGameManager.AddResource( cost );
	}
}
