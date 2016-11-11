using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIRadar : MonoBehaviour {

    [SerializeField]
    private GameObject m_player         = null;
    //[SerializeField]
    //private GameObject m_ownFighter      = null;
    [SerializeField]
    private GameObject m_enemyFighter    = null;
    [SerializeField]
    private GameObject m_backGround    = null;

    [SerializeField]
    private float     m_searchRange      = 50.0f;

    [SerializeField]
    private Color      m_ownColor       = Color.blue;
    [SerializeField]
    private Color      m_enemyColor      = Color.red;
    

    struct DATA
    {
        public GameObject reference;
        public GameObject dst;
    }
	private	List<DATA>	m_uiSymbolList  = null;

    private static UIRadar instance = null;

    void Awake()
    {
        instance = GetComponent<UIRadar>();

        m_uiSymbolList = new List<DATA>();
    }
    void OnEnable()
    {
        instance = GetComponent<UIRadar>();

        m_uiSymbolList = new List<DATA>();
    }

    void Update()
    {
        if (m_player == null) return;

        Matrix4x4 worldToLocalMatrix = m_player.transform.worldToLocalMatrix;
        foreach (var item in m_uiSymbolList)
        {
            RectTransform rt = item.dst.GetComponent<RectTransform>();

            Vector3 relativePosition = worldToLocalMatrix.MultiplyPoint(item.reference.transform.position);

			item.dst.transform.SetParent( transform );

            Vector3 rtPosition   = relativePosition / m_searchRange;
                    rtPosition.y = 0.0f;
            float   maxLength    = 70.0f;

            if (rtPosition.sqrMagnitude > 1.0f)
            {
                rtPosition.Normalize();
            }
            rt.localPosition = rt.localPosition.normalized * ( Mathf.Min( rt.localPosition.magnitude, maxLength ) );

            //item.dst.SetActive(rtPosition.sqrMagnitude < 1.0f);
            item.dst.SetActive( true );
        }
        {
            //RectTransform rt = m_backGround.GetComponent<RectTransform>();
            //rt.eulerAngles = new Vector3(rt.eulerAngles.x, rt.eulerAngles.y, m_player.transform.eulerAngles.y);
        }

    }

    static public void SetPlayer(GameObject player)
    {
        instance.m_player = player;
    }

    static public void Add(GameObject src, Color rgba)
    {
        //if( !instance )                                 return;
        //if( !instance.gameObject.activeInHierarchy )    return;
        

        DATA data = new DATA();
        data.reference = src;
        data.dst = Instantiate(instance.m_enemyFighter);
        data.dst.transform.SetParent( instance.transform );
        data.dst.transform.localScale = Vector3.one;
        data.dst.GetComponent<RawImage>().color = rgba;
        data.dst.SetActive(true);

        instance.m_uiSymbolList.Add(data);
    }
    static public void Remove(GameObject obj)
    {
        //if( !instance )                                 return;
        //if( !instance.gameObject.activeInHierarchy )    return;

        int numSymbols = instance.m_uiSymbolList.Count;
        for (int i = numSymbols - 1; i >= 0; --i)
        {
            DATA item = instance.m_uiSymbolList[i];
            if (item.reference.Equals(obj))
            {
                Destroy(item.dst);
                instance.m_uiSymbolList.RemoveAt(i);
                break;
            }
        }

    }

    static public void AddOwnObject(GameObject src)
    {
        Add(src, instance.m_ownColor);
    }
    static public void AddEnemy(GameObject src)
    {
        Add(src, instance.m_enemyColor);
    }



}
