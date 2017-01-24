using UnityEngine;
using System.Collections;

public class UIOwnHealth : MonoBehaviour
{
    public enum TYPE
    {
        eRobot,
        eGirl
    }
    [SerializeField]
    private TYPE m_ownType = TYPE.eRobot;
    
    private LinkManager   m_rLinkManager  { get; set; }
    private PlayerHealth  m_ownFighter    { get; set; }

    void Awake()
    {
        m_ownFighter    = null;
        m_rLinkManager  = FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );
    }

    void Update()
    {                  
        if ( m_ownFighter == null )
        {
            GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < playerList.Length; i++)
            {
                if( playerList[i] == m_rLinkManager.m_rLocalPlayer ) continue;

                if( m_ownType == TYPE.eGirl  && playerList[i].GetComponent<TPSPlayer_Control>() != null ) continue;
                if( m_ownType == TYPE.eRobot && playerList[i].GetComponent<RTSPlayer_Control>() != null ) continue;

                m_ownFighter = playerList[i].GetComponent<PlayerHealth>();

            }
        }   

        if ( m_ownFighter == null ) return;
        




         
    }
}
