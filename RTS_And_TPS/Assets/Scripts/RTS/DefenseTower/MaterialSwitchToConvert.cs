using UnityEngine;
using System.Collections;

public class MaterialSwitchToConvert : MonoBehaviour
{
    [SerializeField]
    Renderer[]              m_renderer              = null;
    [SerializeField]
    Material                m_matFocusFieldTower    = null;

    float                   m_UVAnimationY  { get; set; }
    Material[]              m_mainMaterial  { get; set; }
    MaterialPropertyBlock[] m_matProperty   { get; set; }


    public void Activate()
    {
        m_UVAnimationY  = 0.0f;
        m_matProperty   = new MaterialPropertyBlock[m_renderer.Length];
        m_mainMaterial  = new Material[m_renderer.Length];

        for (int i = 0; i < m_renderer.Length; i++)
        {
            m_matProperty[i]    = new MaterialPropertyBlock();

            m_matProperty[i].SetColor( "_Color", m_renderer[i].material.GetColor("_Color") );
            Texture mainTex = m_renderer[i].material.GetTexture("_MainTex");
            if ( mainTex != null )
                m_matProperty[i].SetTexture( "_MainTex", mainTex );

            m_mainMaterial[i]   = m_renderer[i].material;
            m_renderer[i].material = m_matFocusFieldTower;
        }

    }
    public void Deactivate()
    {
        for (int i = 0; i < m_renderer.Length; i++)
        {
            m_renderer[i].material = m_mainMaterial[i];
        }
    }

    void Update()
    {
        for (int i = 0; i < m_renderer.Length; i++)
        {
            m_UVAnimationY -= 0.0001f;  
            m_matProperty[i].SetFloat( "_UVAnimationY", m_UVAnimationY );
            m_renderer[i].SetPropertyBlock( m_matProperty[i] );
        }
    }

}
