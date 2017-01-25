
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class ResourceCreator : NetworkBehaviour
{
	public	GameObject[]			m_resources					= null;
	public	GameObject[]			m_textures					= null;

	private ResourceInformation		m_resourcesInformation		= null;
	private GameObject				m_fieldResources			= null;
	private GameObject				m_staticResources			= null;

	public	GameObject				m_resourceRangeGuide		= null;
	private	GameObject				m_resourceRangeGuideRef		= null;
	private	GameObject				m_prevAddResource			= null;

	private float					m_rotateAngle				= 0;


    private LinkManager             m_rLinkManager              = null;

	// Use this for initialization
	void Start()
	{
        m_rLinkManager          = FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );

		m_resourcesInformation	= GameObject.Find("ResourceInformation").GetComponent<ResourceInformation>();
		m_fieldResources		= GameObject.Find("FieldResources");

		//	ガイドリソースの初期化
		m_staticResources = new GameObject();
		m_staticResources.name = "StaticResources";
		m_staticResources.transform.SetParent( transform );

		for( int i=0; i<m_resources.Length; ++i )
		{
			Transform add = Instantiate( m_resources[i] ).transform;
			add.parent = m_staticResources.transform;
			ChangeGuideState( add.gameObject, false );
		}

		//	ガイドレンジの初期化
		m_resourceRangeGuideRef = Instantiate( m_resourceRangeGuide );

        //  エディット用
        {
            GameObject  rPTShell    =   GameObject.Find( "PlayTest_Shell" );
            if( rPTShell ){
                m_resourceRangeGuideRef.transform.parent
                    =   rPTShell.transform;
            }
        }

		//	ガイドの非表示
		SetGuideVisibleDisable();
	}
	void Update()
	{
		const float rot = 360.0f / 8.0f;

		if (Input.GetKeyDown(KeyCode.R))
		{
			m_rotateAngle += rot;
		}
//		else if (Input.GetKeyDown(KeyCode.O))
//		{
//			m_rotateAngle -= rot;
//		}
	}
	public override void OnNetworkDestroy()
	{
		base.OnNetworkDestroy();

		if (m_resourceRangeGuideRef)
		{
			Destroy(m_resourceRangeGuideRef);
		}
	}


	//---------------------------------------------------------------------
	//  
	//---------------------------------------------------------------------
	public void UpdateGuideResource( int resourceID, Vector3 pos )
	{
		//	ガイドリソースの表示切替と姿勢
		for ( int i = 0; i < m_staticResources.transform.childCount; i++ )
		{
			m_staticResources.transform.GetChild(i).gameObject.SetActive( i == resourceID );
		}

		Transform guide = m_staticResources.transform.GetChild( resourceID );
		Vector3 gridPos = m_resourcesInformation.ComputeGridPosition( pos );

		guide.position = gridPos;
		guide.rotation = Quaternion.AngleAxis( m_rotateAngle, Vector3.up );
	}
	public void UpdateGuideRange( int resourceID, Vector3 pos )
	{
		var guideParam = m_staticResources.transform.GetChild( resourceID ).GetComponent<ResourceParameter>();
		UpdateGuideRange( pos, guideParam.GetCurLevelParam().range );
	}
	public void UpdateGuideRange( Vector3 pos )
	{
		var gridParam = m_resourcesInformation.GetResourceParamFromPosition( pos );
		UpdateGuideRange( pos, gridParam.GetCurLevelParam().range );
	}
	public void UpdateGuideRange( Vector3 pos, float range )
	{
		//	ガイドリソースの範囲の更新
		Vector3		gridPos		= m_resourcesInformation.ComputeGridPosition( pos );
		gridPos					+= new Vector3( 0, 0.1f, 0 );

		//	板＊テクスチャ
		float planeAdjust	= 0.2f * 1.3f; 
		range				*= planeAdjust;

		//
		m_resourceRangeGuideRef.SetActive( true );
		m_resourceRangeGuideRef.transform.position		= gridPos;
		m_resourceRangeGuideRef.transform.localScale	= new Vector3( range, 0, range );
	}


	//---------------------------------------------------------------------
	//  
	//---------------------------------------------------------------------
	void ChangeGuideState( GameObject obj, bool ena )
	{               
		if (obj.GetComponent<Collider>())
			obj.GetComponent<Collider>().enabled = ena;

		if (obj.GetComponent<Pauser>())
			obj.GetComponent<Pauser>().Enable(ena);

		if (obj.GetComponent<NavMeshObstacle>())
			obj.GetComponent<NavMeshObstacle>().enabled = ena;

		if (obj.GetComponent<ResourceAppear>())
			obj.GetComponent<ResourceAppear>().enabled = ena;

        for (int i = 0; i < obj.transform.childCount; ++i)
		{
			if( obj.transform.GetChild(i).GetComponent<Collider>())
				obj.transform.GetChild(i).GetComponent<Collider>().enabled = ena;
		}

        if (obj.transform.FindChild("_Health"))
        {
            obj.transform.FindChild("_Health").gameObject.SetActive(ena);
        }
        if ( obj.transform.FindChild("VisibilityChecker") )
        {
            obj.transform.FindChild("VisibilityChecker").gameObject.SetActive(ena);
        }

	}


	//---------------------------------------------------------------------
	//	アクセサ
	//---------------------------------------------------------------------
	public  GameObject    AddResource( int resourceID )
	{
		//  サーバーにコマンドを送信
		Transform rTrans = m_staticResources.transform.GetChild( resourceID );

        //  プレイヤーオブジェクトを経由してコマンドを送信
        m_rLinkManager.m_rLocalPlayer.GetComponent< RTSPlayer_Control >()
            .CmdAddResource( resourceID, rTrans.position, rTrans.rotation );

		return m_prevAddResource;
	}
    public  void    AddResource_CallByCommand( int resourceID, Vector3 _Position, Quaternion _Rotation, int _OwnerID )
	{
		GameObject rObj = Instantiate( m_resources[ resourceID ] );
		Transform rTrans = rObj.transform;

		rTrans.parent = m_fieldResources.transform;
		rTrans.position = _Position;
		rTrans.rotation = _Rotation;

		//	保存
		m_prevAddResource = rObj;

        //  所有者を登録
        RTSResourece_Control    rControl    =   rObj.GetComponent< RTSResourece_Control >();
        rControl.c_OwnerID  =   _OwnerID;

		//  実体を共有
		NetworkServer.Spawn(rObj);

		//	リソース配置フラグをセット
		m_resourcesInformation.SetGridInformation( rObj, _Position, true );
    }

	public void SetGuideVisibleDisable()
	{
		for( int i=0; i<m_staticResources.transform.childCount; i++ )
		{
			m_staticResources.transform.GetChild(i).gameObject.SetActive( false );
		}

		m_resourceRangeGuideRef.SetActive( false );
	}
	public void SetGuideResourcePosition( int resourceID, Vector3 pos )
	{
		Transform guide = m_staticResources.transform.GetChild( resourceID );
		guide.position = m_resourcesInformation.ComputeGridPosition( pos );
	}


	//---------------------------------------------------------------------
	//      コマンド
	//---------------------------------------------------------------------
	//---------------------------------------------------------------------
	//      リクエスト
	//---------------------------------------------------------------------
}
