using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InputFlush : MonoBehaviour
{
    [SerializeField]
    private KeyCode   m_keyCode = KeyCode.None;
    [SerializeField]
    private RawImage  m_image   = null;
    [SerializeField]
    private Image     m_image2  = null;

    void Update()
    {
        if( m_image2 ){
            if ( Input.GetKeyDown(m_keyCode) )
            {
                Color color = m_image2.color;
                color.a = 1.2f;
                m_image2.color = color;
            }

            {
                Color color = m_image2.color;
                color.a *= 0.85f; 
                m_image2.color = color;
            }

            return;
        }


        if ( Input.GetKeyDown(m_keyCode) )
        {
            Color color = m_image.color;
            color.a = 5.0f;
            m_image.color = color;
        }

        {
            Color color = m_image.color;
            color.a *= 0.75f;
            m_image.color = color;
        }
    }

}
