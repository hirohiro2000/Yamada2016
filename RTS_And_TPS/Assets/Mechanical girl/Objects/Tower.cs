using UnityEngine;
using System.Collections;

public class Tower : MonoBehaviour
{
	public	GameObject m_actionBar	= null;

	// Use this for initialization
	void Start ()
	{
		m_actionBar						= Instantiate( m_actionBar );
		m_actionBar.transform.SetParent( GameObject.Find("Canvas").transform );
	}
	
	// Update is called once per frame
	void Update ()
	{
		ValueSlider slider = m_actionBar.GetComponent<ValueSlider>();
		slider.m_pos = transform.position;
	}

	void OnCollisionEnter( Collision collision )
	{
		if( collision.gameObject.tag == "Enemy")
		{
			m_actionBar.GetComponent<ValueSlider>().m_cur -= 3.0f;
		}
	}
}
