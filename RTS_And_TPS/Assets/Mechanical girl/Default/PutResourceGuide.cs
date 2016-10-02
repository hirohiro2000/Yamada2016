using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PutResourceGuide : MonoBehaviour
{
    private GameObject		m_staticResources		= null;
    private GameObject		m_fieldResources		= null;
    public  GameObject		m_barricade 			= null;

	private int				m_guideID				= 0;
    private float			m_rotateAngle			= 0;

    private GameObject		m_mechanicalGirl		= null;


    // Use this for initialization
    void Start ()
    {
        m_staticResources		= new GameObject();
		m_staticResources.name	= "m_staticResources";

		m_fieldResources		= new GameObject();
		m_fieldResources.name	= "m_fieldResources";


		Instantiate( m_barricade ).transform.parent = m_staticResources.transform;


		m_mechanicalGirl   = GameObject.Find("MechanicalGirl");
    }

    void Update()
    {
        UpdateGuide();
		UpdateGuideAngle();
    }

    void UpdateGuide()
    {
        for ( int i = 0; i < m_staticResources.transform.childCount; i++ )
        {
            m_staticResources.transform.GetChild(i).gameObject.active = (i == m_guideID);
        }


		Vector3		forward		= m_mechanicalGirl.transform.forward;
        float		putDist     = 3.0f;
		Transform	guide		= m_staticResources.transform.GetChild( m_guideID );

		guide.position  = m_mechanicalGirl.transform.position + forward * putDist;
		guide.rotation  = m_mechanicalGirl.transform.rotation * Quaternion.AngleAxis( m_rotateAngle, Vector3.up );
    }
	void UpdateGuideAngle()
	{
		const float rot = 360.0f / 8.0f;

		if( Input.GetKeyDown( KeyCode.Q ) )
		{
			m_rotateAngle += rot;
		}
		else if( Input.GetKeyDown( KeyCode.Q ) )
		{
			m_rotateAngle -= rot;
		}
	}

    public void AddResource()
    {
		GameObject add = Instantiate( m_staticResources.transform.GetChild( m_guideID ).gameObject );

		add.transform.parent	= m_fieldResources.transform;
		add.transform.position	= m_staticResources.transform.GetChild( m_guideID ).position;
	}
}
