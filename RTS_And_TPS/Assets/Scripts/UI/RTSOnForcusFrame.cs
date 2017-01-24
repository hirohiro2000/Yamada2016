using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class RTSOnForcusFrame : MonoBehaviour, IPointerClickHandler
{
    private UIGirlTaskSelect m_uiGirlTaskSelect  { set; get; }

    void Start()
    {
        Transform   rHUD    = GameObject.Find( "Canvas" ).transform.FindChild("RTS_HUD");
        m_uiGirlTaskSelect  = rHUD.GetComponent<UIGirlTaskSelect>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if( eventData.button == PointerEventData.InputButton.Right )    return;
        if ( m_uiGirlTaskSelect.canCreate( m_uiGirlTaskSelect.m_computePosition ) == false )                  return;
                   
        m_uiGirlTaskSelect.ClikedForcusFrame();
    }

}
