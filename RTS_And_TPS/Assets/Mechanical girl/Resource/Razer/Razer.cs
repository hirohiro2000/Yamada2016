using UnityEngine;
using System.Collections;

public class Razer : MonoBehaviour
{
	public  float	m_interval	= 1.0f;
	private int		m_lazerID	= 1;

	// Use this for initialization
	void Start ()
	{	
		StartCoroutine( ChangeOnOff() );
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

	IEnumerator ChangeOnOff()
    {
        while( true )
        {
			var g = transform.GetChild( m_lazerID ).gameObject;
           	g.SetActive( !g.activeInHierarchy );
            yield return new WaitForSeconds( m_interval );
        }     
    }
}
