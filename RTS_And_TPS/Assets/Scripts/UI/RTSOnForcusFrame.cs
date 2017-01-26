using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class RTSOnForcusFrame : MonoBehaviour, 
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
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
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.GetChild(7).gameObject.SetActive(true);
        GetComponent<Image>().color = new Color(0.13f, 0.13f, 0.13f, 1.0f);
        // 効果音再生
        SoundController.PlayNow("UI_MenuOpen", 0.0f, 0.1f, 1.0f, 1.0f);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.GetChild(7).gameObject.SetActive(false);
        GetComponent<Image>().color = new Color( 0.11f, 0.11f, 0.11f, 1.0f );
    }

}
