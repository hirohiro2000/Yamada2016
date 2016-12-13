using UnityEngine;
using System.Collections;

public class Cloud_Control : MonoBehaviour {

    private float       m_CloudSpeed    =   0.0f;
    private Renderer    m_rRenderer     =   null;

	// Use this for initialization
	void    Start()
    {
	    m_rRenderer =   GetComponent< Renderer >();
	}
	
	// Update is called once per frame 
	void    Update()
    {
	    transform.Rotate( 0.0f, Time.deltaTime * m_CloudSpeed, 0.0f );
	}
    void    LateUpdate()
    {

    }

    public  void    SetMaterial( Material _rMaterial )
    {
        m_rRenderer.material    =   _rMaterial;
    }
    public  void    SetSpeed( float _CloudSpeed )
    {
        m_CloudSpeed            =   _CloudSpeed;
    }
}
