using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIRadar : MonoBehaviour {

    [SerializeField]
    private GameObject m_player         = null;
    [SerializeField]
    private GameObject m_ownFighter      = null;
    [SerializeField]
    private GameObject m_enemyFighter    = null;

    [SerializeField]
    private float     m_searchRange      = 50.0f;

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

    void Update()
    {

        foreach (var item in m_uiSymbolList)
        {
            RectTransform rt = item.dst.GetComponent<RectTransform>();

            Vector3 relativePosition = m_player.transform.worldToLocalMatrix.MultiplyPoint( item.reference.transform.position );
            Vector3 rtPosition = relativePosition / m_searchRange;
            
            rt.localPosition = new Vector3( rtPosition.x, rtPosition.z, 0.0f ) * 50.0f;

            item.dst.SetActive( rtPosition.sqrMagnitude < 1.0f );

        }
    }

    static public void Add(GameObject src, Color rgba)
    {
        DATA data = new DATA();
        data.reference = src;
        data.dst = Instantiate(instance.m_enemyFighter);
        data.dst.transform.parent = instance.transform;

        RectTransform rt = data.dst.GetComponent<RectTransform>();
        rt.position = src.transform.position - instance.m_player.transform.position;
        rt.position /= 100.0f;

        data.dst.SetActive(true);

        instance.m_uiSymbolList.Add(data);
    }
    static public void Remove(GameObject obj)
    {
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
        Add(src, Color.blue);
    }
    static public void AddEnemy(GameObject src)
    {
        Add(src, Color.red);
    }



}
