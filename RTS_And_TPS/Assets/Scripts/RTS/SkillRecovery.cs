using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SkillRecovery : MonoBehaviour
{
    //  外部へのアクセス
    private LinkManager     m_rLinkManager = null;

    private void Awake()
    {
        m_rLinkManager = GameObject.Find("LinkManager").GetComponent<LinkManager>();
    }
    private void Update()
    {
    }
    public  void SkillInvoke()
    {
        if ( CanInvokeSkill() == false )    return;
        SkillInvoker.UsedMP( 1 );

        GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < playerList.Length; i++)
        {
            TPSPlayer_HP hp = playerList[i].GetComponent<TPSPlayer_HP>();

            if (hp.m_IsDying == false) continue;

            // 強制的に蘇らせる
            hp.CmdForciblyRevival( m_rLinkManager.m_rLocalPlayer.GetComponent< NetPlayer_Control >().c_ClientID );

        }
    }
    public  void SkillDeath()
    {
    }

    private bool CanInvokeSkill()
    {
        return ( SkillInvoker.CurrentMagicalPower() > 0 );
    }

}
