using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
	public	GameObject		m_valueSlider		= null;
	public	Color			m_sliderColor		= Color.green;
	public	bool			m_levelTextEnable	= false;
	public	GameObject		m_levelText			= null;

	private	GameObject		m_valueSliderRef	= null;
	private GameObject		m_textRef			= null;

	private	ValueSlider		m_valueSliderCom	= null;
	private ResourceParameter	m_param				= null;

	// Use this for initialization
	void Start ()
	{
		m_valueSliderRef	= Instantiate( m_valueSlider );
		m_valueSliderRef.transform.SetParent( GameObject.Find("Canvas").transform );

		m_textRef = Instantiate( m_levelText );
		m_textRef.transform.SetParent( GameObject.Find("Canvas").transform );
		m_textRef.GetComponent<Text>().color = Color.white;

		m_param				= GetComponent<ResourceParameter>();
		m_valueSliderCom	= m_valueSliderRef.GetComponent<ValueSlider>();
	}

	//
	void OnDestroy()
	{
		Destroy( m_valueSliderRef );
		Destroy( m_textRef );
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 offsetV = new Vector3( 0, -1.5f, 0 );
		Vector3 offsetT = new Vector3( -35.0f, -18.0f, 0 );

		//m_valueSliderRef.SetActive( m_param.GetRate() < 1.0f );
		m_valueSliderCom.SetRate( m_param.GetRate() );
		m_valueSliderCom.m_pos = transform.position;
		m_valueSliderCom.m_pos	+= offsetV;
		m_valueSliderCom.SetColor( m_sliderColor );

		m_textRef.SetActive( m_levelTextEnable );
		m_textRef.GetComponent<Text>().text = m_param.m_level.ToString();
		m_textRef.transform.position		= RectTransformUtility.WorldToScreenPoint ( Camera.main, transform.position );
		m_textRef.transform.position		+= offsetT;
	}
}
