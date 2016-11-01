using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GuideHitEffect : MonoBehaviour
{

    [SerializeField]
    private float m_life = 0.5f;

    [SerializeField]
    private float m_uiCircle = 100.0f;

    [SerializeField]
    private GameObject m_guideOriginal = null;

    class DATA
    {
        public GameObject reference;
        public GameObject dst;
        public float lifeTimer;
    }
    private List<DATA> m_guides = null;

    private RectTransform m_parentTrans;
    private Camera m_uiCamera;

    private static GuideHitEffect instance = null;

    void Awake()
    {
        instance = GetComponent<GuideHitEffect>();
        m_guides = new List<DATA>();

        RectTransform rt = GetComponent<RectTransform>();
        m_parentTrans = rt.parent.GetComponent<RectTransform>();

        Canvas[] canvasArr = GetComponentsInParent<Canvas>();
        for (int i = 0; i < canvasArr.Length; i++)
        {
            if (canvasArr[i].isRootCanvas)
            {
                m_uiCamera = canvasArr[i].worldCamera;
            }
        }

    }
    void OnEnable()
    {
        instance = GetComponent<GuideHitEffect>();
        m_guides = new List<DATA>();

        RectTransform rt = GetComponent<RectTransform>();
        m_parentTrans = rt.parent.GetComponent<RectTransform>();

        Canvas[] canvasArr = GetComponentsInParent<Canvas>();
        for (int i = 0; i < canvasArr.Length; i++)
        {
            if (canvasArr[i].isRootCanvas)
            {
                m_uiCamera = canvasArr[i].worldCamera;
            }
        }

    }

    void Update()
    {
        int numGuides = m_guides.Count;
        for (int i = numGuides - 1; i >= 0; --i)
        {
            DATA item = m_guides[i];

            Vector3 screenPos = Camera.main.WorldToScreenPoint(item.reference.transform.position);
            Vector2 localPos = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(m_parentTrans, screenPos, m_uiCamera, out localPos);

            if (screenPos.z < 0.0f)
                localPos.y = -localPos.y;

            RectTransform rt = item.dst.GetComponent<RectTransform>();
            rt.localPosition = localPos.normalized * m_uiCircle;

            float angle = Mathf.Atan2(rt.localPosition.y, rt.localPosition.x) - Mathf.PI * 0.5f;
            rt.eulerAngles = new Vector3(rt.eulerAngles.x, rt.eulerAngles.y, angle * Mathf.Rad2Deg);

            RawImage ri = item.dst.GetComponent<RawImage>();
            Color color = ri.color;
            color.a = (item.lifeTimer / m_life);
            ri.color = color;

            item.lifeTimer -= Time.deltaTime;
            if (item.lifeTimer <= 0.0f)
            {
                Destroy(item.dst);
                m_guides.RemoveAt(i);
            }
        }

    }

    static public void Add(GameObject attacker)
    {
        Vector3 screenPos = Camera.main.WorldToViewportPoint(attacker.transform.position);
        if ((screenPos.x >= 0.0f && screenPos.x <= 1.0f) &&
            (screenPos.y >= 0.0f && screenPos.y <= 1.0f))
        {
            return;
        }

        GameObject dst = Instantiate(instance.m_guideOriginal);
        dst.transform.SetParent( instance.transform );
        dst.transform.localScale = Vector3.one;
        dst.SetActive(true);

        DATA data = new DATA();

        data.reference = attacker;
        data.dst = dst;
        data.lifeTimer = instance.m_life;
        instance.m_guides.Add(data);

    }


}
