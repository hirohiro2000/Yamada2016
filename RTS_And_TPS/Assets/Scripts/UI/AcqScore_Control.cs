
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AcqScore_Control : MonoBehaviour {

    public  string      c_Label         =   "+ ";
    public  bool        c_UseAnime      =   true;

	public  float       c_CountUpTime   =   0.3f;
    public  float       c_DisplayTime   =   1.0f;
    public  int         c_Decimal       =   0;

    private Text        m_rText         =   null;
    private Animator    m_rAnimator     =   null;

    private float       m_DispScore     =   0;
    private float       m_UpScore       =   0;
    private float       m_CUTimer       =   0.0f;
    private float       m_DispTimer     =   0.0f;

    private float       m_PrevScore     =   0;
    private float       m_StartScore    =   0;

	// Use this for initialization
	void    Awake()
    {
	    m_rText         =   GetComponent< Text >();
        m_rAnimator     =   GetComponent< Animator >();

        m_DispScore     =   0;
        m_UpScore       =   0;
        m_CUTimer       =   0.0f;
        m_DispTimer     =   0.0f;

        m_PrevScore     =   0;
        m_StartScore    =   0;
	}
	
	// Update is called once per frame
	void    Update()
    {
        //if( Input.GetKeyDown( KeyCode.K ) ){
        //    m_rAnimator.SetTrigger( "Appear" );
        //    SetAddScore( 150 );
        //}

	    UpdateDispScore();
	}
    void    UpdateDispScore()
    {
        m_CUTimer   -=  Time.deltaTime;
        if( m_CUTimer <= 0.0f ){
            m_CUTimer   =   0.0f;

            m_DispTimer -=  Time.deltaTime;
            if( m_DispTimer < 0.0f ){
                m_DispTimer =   0.0f;
            }
        }

        float   timeRate    =  1.0f - m_CUTimer / c_CountUpTime;

        m_DispScore         =   m_StartScore + m_UpScore * timeRate;
        m_PrevScore         =   m_DispScore;

        if( timeRate >= 1.0f ){
            m_DispScore     =   ( int )m_DispScore;
        }

        SetDispScore( m_DispScore );

        m_rText.enabled     =   m_DispTimer > 0.0f;
    }

    //  アクセス
    public  void    SetAddScore( float _AddScore ){
        if( _AddScore < 0.0f )      return;

        if( m_DispTimer > 0.0f ){
            float lastScore =   m_StartScore + m_UpScore;
            float curScore  =   m_PrevScore;
            m_UpScore       =   ( lastScore - curScore ) + _AddScore;
            m_StartScore    =   m_PrevScore;
        }
        else{
            m_UpScore       =   _AddScore;
            m_StartScore    =   0;
        }

        m_CUTimer       =   c_CountUpTime;
        m_DispTimer     =   c_DisplayTime;
        m_PrevScore     =   m_StartScore;

        //  アニメーション
        if( c_UseAnime ){
            m_rAnimator.SetTrigger( "Appear" );
        }
    }
    void    SetDispScore( float _Score ){
        float   dispScore   =   _Score;
        //for( int i = 0; i < c_Decimal; i++ )    dispScore   *=  10;

        //dispScore   =   ( int )dispScore;

        //for( int i = 0; i < c_Decimal; i++ )    dispScore   /=  10;

        string  format  =   "0";
        if( c_Decimal > 0 ){
            format  +=  ".";
            for( int i = 0; i < c_Decimal; i++ ){
                format  +=  "0";
            }
        }

        m_rText.text    =   c_Label +   dispScore.ToString( format );
    }
}
