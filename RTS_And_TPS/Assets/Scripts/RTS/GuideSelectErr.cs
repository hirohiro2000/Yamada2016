using UnityEngine;
using System.Collections;

public class GuideSelectErr : MonoBehaviour
{
    Color                   m_color         = Color.white;
    MaterialPropertyBlock   m_matProperty   { get; set; }
    Renderer                m_renderer      { get; set; }

    // Use this for initialization
    void Start()
    {
        m_matProperty   = new MaterialPropertyBlock();
        m_renderer      = GetComponent<Renderer>();
        m_color         = m_renderer.material.GetColor("_Color");
        m_matProperty.SetColor( "_Color", m_color );
        m_renderer.SetPropertyBlock( m_matProperty );
    }

    // Update is called once per frame
    void Update()
    {

        if (m_color.a > 0.8f)
        {
            m_color.a -= 0.01f;
        }
        else
        {
            Destroy(gameObject);
        }

        m_matProperty.SetColor( "_Color", m_color );
        m_renderer.SetPropertyBlock( m_matProperty );
    }

}
