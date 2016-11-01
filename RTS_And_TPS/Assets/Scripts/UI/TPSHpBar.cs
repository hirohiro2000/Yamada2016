using UnityEngine;
using System.Collections;

public class TPSHpBar : MonoBehaviour
{
    [SerializeField]
    private GameObject m_mainImage = null;

    float   m_hp;
    float   m_maxHP;

    private static TPSHpBar instance = null;
    void Awake()
    {
        instance = GetComponent<TPSHpBar>();
    }
    void OnEnable()
    {
        instance = GetComponent<TPSHpBar>();
    }

    static public void Initialize(float maxHP)
    {
        instance.m_maxHP = maxHP;
        instance.m_hp = maxHP;
    }
    static public void SetHP(float hp)
    {
        if (hp < 0.0f)
        {
            hp = 0.0f;
        }
        float delta = hp - instance.m_hp;
        float rate = hp / instance.m_maxHP;

        RectTransform rt = instance.m_mainImage.GetComponent<RectTransform>();
        Vector3 scale = rt.localScale;
        scale.x = rate;
        rt.localScale = scale;

        Vector3 localPosition = rt.localPosition;
        localPosition.x += rt.sizeDelta.x * delta / instance.m_maxHP * 0.5f;
        rt.localPosition = localPosition;

        instance.m_hp = hp;

    }

}
