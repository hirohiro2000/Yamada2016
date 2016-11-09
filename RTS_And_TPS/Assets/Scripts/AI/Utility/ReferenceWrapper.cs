using UnityEngine;
using System.Collections;

public class ReferenceWrapper : MonoBehaviour {

    public GameObject m_home_base { get; private set; }
    public GameObject m_attack_object_root { get; private set; }

    void Awake()
    {
        m_home_base = GameObject.Find("Homebase");
        m_attack_object_root = GameObject.Find("EnemyAttackObjectRoot");
    }

}
