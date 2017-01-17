
using   UnityEngine;
using   UnityEngine.UI;
using   System.Collections;

public class PlayerHealthBar : MonoBehaviour {
	[ SerializeField ]  TPSPlayer_HP        m_rHealth       =   null;
    [ SerializeField ]  HealthBar3D         m_rHealthBar    =   null;
    [ SerializeField ]  NetPlayer_Control   m_rNPControl    =   null;
    [ SerializeField ]  Text                m_rText         =   null;
    [ SerializeField ]  Text                m_rTextHelp     =   null;

    //  外部へのアクセス
    private LinkManager m_rLinkManager  =   null;

    void    Start()
    {
        m_rText.text    =   m_rNPControl.c_PlayerName;
        m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );

        if( m_rLinkManager.m_LocalPlayerID == m_rNPControl.c_ClientID ){
            gameObject.SetActive( false );
        }
    }
    void    Update()
    {
        //  ＨＰバー更新
        m_rHealthBar.setValue( m_rHealth.m_CurHP / m_rHealth.m_MaxHP );

        //  救助要請更新
        if( m_rTextHelp ){
            float   deathTimer      =   m_rHealth.GetDeathTimer();
            int     minute          =   ( int )deathTimer / 60;
            int     second          =   ( int )deathTimer % 60;
            m_rTextHelp.enabled     =   m_rHealth.m_IsDying && m_rNPControl.c_ClientID != m_rLinkManager.m_LocalPlayerID;
            m_rTextHelp.text        =   "HELP ME!!\n"
                                    +   minute.ToString( "00" ) + " : " + second.ToString( "00" );
        }
    }
}
