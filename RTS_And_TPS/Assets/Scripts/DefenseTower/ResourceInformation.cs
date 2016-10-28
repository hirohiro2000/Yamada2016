
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class ResourceInformation : NetworkBehaviour
{
	public  GameObject		m_gridSplitSpacePlane					= null;
	public	Transform		m_gridSplitSpacePlaneTargetTransform	= null;

	public	float			m_gridSplitSpaceScale					= 10.0f;
    public  Vector3         m_GridOffset                            = Vector3.zero;

	private	const int		m_gridSplitNum							= 100;

    //  作成済みリソースへのアクセス
    private Transform       m_rFieldResources                       =   null;

	struct GridInformation
	{
		public  bool        exist;
		public  GameObject	resource;
        public  GridInformation( bool _Exsit, GameObject _rObj ){
            exist       =   _Exsit;
            resource    =   _rObj;
        }
	}
	//private GridInformation[,]	m_fieldResourceInformations		= null;

    //  同期用
    [ System.Serializable ]
    public  struct  GridInfo_Net{
        public  int                 x, y;
        public  NetworkInstanceId   netId;
        public  GridInfo_Net( int _X, int _Y, NetworkInstanceId _NetID ){
            x       =   _X;
            y       =   _Y;
            netId   =   _NetID;
        }
    }
    public  class   SyncListGridInfo    :   SyncListStruct< GridInfo_Net >{

    }
    public  SyncListGridInfo    m_rGridInfo     =   new SyncListGridInfo();

	// Use this for initialization
	void Start ()
	{
        m_rFieldResources       =   GameObject.Find( "FieldResources" ).transform;

		m_gridSplitSpacePlane	= Instantiate( m_gridSplitSpacePlane );
		m_gridSplitSpacePlane.transform.localScale = new Vector3( m_gridSplitSpaceScale*0.1f, 1.0f, m_gridSplitSpaceScale*0.1f );//test

        //  非表示
        m_gridSplitSpacePlane.GetComponent< MeshRenderer >().enabled    =   false;
	}

	
	// Update is called once per frame
	void Update ()
	{
        //  
        if( !m_gridSplitSpacePlaneTargetTransform ) return;

		m_gridSplitSpacePlane.transform.position = ComputeGridPosition( m_gridSplitSpacePlaneTargetTransform.position );
		m_gridSplitSpacePlane.transform.position += new Vector3( 0, 0.04f, 0 );
	}

    //---------------------------------------------------------------------
	//      グリッド関係の操作
	//---------------------------------------------------------------------
    GridInfo_Net    GetGridInfo( int _X, int _Y )
    {
        for( int i = 0; i < m_rGridInfo.Count; i++ ){
            if( m_rGridInfo[ i ].x != _X )  continue;
            if( m_rGridInfo[ i ].y != _Y )  continue;

            return  m_rGridInfo[ i ];
        }

        //  項目が見つからない場合は何も置かれていないので、空のデータを返す
        return  new GridInfo_Net( _X, _Y, NetworkInstanceId.Invalid );
    }
    void            AddGridInfo( GridInfo_Net _GridInfo )
    {
        m_rGridInfo.Add( _GridInfo );
    }
    void            RemoveGridInfo( int _X, int _Y )
    {
        for( int i = 0; i < m_rGridInfo.Count; i++ ){
            if( m_rGridInfo[ i ].x != _X )  continue;
            if( m_rGridInfo[ i ].y != _Y )  continue;

            m_rGridInfo.RemoveAt( i );
        }
    }

    GameObject      FindResourceAtNetID( NetworkInstanceId _NetID )
    {
        for( int i = 0; i < m_rFieldResources.childCount; i++ ){
            Transform       rTrans      =   m_rFieldResources.GetChild( i );
            NetworkIdentity rIdentity   =   rTrans.GetComponent< NetworkIdentity >();

            if( rIdentity.netId == _NetID ) return  rIdentity.gameObject;
        }

        return  null;
    }

	//---------------------------------------------------------------------
	//  
	//---------------------------------------------------------------------
	public void LevelUpResource( Vector3 pos )
    {
		var gi = ComputeGridResource( pos );

		if( gi.resource != null )
		{
			var cp = gi.resource.GetComponent<CollisionParam>();
			var rp = gi.resource.GetComponent<ResourceParam>();

			cp.LevelUp();
		}
	}


	//---------------------------------------------------------------------
	//  compute
	//---------------------------------------------------------------------
	private void ComputeGridResourceExistentID( Vector3 pos, out int i, out int j )
	{
		float half = ( m_gridSplitSpaceScale * m_gridSplitNum * 0.5f ) + ( m_gridSplitSpaceScale * 0.5f );
		i = (int)(( pos.x + half ) / m_gridSplitSpaceScale );
		j = (int)(( pos.z + half ) / m_gridSplitSpaceScale );

		if( i < 0 || j < 0 || i >= m_gridSplitNum || j >= m_gridSplitNum )
		{
			Debug.Log("out of range in ComputeGridResourceExistentIDFromPosition");
		}
	}  
	private GridInformation ComputeGridResource( Vector3 pos )
	{
		int x,y;
		ComputeGridResourceExistentID( pos, out x, out y );

        GridInformation gridInfo;
        GridInfo_Net    rGINet      =   GetGridInfo( x, y );

        //  使用されていない場合は検索せずに返す
        if( rGINet.netId == NetworkInstanceId.Invalid ){
            return  new GridInformation( false, null );
        }

        //  ネットワークＩＤからオブジェクトを検索して返す
        gridInfo.exist      =   true;
        gridInfo.resource   =   FindResourceAtNetID( rGINet.netId );
                
        return  gridInfo;
	}
	public Vector3 ComputeGridPosition( Vector3 pos )
	{
		//	四捨五入
		int splitScaleX = (int)(( pos.x / m_gridSplitSpaceScale ) + ( Mathf.Sign( pos.x )>0? 0.5f:-0.5f ));
		int splitScaleZ = (int)(( pos.z / m_gridSplitSpaceScale ) + ( Mathf.Sign( pos.z )>0? 0.5f:-0.5f ));

		return new Vector3( splitScaleX*m_gridSplitSpaceScale, pos.y, splitScaleZ*m_gridSplitSpaceScale ) + m_GridOffset;
	}



	//---------------------------------------------------------------------
	//  get
	//---------------------------------------------------------------------
	public bool CheckExistResourceFromPosition( Vector3 pos )
	{
		return ComputeGridResource( pos ).exist;
	}
	public bool CheckIfCanUpALevel( Vector3 pos, int haveCost )
	{
		var gi = ComputeGridResource( pos );

		if( gi.resource == null )
			return false;

		return haveCost >= gi.resource.GetComponent<ResourceParam>().m_levelUpCost;
	}
	public ResourceParam GetResourceParamFromPosition( Vector3 pos )
	{
        GridInformation rGridInfo   =   ComputeGridResource( pos );
        GameObject      rObj        =   rGridInfo.resource;

        if( !rObj )     return  null;
        else		    return  rObj.GetComponent< ResourceParam >();
	}

	
	//---------------------------------------------------------------------
	//  set
	//---------------------------------------------------------------------
	public void SetGridInformation( GameObject game, Vector3 pos, bool enable )
	{
		//i don't know how to get reference
		int x,y;
		ComputeGridResourceExistentID( pos, out x, out y );
		
		if( enable )
		{
            AddGridInfo( new GridInfo_Net( x, y, game.GetComponent< NetworkIdentity >().netId ) );
		}
		else
		{
            GridInfo_Net    rGINet      =   GetGridInfo( x, y );
            GameObject      rObj        =   FindResourceAtNetID( rGINet.netId );

            Destroy( rObj );
            RemoveGridInfo( x, y );
		}
	}
}
