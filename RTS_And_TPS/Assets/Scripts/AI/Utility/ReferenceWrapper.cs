using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReferenceWrapper : MonoBehaviour {

    public GameObject m_home_base { get; private set; }
    public GameObject m_attack_object_root { get; private set; }
    public List<GameObject> m_active_enemy_list { get; private set; }

    void Awake()
    {
        m_home_base = GameObject.Find("Homebase");
        m_attack_object_root = GameObject.Find("EnemyAttackObjectRoot");
        m_active_enemy_list = GetComponent<EnemyGenerator>().GetCurrentHierachyList();
    }

   void Update()
    {
        UserLog.Terauchi(m_active_enemy_list.Count);
    }

}
