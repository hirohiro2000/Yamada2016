
using   UnityEngine;
using   UnityEngine.UI;
using   System.Collections;

public class DyingMessage_Control : MonoBehaviour {

    private bool        m_IsActive          =   false;

    private Text        m_rMessage          =   null;
    private Text        m_rTimeLimit        =   null;
    private Animator    m_rAnimeMessage     =   null;
    private Animator    m_rAnimeTimeLimit   =   null;

	// Use this for initialization
	void    Start()
    {
        Transform   rMessage    =   transform.FindChild( "_Message" );
        Transform   rTimeLimit  =   transform.FindChild( "_TimeLimit" );

        m_rMessage          =   rMessage.GetComponent< Text >();
        m_rTimeLimit        =   rTimeLimit.GetComponent< Text >();
        m_rAnimeMessage     =   rMessage.GetComponent< Animator >();
        m_rAnimeTimeLimit   =   rTimeLimit.GetComponent< Animator >();
	}
	
	// Update is called once per frame
	void    Update()
    {

	}

    public  void    SetActive( bool _IsActive )
    {
        //  変更がなければ処理を行わない
        if( m_IsActive == _IsActive )   return;

        //  フラグ更新
        m_IsActive                  =   _IsActive;

        //  動作フラグ更新
        m_rMessage.enabled          =   _IsActive;
        m_rTimeLimit.enabled        =   _IsActive;
        m_rAnimeMessage.enabled     =   _IsActive;
        m_rAnimeTimeLimit.enabled   =   _IsActive;

        //  アクティベートされた場合はアニメーションを再生
        if( _IsActive ){
            m_rAnimeMessage.SetTrigger( "Appear" );
            m_rAnimeTimeLimit.SetTrigger( "Appear" );
        }
    }
    public  void    SetTimeLimit( float _TimeLimit )
    {
        int minute  =   ( int )_TimeLimit / 60;
        int second  =   ( int )_TimeLimit % 60;

        m_rTimeLimit.text   =   minute.ToString( "00" ) + " : " + second.ToString( "00" );
    }
}
