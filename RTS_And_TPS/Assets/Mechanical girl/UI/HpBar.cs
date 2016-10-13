using UnityEngine;
using System.Collections;

public class HpBar : MonoBehaviour
{
	public	GameObject		m_valueSliderObject	= null;
	public	Color			m_color				= Color.green;

	private	GameObject		m_valueSlider		= null;
	private	ValueSlider		m_valueSliderRef	= null;
	private CollisionParam	m_param				= null;

	// Use this for initialization
	void Start ()
	{
		m_valueSlider		= Instantiate( m_valueSliderObject );
		m_valueSlider.transform.SetParent( GameObject.Find("Canvas").transform );

		m_param				= GetComponent<CollisionParam>();
		m_valueSliderRef	= m_valueSlider.GetComponent<ValueSlider>();
	}

	void OnDestroy()
	{
		Destroy( m_valueSlider );
	}
	
	// Update is called once per frame
	void Update ()
	{
		//m_valueSlider.SetActive( m_valueSliderRef.GetRate() < 1.0f );
		m_valueSliderRef.SetRate( m_param.GetRate() );
		m_valueSliderRef.m_pos = transform.position;
		m_valueSliderRef.SetColor( m_color );
	}
}
