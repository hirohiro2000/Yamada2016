using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourceCreator : MonoBehaviour
{
    private GameObject		m_staticResources			= null;
    private GameObject		m_fieldResources			= null;

	public  GameObject		m_gridSplitSpacePlane		= null;
	public	float			m_gridSplitSpaceScale		= 10.0f;
	private	const int		m_gridSplitNum				= 100;

	private int				m_guideID					= 1;
    private float			m_rotateAngle				= 0;

	struct GridInformation
	{
		public bool			exist;
		public GameObject	resource;
	}
	private GridInformation[,]	m_fieldResourceInformations		= null;


    // Use this for initialization
    void Start ()
    {
        m_staticResources		= new GameObject();
		m_staticResources.name	= "StaticResources";

		m_fieldResources		= new GameObject();
		m_fieldResources.name	= "FieldResources";


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


		GameObject obj	= GameObject.Find("ResourceInformation");
		
		for( int i=0; i<obj.transform.childCount; ++i )
		{
			Transform add							= Instantiate( obj.transform.GetChild(i) );
			add.parent								= m_staticResources.transform;
			ChangeGuideState( add.gameObject, false );
		}
    }
    void Update()
    {
        UpdateGuide();
		UpdateGuideAngle();
    }


	//---------------------------------------------------------------------
	//  
	//---------------------------------------------------------------------
	void UpdateGuide()
    {
		//	update grid plane
		m_gridSplitSpacePlane.transform.position = ComputeGridPosition( transform.position );


		//	update guide-object
		m_guideID = GetComponent<GirlController>().GetItemFocus();

		for ( int i = 0; i < m_staticResources.transform.childCount; i++ )
        {
            m_staticResources.transform.GetChild(i).gameObject.SetActive( i == m_guideID );
        }


		//	update guide's orientation
		Vector3		forward		= transform.forward;
        float		putDist     = 3.0f;
		Transform	guide		= m_staticResources.transform.GetChild( m_guideID );

		guide.position  = transform.position + forward * putDist;
		//guide.rotation  = transform.rotation * Quaternion.AngleAxis( m_rotateAngle, Vector3.up );
		guide.rotation  = Quaternion.AngleAxis( m_rotateAngle, Vector3.up );
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
		GameObject add = Instantiate( m_staticResources.transform.GetChild( m_guideID ).gameObject );

		add.transform.parent	= m_fieldResources.transform;
		//add.transform.position	= m_staticResources.transform.GetChild( m_guideID ).position;
		add.transform.position = ComputeGridPosition( transform.position );
		ChangeGuideState( add, true );

		//	リソース配置フラグをセット
		{
			int i,j;
			ComputeGridResourceExistentIDFromPosition( out i, out j );
			m_fieldResourceInformations[i,j].exist		= true;
			m_fieldResourceInformations[i,j].resource	= add;
		}
	}
	public void LevelUpResource()
    {
		int i,j;
		ComputeGridResourceExistentIDFromPosition( out i, out j );

		if( m_fieldResourceInformations[i,j].resource != null )
			m_fieldResourceInformations[i,j].resource.GetComponent<ResourceParam>().m_level++;
	}


	//---------------------------------------------------------------------
	//  
	//---------------------------------------------------------------------
	Vector3 ComputeGridPosition( Vector3 pos )
	{
		//	四捨五入
		int splitScaleX = (int)(( pos.x / m_gridSplitSpaceScale ) + ( Mathf.Sign( pos.x )>0? 0.5f:-0.5f ));
		int splitScaleZ = (int)(( pos.z / m_gridSplitSpaceScale ) + ( Mathf.Sign( pos.z )>0? 0.5f:-0.5f ));

		return new Vector3( splitScaleX*m_gridSplitSpaceScale, pos.y, splitScaleZ*m_gridSplitSpaceScale );
	}
	void ComputeGridResourceExistentIDFromPosition( out int i, out int j )
	{
		float half = ( m_gridSplitSpaceScale * m_gridSplitNum * 0.5f ) + ( m_gridSplitSpaceScale * 0.5f );
		i = (int)(( transform.position.x + half ) / m_gridSplitSpaceScale );
		j = (int)(( transform.position.z + half ) / m_gridSplitSpaceScale );

		//
		if( i < 0 || j < 0 || i >= m_gridSplitNum || j >= m_gridSplitNum )
		{
			Debug.Log("out of range in ComputeGridResourceExistentIDFromPosition");
		}

		//
		//Debug.Log(i);
		//Debug.Log(j);
	}  
	public bool CheckExistResource()
	{
		int i,j;
		ComputeGridResourceExistentIDFromPosition( out i, out j );

		return m_fieldResourceInformations[i,j].exist;
	}
}
