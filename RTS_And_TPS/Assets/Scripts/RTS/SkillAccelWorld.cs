using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillAccelWorld : MonoBehaviour
{
    class DATA
    {
        public GameObject reference;
        public float      identSpeed;
        public float      slowSpeed;
    }
    private	List<DATA>	    m_enemyList     = null;
    private float           m_skillRange    = 10.0f;
    private bool            m_isActive      = false;

    private GameObject      m_effect        { get; set; }
    Color                   m_color         = Color.white;
    MaterialPropertyBlock   m_matProperty   { get; set; }
    Renderer                m_renderer      { get; set; }

    //  外部へのアクセス
    private LinkManager         m_rLinkManager  = null;

    private void Awake()
    {
        m_enemyList     = new List<DATA>();
        m_rLinkManager  = GameObject.Find("LinkManager").GetComponent<LinkManager>();

        // エフェクト
        GameObject org = GameObject.Find("SkillItemShell").transform.GetChild(1).gameObject;
        m_effect    =   Instantiate( org );
        m_effect.SetActive(false);
        m_matProperty   = new MaterialPropertyBlock();
        m_renderer      = m_effect.GetComponent<Renderer>();
        m_color         = m_renderer.material.GetColor("_TintColor");
        m_matProperty.SetColor( "_TintColor", m_color );
        m_renderer.SetPropertyBlock( m_matProperty );

    }
    private void Update()
    {
        if ( m_isActive == false )  return;

        // 強制的に移動スピードを下げる
        for (int i = 0; i < m_enemyList.Count; i++)
        {
            DATA rData = m_enemyList[i];

            if (rData.reference == null) continue;

            GameObject      ene     = rData.reference;
            NavMeshAgent    agent   = rData.reference.GetComponent<NavMeshAgent>();

            agent.speed = rData.slowSpeed;
        }
        //  無効になった項目を削除
        for (int i = 0; i < m_enemyList.Count; i++)
        {
            DATA rData = m_enemyList[i];
            if (rData.reference) continue;

            //  項目を削除
            m_enemyList.Remove(rData);

            //  最初に戻る
            i = -1;
        }

        if ( m_effect.transform.localScale.x < m_skillRange )
        { 
            float scale = m_effect.transform.localScale.x;
            scale = Mathf.Min( scale + 0.45f, m_skillRange );
            m_effect.transform.localScale = new Vector3( scale, scale, scale );
            m_effect.SetActive( m_effect.transform.localScale.x < m_skillRange );
            m_color.a = 0.5f * ( 1.0f - ( scale / m_skillRange ));
            m_matProperty.SetColor( "_TintColor", m_color );
            m_renderer.SetPropertyBlock( m_matProperty );
        }


    }
    public  void SkillInvoke()
    {
        if ( CanInvokeSkill() == false )    return;
        SkillInvoker.UsedMP( 1 );

        ReferenceWrapper rEnemyShell = FunctionManager.GetAccessComponent< ReferenceWrapper >( "EnemySpawnRoot" );

        for (int i = 0; i < rEnemyShell.m_active_enemy_list.Count; i++)
        {
            DATA add = new DATA();

            GameObject      ene     = rEnemyShell.m_active_enemy_list[i];
            NavMeshAgent    agent   = ene.GetComponent<NavMeshAgent>();

            float distanceSq = ( m_rLinkManager.m_rLocalPlayer.transform.position - ene.transform.position ).sqrMagnitude;
            if ( distanceSq > m_skillRange*m_skillRange )  continue;

            add.reference   = ene;
            add.identSpeed  = agent.speed;
            add.slowSpeed   = agent.speed * 0.1f;

            agent.speed = agent.speed * 0.1f;
            m_enemyList.Add(add);
        }

        //  配置設定
        m_effect.transform.SetParent( m_rLinkManager.m_rLocalPlayer.transform );
        m_effect.transform.localPosition = new Vector3( 0.0f, 0.3f, 0.0f );
        m_effect.transform.localRotation = Quaternion.identity;
        m_effect.transform.localScale    = Vector3.zero;
        m_color.a = 1.0f;

        m_isActive = true;

    }
    public  void SkillDeath()
    {
        m_enemyList.Clear();
        m_isActive = false;
    }
    
    private bool CanInvokeSkill()
    {
        return ( SkillInvoker.CurrentMagicalPower() > 0 );
    }

}
