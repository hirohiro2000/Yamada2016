
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AcqRecord_Control : MonoBehaviour {

    public  float       c_DisplayTime   =   1.3f;

    private Text        m_rText         =   null;
    private Animator    m_rAnimator     =   null;

    private float       m_DispTimer     =   0.0f;

	// Use this for initialization
	void    Start()
    {
	    m_rText         =   GetComponent< Text >();
        m_rAnimator     =   GetComponent< Animator >();

        m_DispTimer     =   0.0f;
	}
	
	// Update is called once per frame
	void    Update()
    {
        //  タイマー更新
	    m_DispTimer -=  Time.deltaTime;
        m_DispTimer =   Mathf.Max( m_DispTimer, 0.0f );

        //  表示設定
        m_rText.enabled     =   m_DispTimer > 0.0f;
	}

    //  アクセス
    public  void    SetRecord( string _Record )
    {
        //  タイマー設定
        m_DispTimer     =   c_DisplayTime;

        //  テキスト更新
        m_rText.text    =   _Record;

        //  アニメーション
        m_rAnimator.SetTrigger( "Appear" );
    }
}
