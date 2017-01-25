using UnityEngine;
using System.Collections;

public class MaterialSwitchToConvert : MonoBehaviour
{                   
    [SerializeField]
    private Renderer[]              m_renderer                  = null;
    [SerializeField]
    private Material                m_matFocusFieldTower        = null;
    [SerializeField]
    private Material                m_matPermeateFieldTower     = null;
    [SerializeField]
    private Material                m_matVirtualFieldTower      = null;

    private bool                    m_isActive      { get; set; }
    private bool                    m_canSee        { get; set; }
    private bool                    m_isVirtual     { get; set; }
    private float                   m_UVAnimationY  { get; set; }
    private Material[]              m_mainMaterial  { get; set; }
    private MaterialPropertyBlock[] m_matProperty   { get; set; }

    private void Awake()
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
        }
    }
    public  void Activate()
    {
        m_isActive = true;
        for (int i = 0; i < m_renderer.Length; i++)
        {
            m_renderer[i].material = m_matFocusFieldTower;
        }
    }
    public  void Deactivate()
    {
        m_isActive = false;
        for (int i = 0; i < m_renderer.Length; i++)
        {
            m_renderer[i].material = m_mainMaterial[i];
        }
    }
    public  void SetVisibility( bool canSee )
    {
        m_canSee = canSee;            
    }
    public  void SetVirtual( bool isVirtual )
    {
        m_isVirtual = isVirtual;
    }
    private void Update()
    {                  
        for (int i = 0; i < m_renderer.Length; i++)
        {
            if (m_isVirtual)
                m_renderer[i].material = m_matVirtualFieldTower;
            else if (m_canSee)
                m_renderer[i].material = m_matFocusFieldTower;
            else
                m_renderer[i].material = m_matPermeateFieldTower;
            
            m_UVAnimationY -= 0.0001f;  
            m_matProperty[i].SetFloat( "_UVAnimationY", m_UVAnimationY );
            m_renderer[i].SetPropertyBlock( m_matProperty[i] );
        }
    }

}
