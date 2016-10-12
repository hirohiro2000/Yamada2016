using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourceCreator : MonoBehaviour
{
	private ResourceInformation		m_resourcesInformation		= null;
	private GameObject				m_fieldResources			= null;
    private GameObject				m_staticResources			= null;

	private int						m_guideID					= 1;
    private float					m_rotateAngle				= 0;

	
	// Use this for initialization
    void Start ()
    {
		m_resourcesInformation	= GameObject.Find("ResourceInformation").GetComponent<ResourceInformation>();
		m_fieldResources		= GameObject.Find("FieldResources");

        m_staticResources		= new GameObject();
		m_staticResources.name	= "StaticResources";


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
		GameObject add			= Instantiate( m_staticResources.transform.GetChild( m_guideID ).gameObject );

		add.transform.parent	= m_fieldResources.transform;
		add.transform.position	= m_resourcesInformation.ComputeGridPosition( transform.position );
		ChangeGuideState( add, true );


		//	リソース配置フラグをセット
		m_resourcesInformation.SetGridInformation( add, transform.position, true );
	}
}
