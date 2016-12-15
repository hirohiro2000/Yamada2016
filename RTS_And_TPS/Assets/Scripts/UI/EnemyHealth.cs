using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    Health m_health = null;

    [SerializeField]
    HealthBar3D m_healthBar = null;

    [SerializeField]
    Text        m_text = null;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame 
    void Update()
    {
        m_healthBar.setValue( m_health.GetHP() / m_health.GetMaxHP() );
        m_text.text = "Lv " + ( m_health.GetLevel() + 1 ).ToString();
    }
}
