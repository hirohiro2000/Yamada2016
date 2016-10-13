using UnityEngine;
using System.Collections;

public class ResourceInformation : MonoBehaviour
{
	public  GameObject		m_gridSplitSpacePlane					= null;
	public	Transform		m_gridSplitSpacePlaneTargetTransform	= null;

	public	float			m_gridSplitSpaceScale					= 10.0f;
	private	const int		m_gridSplitNum							= 100;

	struct GridInformation
	{
		public bool			exist;
		public GameObject	resource;
	}
	private GridInformation[,]	m_fieldResourceInformations		= null;


	// Use this for initialization
	void Start ()
	{
		m_gridSplitSpacePlane	= Instantiate( m_gridSplitSpacePlane );
		m_gridSplitSpacePlane.transform.localScale = new Vector3( m_gridSplitSpaceScale*0.1f, 1.0f, m_gridSplitSpaceScale*0.1f );

		m_fieldResourceInformations = new GridInformation[ m_gridSplitNum, m_gridSplitNum ];
		for( int i=0; i<m_gridSplitNum; ++i )
		{
			for( int j=0; j<m_gridSplitNum; ++j )
			{
				m_fieldResourceInformations[i,j].exist		= false;
				m_fieldResourceInformations[i,j].resource	= null;
			}
		}
	}

	
	// Update is called once per frame
	void Update ()
	{
		m_gridSplitSpacePlane.transform.position = ComputeGridPosition( m_gridSplitSpacePlaneTargetTransform.position );
		m_gridSplitSpacePlane.transform.position += new Vector3( 0, -m_gridSplitSpacePlaneTargetTransform.transform.localScale.y*0.5f+0.1f, 0 );
	}


	//---------------------------------------------------------------------
	//  
	//---------------------------------------------------------------------
	public int LevelUpResource( Vector3 pos )
    {
		int x,y;
		ComputeGridResourceExistentID( pos, out x, out y );

		if( m_fieldResourceInformations[x,y].resource != null )
		{
			var rp = m_fieldResourceInformations[x,y].resource.GetComponent<ResourceParam>();
			rp.m_level++;
			return rp.m_createCost;
		}

		return 0;
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
		return m_fieldResourceInformations[x,y];
	}
	public Vector3 ComputeGridPosition( Vector3 pos )
	{
		//	四捨五入
		int splitScaleX = (int)(( pos.x / m_gridSplitSpaceScale ) + ( Mathf.Sign( pos.x )>0? 0.5f:-0.5f ));
		int splitScaleZ = (int)(( pos.z / m_gridSplitSpaceScale ) + ( Mathf.Sign( pos.z )>0? 0.5f:-0.5f ));

		return new Vector3( splitScaleX*m_gridSplitSpaceScale, pos.y, splitScaleZ*m_gridSplitSpaceScale );
	}



	//---------------------------------------------------------------------
	//  get
	//---------------------------------------------------------------------
	public bool CheckExistResourceFromPosition( Vector3 pos )
	{
		return ComputeGridResource( pos ).exist;
	}
	public ResourceParam GetResourceParamFromPosition( Vector3 pos )
	{
		return ComputeGridResource( pos ).resource.GetComponent<ResourceParam>();
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
			m_fieldResourceInformations[x,y].resource	= game;
			m_fieldResourceInformations[x,y].exist		= true;
		}
		else
		{
			Destroy( m_fieldResourceInformations[x,y].resource );
			m_fieldResourceInformations[x,y].exist = false;
		}
	}
}
