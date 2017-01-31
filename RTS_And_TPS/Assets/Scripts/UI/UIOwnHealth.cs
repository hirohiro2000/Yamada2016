using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIOwnHealth : MonoBehaviour
{
    [SerializeField]
    private UIGirlTaskSelect    m_uiGirlTask        = null;
    [SerializeField]
    private GameObject          m_mainImage         =   null;
    
    private bool                m_onetimeUpdate   { get; set; }
    private RectTransform       m_rRectTrans      { get; set; }
    private LinkManager         m_rLinkManager    { get; set; }
    private TPSPlayer_HP        m_ownFighter      { get; set; }
    private GameObject          m_player          { get; set; }

    private static UIOwnHealth instance = null;
    
    float m_hp;

    void Awake()
    {
        instance        = this;
        m_onetimeUpdate = true;
        m_ownFighter    = null;
        m_rLinkManager  = FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );
        m_rRectTrans    = m_mainImage.GetComponent< RectTransform >();
    }
    void OnEnable()
    {
        instance = this;
    }
    void Update()
    {                         
        if ( m_player == null ) return;           
        if ( m_ownFighter == null )
        {
            GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < playerList.Length; i++)
            {
                if (playerList[i] == m_player) continue;
                if (playerList[i].GetComponent<PlayerCommander_Control>()) continue;

                m_ownFighter = playerList[i].GetComponent<TPSPlayer_HP>();

                transform.GetChild(4).GetComponent<Text>().text = playerList[i].GetComponent<NetPlayer_Control>().c_PlayerName;  
                             
                break;

            }

        }   

        if ( m_ownFighter == null ) return;
        
        if ( m_onetimeUpdate )
        {
            if (m_uiGirlTask.gameObject.activeInHierarchy)
            {
                m_uiGirlTask.UpPanel();
            }            
            m_onetimeUpdate = false;
        }

        RectTransform rt = GetComponent<RectTransform>();

        Text uiName = transform.GetChild(4).GetComponent<Text>();
        float targetX = -509.97f + uiName.text.Length * uiName.fontSize * uiName.transform.localScale.x * 0.5f;
        if ( rt.localPosition.x < targetX )
        {
            Vector3 p = rt.localPosition;
            p.x += ( targetX - p.x ) * 0.25f;                          
            rt.localPosition = p;
        }
                 
        float hp = m_ownFighter.m_CurHP;
        if ( hp < 0.0f)
        {
            hp = 0.0f;
        }
        float delta = hp - m_hp;
        float rate  = hp / m_ownFighter.m_MaxHP;

        Vector3 scale = m_rRectTrans.localScale;
        scale.x = rate;
        m_rRectTrans.localScale = scale;

        m_hp = hp;
                    
                 
    }

    static public void SetPlayer(GameObject player)
    {
        instance.m_player = player;
    }
    

}
