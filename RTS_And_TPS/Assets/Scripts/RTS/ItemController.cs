
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
	private List<Text>			m_textList				= null;
	private List<ResourceParameter> m_resourceList = null;

	private int					m_kindMax				= 0;
	private int					m_curForcus				= -1;

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

        Transform   rHUD        = GameObject.Find( "Canvas" ).transform.FindChild("RTS_HUD");
        Transform   rFrameShell = rHUD.FindChild("FrameShell");
		for( int i=0; i<m_kindMax; ++i )
		{
			GameObject  add			= rFrameShell.GetChild(i).gameObject;
            float       screenRatio = Screen.width / 1280.0f;

			add.transform.SetParent( rFrameShell );
                
            RTSOnItemFrame onFrame = add.GetComponent<RTSOnItemFrame>();
            onFrame.id = i;

			Text text = add.transform.GetChild(2).GetComponent<Text>();
			ResourceParameter resource = m_resourceCreator.m_resources[i].GetComponent<ResourceParameter>();
            text.text = resource.GetCreateCost().ToString();

            add.transform.GetChild(0).GetComponent<RawImage>().texture = m_resourceCreator.m_textures[i].GetComponent<Image>().mainTexture;

			m_frameList.Add( add );

			m_textList.Add(text);
			m_resourceList.Add(resource);
		}
        rFrameShell.gameObject.SetActive(false);

    }
	public  override    void    OnNetworkDestroy()
	{
        base.OnNetworkDestroy();

        if( m_frameList != null )
		{
			for( int i = 0; i < m_frameList.Count; i++ )
				Destroy( m_frameList[ i ].gameObject );
		}

	}

	// Update is called once per frame
	void Update()
	{	
        //  自分のキャラクターの場合のみ処理を行う
        if( !isLocalPlayer ) return;

        //GameWorldParameterで強制的に書き換える
		{
			for (int i = 0; i < m_textList.Count; i++)
			{
				m_textList[i].text = m_resourceList[i].GetCreateCost().ToString();
			}
		}

		{
			const float forcus	= 1.2f;
			const float basic	= 0.8f;
		
			for( int i=0; i<m_kindMax; ++i )
			{
                Image image = m_frameList[i].GetComponent<Image>();

                bool isEnoughCost = CheckWhetherTheCostIsEnough( i );

                // 色更新
                m_frameList[i].transform.GetChild(1).gameObject.SetActive(!isEnoughCost);
                
                // 大きさ更新
                if ( m_curForcus == i )
                {
                    m_frameList[i].transform.localScale = new Vector3( forcus,forcus,forcus );
                }
                else
                {
                    m_frameList[i].transform.localScale = new Vector3( basic,basic,basic );
                }

			}
		}
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
    public int GetNumKind()
    {
        return m_kindMax;
    }
	public ResourceParameter GetForcusResourceParam()
	{
        if ( m_curForcus == -1 )            return null;
		return m_resourceCreator.m_resources[ m_curForcus ].GetComponent<ResourceParameter>();
	}
	public ResourceParameter GetResourceParam( int resourceID )
	{
		if ( resourceID < 0 || m_kindMax <= resourceID ) return null;
		return m_resourceCreator.m_resources[ resourceID ].GetComponent<ResourceParameter>();
	}
	public bool CheckWhetherTheCostIsEnough()
	{
		return ( m_curForcus != -1 ) && ( m_rGameManager.GetResource() >= GetForcusResourceParam().GetCreateCost() );
	}
	public bool CheckWhetherTheCostIsEnough( int resourceID )
	{
		return ( 0 <= resourceID && resourceID < m_kindMax ) 
            && ( m_rGameManager.GetResource() >= m_resourceCreator.m_resources[ resourceID ].GetComponent<ResourceParameter>().GetCreateCost() );
	}


	//------------------------------------------------------------
	//	set
	//------------------------------------------------------------	
	public void AddResourceCost( int cost )
	{
        m_rGameManager.AddResource( cost );
	}
    public void SetActive( bool isActive )
    {
		for( int i=0; i<m_kindMax; ++i )
		{
            m_frameList[i].SetActive( isActive );
        }
    }
    public void SetActive( bool isActive, int index )
    {
        m_frameList[index].SetActive( isActive );
    }
    public void SetForcus( int forcusID )
    {
        m_curForcus = forcusID;
    }

}
