using UnityEngine;
using System.Collections;

public class Razer : MonoBehaviour
{
	public  float	m_interval	= 1.0f;

	// Use this for initialization
	void Start ()
	{	
		StartCoroutine( ChangeOnOff() );
	}
	
	// Update is called once per frame
	void Update ()
	{
		UpdateLevel();
	}

	void UpdateLevel()
	{	
		int			level		= GetComponent<ResourceParam>().m_level;
		float		addScale	= 0.5f * ( level-1 );
		Transform	beam		= transform.GetChild(1);

		beam.localScale = new Vector3( 0.1f+addScale, beam.localScale.y, 0.1f+addScale );
	}

	IEnumerator ChangeOnOff()
    {
        while( true )
        {
           	transform.GetChild(1).gameObject.SetActive( !transform.GetChild(1).gameObject.activeInHierarchy );
            yield return new WaitForSeconds( m_interval );
        }     
    }
}
