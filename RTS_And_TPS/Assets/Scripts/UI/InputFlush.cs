using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InputFlush : MonoBehaviour
{
    [SerializeField]
    private KeyCode   m_keyCode = KeyCode.None;
    [SerializeField]
    private RawImage  m_image   = null;

    void Update()
    {
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
