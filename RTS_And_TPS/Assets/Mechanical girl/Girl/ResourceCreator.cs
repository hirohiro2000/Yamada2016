using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourceCreator : MonoBehaviour
{
    private GameObject		m_staticResources		= null;
    private GameObject		m_fieldResources		= null;

	public  GameObject		m_gridSplitSpacePlane	= null;
	public	float			m_gridSplitSpaceScale	= 10.0f;

	private int				m_guideID				= 1;
    private float			m_rotateAngle			= 0;


    // Use this for initialization
    void Start ()
    {
        m_staticResources		= new GameObject();
		m_staticResources.name	= "StaticResources";

		m_fieldResources		= new GameObject();
		m_fieldResources.name	= "FieldResources";

		m_gridSplitSpacePlane	= Instantiate( m_gridSplitSpacePlane );
		m_gridSplitSpacePlane.transform.localScale = new Vector3( m_gridSplitSpaceScale*0.1f, 1.0f, m_gridSplitSpaceScale*0.1f );

		GameObject obj	= GameObject.Find("ResourceInformation");
		
		for( int i=0; i<obj.transform.childCount; ++i )
		{
			Transform add							= Instantiate( obj.transform.GetChild(i) );
			add.parent								= m_staticResources.transform;
			add.GetComponent<Collider>().enabled	= false;
			add.GetComponent<Pauser>().Enable( false );
		}
    }

    void Update()
    {
        UpdateGuide();
		UpdateGuideAngle();
    }

	void UpdateGuide()
    {
		//	update grid plane
		m_gridSplitSpacePlane.transform.position = ComputeGridPosition( transform.position );


		//	update guide-object
		m_guideID = GetComponent<GirlController>().GetItemFocus();

		for ( int i = 0; i < m_staticResources.transform.childCount; i++ )
        {
            m_staticResources.transform.GetChild(i).gameObject.active = ( i == m_guideID );
        }


		//	update guide's orientation
		Vector3		forward		= transform.forward;
        float		putDist     = 3.0f;
		Transform	guide		= m_staticResources.transform.GetChild( m_guideID );

		guide.position  = transform.position + forward * putDist;
		guide.rotation  = transform.rotation * Quaternion.AngleAxis( m_rotateAngle, Vector3.up );
    }
	void UpdateGuideAngle()
	{
		const float rot = 360.0f / 8.0f;

		if( Input.GetKeyDown( KeyCode.Q ) )
		{
			m_rotateAngle += rot;
		}
		else if( Input.GetKeyDown( KeyCode.E ) )
		{
			m_rotateAngle -= rot;
		}
	}
	Vector3 ComputeGridPosition( Vector3 pos )
	{
		//	四捨五入
		int splitScaleX = (int)(( pos.x / m_gridSplitSpaceScale ) + ( Mathf.Sign( pos.x )>0? 0.5f:-0.5f ));
		int splitScaleZ = (int)(( pos.z / m_gridSplitSpaceScale ) + ( Mathf.Sign( pos.z )>0? 0.5f:-0.5f ));

		return new Vector3( splitScaleX*m_gridSplitSpaceScale, pos.y, splitScaleZ*m_gridSplitSpaceScale );
	}
    public void AddResource()
    {
		GameObject add = Instantiate( m_staticResources.transform.GetChild( m_guideID ).gameObject );

		add.transform.parent	= m_fieldResources.transform;
		//add.transform.position	= m_staticResources.transform.GetChild( m_guideID ).position;
		add.transform.position = ComputeGridPosition( transform.position );
		add.GetComponent<Collider>().enabled = true;
		add.GetComponent<Pauser>().Enable( true );
	}
}
