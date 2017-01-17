using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AppearInfopanel : MonoBehaviour
{
    float m_openWaitTimer       { get; set; }
    float m_closeWaitTimer      { get; set; }
    bool  m_targetActiveFlag    { get; set; }

    void Start()
    {
        m_closeWaitTimer    = 0.0f;
        m_targetActiveFlag  = true;
    }
    void Update()
    {
        if (m_targetActiveFlag)
        {
            // 開くアニメーション
            RectTransform rt = GetComponent<RectTransform>();

            Vector2 size = rt.localScale;

            size.x += 20.0f*Time.deltaTime;

            rt.localScale = size;
            
            if ( size.x >= 1.0 )
            {      
                rt.localScale = Vector3.one;          
                if ( m_openWaitTimer > 0.0f )
                {
                    m_openWaitTimer -= Time.deltaTime;
                }
            }

        }
        else
        {
            // 閉じるアニメーション
            RectTransform rt = GetComponent<RectTransform>();

            Vector3 size = rt.localScale;

            size.x -= 20.0f*Time.deltaTime;

            rt.localScale = size;
            
            if ( size.x <= 0.0f )
            {                
                rt.localScale = new Vector2( 0.0f, 1.0f );
                if ( m_closeWaitTimer > 0.0f )
                {
                    m_closeWaitTimer -= Time.deltaTime;
                }
                else
                {
                    gameObject.SetActive( false );
                }

            }

        }

    }
    void Open()
    {
        if ( m_closeWaitTimer > 0.0f )
        {
            RectTransform rt = GetComponent<RectTransform>();
            rt.localScale = Vector3.one;
        }

        gameObject.SetActive(true);
        m_targetActiveFlag = true;
        m_openWaitTimer   = 0.5f;

    }
    void Close()
    {
        if ( m_openWaitTimer > 0.0f )
        {
            RectTransform rt = GetComponent<RectTransform>();
            rt.localScale = new Vector2( 0.0f, 1.0f );
            gameObject.SetActive( false );
        }

        m_targetActiveFlag = false;
        m_closeWaitTimer   = 1.0f;
    }
      
    // 外部からの利用関数
    public void SetActive( bool isActive )
    {
        if ( m_targetActiveFlag == isActive )   return;

        if (isActive == true)
        {
            Open();       
        }
        else
        {
            Close();
        }

    }
    public bool IsActive() { return m_targetActiveFlag; }

}
