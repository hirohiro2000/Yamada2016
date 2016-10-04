using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourceCreator : MonoBehaviour
{
    private GameObject		m_staticResources		= null;
    private GameObject		m_fieldResources		= null;
    public  GameObject		m_barricade 			= null;

	private int				m_guideID				= 1;
    private float			m_rotateAngle			= 0;

    // Use this for initialization
    void Start ()
    {
        m_staticResources		= new GameObject();
		m_staticResources.name	= "StaticResources";

		m_fieldResources		= new GameObject();
		m_fieldResources.name	= "FieldResources";

		GameObject obj = GameObject.Find("ResourceInformation");
		
		for( int i=0; i<obj.transform.childCount; ++i )
		{
			Transform add = Instantiate( obj.transform.GetChild(i) );
			add.GetComponent<Collider>().enabled = false;
			add.GetComponent<Pauser>().Enable( false );
			add.parent	= m_staticResources.transform;
		}
    }

    void Update()
    {
        UpdateGuide();
		UpdateGuideAngle();
    }

	void UpdateGuide()
    {
		//	update guide-object
		m_guideID = GetComponent<GirlController>().GetItemFocus();

		for ( int i = 0; i < m_staticResources.transform.childCount; i++ )
        {
            m_staticResources.transform.GetChild(i).gameObject.active = (i == m_guideID);
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
    public void AddResource()
    {
		GameObject add = Instantiate( m_staticResources.transform.GetChild( m_guideID ).gameObject );

		add.transform.parent	= m_fieldResources.transform;
		add.transform.position	= m_staticResources.transform.GetChild( m_guideID ).position;
		add.GetComponent<Collider>().enabled = true;
		add.GetComponent<Pauser>().Enable( true );
	}
}
