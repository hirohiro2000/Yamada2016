using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RangeEffect : MonoBehaviour
{
    [SerializeField]
    private GameObject          m_rangeEffect =   null;

    private float               m_timer               { get; set; }
    private List<GameObject>    m_rangeEffectArray    { get; set; }

    // Use this for initialization
    void Start()
    {
        m_timer = 0.0f;
        m_rangeEffectArray = new List<GameObject>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if ( m_timer <= 0.0f )
        {
            GameObject add = Instantiate( m_rangeEffect );
            add.transform.position          = transform.position;
            add.transform.localRotation     = Quaternion.identity;
            add.transform.localScale        = Vector3.zero;

            m_rangeEffectArray.Add( add );

            m_timer = 1.0f;

        }

        m_timer -= Time.deltaTime;

        int numLoop = m_rangeEffectArray.Count; 
        for (int i = 0; i < numLoop; i++)
        {
            GameObject item = m_rangeEffectArray[i];
            item.transform.position          = transform.position;
            item.transform.localRotation     = Quaternion.identity;
            
            float endScale = transform.localScale.x * 10.0f;
            float curScale = item.transform.localScale.x;
            float scale    = curScale + 1.0f;
            item.transform.localScale = new Vector3( scale, scale, scale );

        }

        //  無効になった項目を削除
        for (int i = 0; i < m_rangeEffectArray.Count; i++)
        {
            GameObject item = m_rangeEffectArray[i];
            float endScale = transform.localScale.x * 10.0f;
            float curScale = item.transform.localScale.x;

            if ( endScale >= curScale ) continue;

            Destroy(item);

            //  項目を削除
            m_rangeEffectArray.Remove(item);

            //  最初に戻る
            i = -1;
        }
    }
}
