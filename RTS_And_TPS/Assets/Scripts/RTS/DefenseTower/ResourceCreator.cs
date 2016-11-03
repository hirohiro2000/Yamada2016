
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class ResourceCreator : NetworkBehaviour
{
	public	GameObject[]			m_resources					= null;

	private ResourceInformation		m_resourcesInformation		= null;
	private GameObject				m_fieldResources			= null;
	private GameObject				m_staticResources			= null;

	public	GameObject				m_resourceRangeGuide		= null;
	private	GameObject				m_resourceRangeGuideRef		= null;

	private float					m_rotateAngle				= 0;

	// Use this for initialization
	void Start()
	{
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

		//	ガイドの非表示
		SetGuideVisibleDisable();
	}
	void Update()
	{
		const float rot = 360.0f / 8.0f;

		if (Input.GetKeyDown(KeyCode.U))
		{
			m_rotateAngle += rot;
		}
		else if (Input.GetKeyDown(KeyCode.O))
		{
			m_rotateAngle -= rot;
		}
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
		//	ガイドリソースの範囲の更新
		Transform	guide	= m_staticResources.transform.GetChild( resourceID );
		Vector3		gridPos = m_resourcesInformation.ComputeGridPosition( pos );
		gridPos += new Vector3( 0, 0.1f, 0 );


		float planeAdjust	= 0.2f * 1.3f; //	板＊テクスチャ
		float range			= guide.GetComponent<ResourceParameter>().GetCurLevelParam().range * planeAdjust;

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

		for (int i = 0; i < obj.transform.childCount; ++i)
		{
			if( obj.transform.GetChild(i).GetComponent<Collider>())
				obj.transform.GetChild(i).GetComponent<Collider>().enabled = ena;
		}
	}


	//---------------------------------------------------------------------
	//	アクセサ
	//---------------------------------------------------------------------
	public void AddResource( int resourceID )
	{
		//  サーバーにコマンドを送信
		Transform rTrans = m_staticResources.transform.GetChild( resourceID );
		CmdAddResource( resourceID, rTrans.position, rTrans.rotation );
	}
	public void SetGuideVisibleDisable()
	{
		for( int i=0; i<m_staticResources.transform.childCount; i++ )
		{
			m_staticResources.transform.GetChild(i).gameObject.SetActive( false );
		}

		m_resourceRangeGuideRef.SetActive( false );
	}


	//---------------------------------------------------------------------
	//      コマンド
	//---------------------------------------------------------------------
	[Command]
	void CmdAddResource( int resourceID, Vector3 _Position, Quaternion _Rotation )
	{
		GameObject rObj = Instantiate( m_resources[ resourceID ] );
		Transform rTrans = rObj.transform;

		rTrans.parent = m_fieldResources.transform;
		rTrans.position = _Position;
		rTrans.rotation = _Rotation;

		//  実体を共有
		NetworkServer.Spawn(rObj);

		//	リソース配置フラグをセット
		m_resourcesInformation.SetGridInformation( rObj, _Position, true );
	}
	//---------------------------------------------------------------------
	//      リクエスト
	//---------------------------------------------------------------------
}
