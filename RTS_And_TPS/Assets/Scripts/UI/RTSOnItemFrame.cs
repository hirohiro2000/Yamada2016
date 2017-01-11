using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class RTSOnItemFrame : MonoBehaviour, 
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public  int id { set; get; }
    private UIGirlTaskSelect m_uiGirlTaskSelect  { set; get; }

    void Start()
    {
        Transform   rHUD    = GameObject.Find( "Canvas" ).transform.FindChild("RTS_HUD");
        m_uiGirlTaskSelect  = rHUD.GetComponent<UIGirlTaskSelect>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_uiGirlTaskSelect.SetForcus( id );
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        m_uiGirlTaskSelect.SetForcus(-1);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if( eventData.button == PointerEventData.InputButton.Right )  return;

        m_uiGirlTaskSelect.SelectOK( id );
    }

}
