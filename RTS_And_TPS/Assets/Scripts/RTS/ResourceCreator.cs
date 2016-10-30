
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class ResourceCreator : NetworkBehaviour
{
    public  GameObject[]            c_Resource                  =   null;

	private ResourceInformation		m_resourcesInformation		= null;
	private GameObject				m_fieldResources			= null;
    private GameObject				m_staticResources			= null;

	public	GameObject				m_resourceRangeGuide		= null;
	private	GameObject				m_resourceRangeGuideRef		= null;

	private int						m_guideID					= 1;
    private float					m_rotateAngle				= 0;
	public	float					m_guideResourceDistance		= 2.0f;
	public	float					m_guideResourceHeight		= 2.0f;
	
	// Use this for initialization
    void Start ()
    {
        //  アクセスの取得はクローン側でも行う
		m_resourcesInformation	= GameObject.Find("ResourceInformation").GetComponent<ResourceInformation>();
		m_fieldResources		= GameObject.Find("FieldResources");

        //  自分のキャラクターの場合のみ処理を行う
        if( !isLocalPlayer ) return;

        m_staticResources		= new GameObject();
		m_staticResources.name	= "StaticResources";
		m_staticResources.transform.SetParent( transform );


		GameObject obj	= GameObject.Find("ResourceInformation");
		
		for( int i=0; i<obj.transform.childCount; ++i )
		{
			Transform add							= Instantiate( obj.transform.GetChild(i) );
			add.parent								= m_staticResources.transform;
			ChangeGuideState( add.gameObject, false );
		}


		m_resourceRangeGuideRef = Instantiate( m_resourceRangeGuide );
    }
    void Update()
    {
        //  自分のキャラクターの場合のみ処理を行う
        if( !isLocalPlayer ) return;

        UpdateGuide();
		UpdateGuideAngle();
    }
    public  override    void    OnNetworkDestroy()
    {
        base.OnNetworkDestroy();

        if( m_resourceRangeGuideRef ){
            Destroy( m_resourceRangeGuideRef );
        }
    }


	//---------------------------------------------------------------------
	//  
	//---------------------------------------------------------------------
	void UpdateGuide()
    {
		m_guideID		= GetComponent<GirlController>().GetItemFocus();
		bool enable		= m_guideID >= 0;
		
		//
		for ( int i=0; i < m_staticResources.transform.childCount; i++ )
        {
            m_staticResources.transform.GetChild(i).gameObject.SetActive( i == m_guideID );
        }

		m_resourceRangeGuideRef.SetActive( enable );
		

		if ( !enable )
			return;


		//
		Transform	guide	= m_staticResources.transform.GetChild( m_guideID );
		float		range	= guide.GetComponent<ResourceParam>().m_effectiveRange * 0.2f; //t
		Vector3		gridPos = m_resourcesInformation.ComputeGridPosition( transform.position );
		gridPos				+= new Vector3( 0, m_guideResourceHeight, 0 );	
	
		guide.position  = gridPos;
		guide.rotation  = Quaternion.AngleAxis( m_rotateAngle, Vector3.up );		

		m_resourceRangeGuideRef.transform.position = gridPos;
		m_resourceRangeGuideRef.transform.localScale = new Vector3( range, 0, range );
    }
	void UpdateGuideAngle()
	{
		const float rot = 360.0f / 8.0f;

		if( Input.GetKeyDown( KeyCode.U ) )
		{
			m_rotateAngle += rot;
		}
		else if( Input.GetKeyDown( KeyCode.O ) )
		{
			m_rotateAngle -= rot;
		}
	}


	//---------------------------------------------------------------------
	//  
	//---------------------------------------------------------------------
	void ChangeGuideState( GameObject obj, bool ena )
	{
		if( obj.GetComponent<Collider>() )
			obj.GetComponent<Collider>().enabled = ena;

		if( obj.GetComponent<Pauser>() )
			obj.GetComponent<Pauser>().Enable( ena );

		for( int i = 0; i < obj.transform.childCount; ++i )
		{
			obj.transform.GetChild(i).GetComponent<Collider>().enabled = ena;
		}
	}


	//---------------------------------------------------------------------
	//  
	//---------------------------------------------------------------------
	public void AddResource()
    {
        //  サーバーにコマンドを送信
        Transform   rTrans  =   m_staticResources.transform.GetChild( m_guideID );
        CmdAddResource( m_guideID, rTrans.position, rTrans.rotation );
	}

    //---------------------------------------------------------------------
	//      コマンド
	//---------------------------------------------------------------------
    [ Command ]
    void    CmdAddResource( int _GuideID, Vector3 _Position, Quaternion _Rotation )
    {
        GameObject  rObj    =   Instantiate( c_Resource[ _GuideID ] );
        Transform   rTrans  =   rObj.transform;
        
        rTrans.parent   =   m_fieldResources.transform;
        rTrans.position =   _Position;
        rTrans.rotation =   _Rotation;

        //  実体を共有
        NetworkServer.Spawn( rObj );

        //	リソース配置フラグをセット
        m_resourcesInformation.SetGridInformation( rObj, transform.position, true );
    }
    //---------------------------------------------------------------------
	//      リクエスト
	//---------------------------------------------------------------------
}
