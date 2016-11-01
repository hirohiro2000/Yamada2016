﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ValueSlider : MonoBehaviour
{
	private RectTransform		m_rectTransform = null;
	private Slider				m_slider;

	public	float				m_max = 100.0f;
	public	float				m_cur = 100.0f;
	public	Vector3				m_pos;
	public	Vector3				m_offset;

	// Use this for initialization
	void Start ()
	{
		m_rectTransform = GetComponent<RectTransform>();
	    m_slider		= GetComponent<Slider>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		m_rectTransform.position = RectTransformUtility.WorldToScreenPoint( Camera.main, m_pos+m_offset );
		m_slider.value			 = m_cur / m_max;
	}

	public float GetRate()
	{
		return m_cur / m_max;
	}
	public void SetRate( float rate )
	{
		m_cur = m_max * rate;
	}

	public void SetColor( Color color )
	{
		transform.FindChild("Fill Area").FindChild("Fill").GetComponent<Image>().color = color;
	}
}