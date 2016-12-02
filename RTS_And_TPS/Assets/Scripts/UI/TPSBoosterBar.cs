using UnityEngine;
using System.Collections;

public class TPSBoosterBar : MonoBehaviour
{

    [SerializeField]
    private GameObject m_mainImage = null;
    Vector3 m_mainImageDefaultPosition;

    float m_curValue;
    float m_capacity;


    private static TPSBoosterBar instance = null;
    void Awake()
    {
        instance = GetComponent<TPSBoosterBar>();
        m_mainImageDefaultPosition = m_mainImage.transform.localPosition;
    }
    void OnEnable()
    {
        instance = GetComponent<TPSBoosterBar>();
        m_mainImageDefaultPosition = m_mainImage.transform.localPosition;
    }

    static public void Initialize(float capacity)
    {
        instance.m_capacity = capacity;

        RectTransform rt = instance.m_mainImage.GetComponent<RectTransform>();
        instance.m_curValue = 0.0f;
        rt.localScale = Vector3.one;
    }
    static public void Consumption(float value)
    {
        instance.m_curValue += value;

        float delta = value;
        float rate = (instance.m_capacity - instance.m_curValue) / instance.m_capacity;

        RectTransform rt = instance.m_mainImage.GetComponent<RectTransform>();
        Vector3 scale = rt.localScale;
        scale.x = rate;
        rt.localScale = scale;

    }
    static public void Storage(float value)
    {
        instance.m_curValue -= value;

        float delta = value;
        float rate = (instance.m_capacity - instance.m_curValue) / instance.m_capacity;

        RectTransform rt = instance.m_mainImage.GetComponent<RectTransform>();
        Vector3 scale = rt.localScale;
        scale.x = rate;
        rt.localScale = scale;

    }
    static public void SetGage( float _GageRate )
    {
        if( !instance ) return;

        //  スケールを取得
        Vector3 scale   =   instance.m_mainImage.transform.localScale;
                scale.x =   _GageRate;

        //  変更を反映
        instance.m_mainImage.transform.localScale   =   scale;
    }
}
