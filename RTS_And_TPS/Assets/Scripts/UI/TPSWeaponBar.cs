using UnityEngine;
using System.Collections;

public class TPSWeaponBar : MonoBehaviour
{
    [SerializeField]
    private GameObject m_mainImage = null;

    float   m_curValue;
    float   m_capacity;

    private static TPSWeaponBar instance = null;
    void Awake()
    {
        instance = GetComponent<TPSWeaponBar>();
    }
    void OnEnable()
    {
        instance = GetComponent<TPSWeaponBar>();
    }

    static public void Initialize(float capacity)
    {
        instance.m_capacity = capacity;
        instance.m_curValue = 0.0f;
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

        Vector3 localPosition = rt.localPosition;
        localPosition.x += rt.sizeDelta.x * delta / instance.m_capacity * 0.5f;
        rt.localPosition = localPosition;

        // Clear
        if (instance.m_curValue >= instance.m_capacity)
        {
            instance.m_curValue = 0.0f;
            rt.localScale = Vector3.one;
            rt.localPosition = Vector3.zero;
        }

    }

}
