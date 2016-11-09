using UnityEngine;
using System.Collections;

public class TPSHpBar : MonoBehaviour
{
    [SerializeField]
    private GameObject  m_mainImage =   null;
    

    float   m_hp;
    float   m_maxHP;

    private static  TPSHpBar        instance        =   null;
    private static  RectTransform   m_rRectTrans    =   null;
    void Awake()
    {
        instance        =   GetComponent< TPSHpBar >();
        m_rRectTrans    =   instance.m_mainImage.GetComponent< RectTransform >();
    }
    void OnEnable()
    {
        instance        =   GetComponent< TPSHpBar >();
        m_rRectTrans    =   instance.m_mainImage.GetComponent< RectTransform >();
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

        Vector3 scale = m_rRectTrans.localScale;
        scale.x = rate;
        m_rRectTrans.localScale = scale;

        instance.m_hp = hp;

    }

}
