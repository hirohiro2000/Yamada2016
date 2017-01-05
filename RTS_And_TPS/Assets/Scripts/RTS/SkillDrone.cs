using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

public class SkillDrone : MonoBehaviour 
{
    private List<GameObject>    m_doroneList    = null;

    //  外部へのアクセス
    private LinkManager         m_rLinkManager  = null;

    private void Awake()
    {
        m_doroneList    = new List<GameObject>();
        m_rLinkManager = GameObject.Find("LinkManager").GetComponent<LinkManager>();
    }
    private void Update()
    {
    }
    public  void SkillInvoke()
    {
        if ( CanInvokeSkill() == false )    return;
        SkillInvoker.UsedMP( 1 );

        GameObject girl = m_rLinkManager.m_rLocalPlayer;
        girl.GetComponent<GirlController>().CmdPlaceDrone( girl.GetComponent<NetworkIdentity>().netId );
    }
    public  void SkillDeath()
    {
        for (int i = 0; i < m_doroneList.Count; i++)
        {
            GameObject.Destroy( m_doroneList[i] );
        }
        m_doroneList.Clear();
    }

    private bool CanInvokeSkill()
    {
        return ( SkillInvoker.CurrentMagicalPower() > 0 );
    }

}

