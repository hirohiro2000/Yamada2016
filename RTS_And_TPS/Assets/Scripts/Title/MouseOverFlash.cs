using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class MouseOverFlash : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    private Color m_change_color = Color.yellow;
    private Color m_default_color = Color.white;
    private Text   m_change_text = null;
    void Start()
    {
        m_change_text = GetComponentInChildren<Text>();
        m_change_text.color = m_default_color;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        m_change_text.color = m_change_color;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_change_text.color = m_default_color;
    }
}
