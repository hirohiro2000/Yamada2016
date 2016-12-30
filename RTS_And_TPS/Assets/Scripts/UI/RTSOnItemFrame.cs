using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class RTSOnItemFrame : MonoBehaviour, 
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public  int id { set; get; }
    public  ItemController   m_itemController    { set; get; }
    private UIGirlTaskSelect m_uiGirlTaskSelect  { set; get; }
    private RTSCursor        m_cursor            { set; get; }

    void Start()
    {
        Transform   rHUD    = GameObject.Find( "Canvas" ).transform.FindChild("RTS_HUD");
        m_uiGirlTaskSelect  = rHUD.GetComponent<UIGirlTaskSelect>();
        m_cursor            = rHUD.GetComponent<RTSCursor>();
    }
    void OnDisable()
    {
        if (m_itemController.GetForcus() == id)
        {
            m_cursor.Destruction(this.transform.parent.gameObject);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 作成に必要なコストが足りない場合[-1]を設定する
        int forcusID =  m_itemController.CheckWhetherTheCostIsEnough( id ) ? id : -1;

        m_itemController.SetForcus(forcusID);
        m_uiGirlTaskSelect.SetForcus(id);

        m_cursor.Require(this.transform.parent.gameObject, RTSCursor.MODE.eUI);

        //  効果音再生
        SoundController.PlayNow( "UI_FocusChange", 0.0f, 0.1f, Random.Range( 0.95f, 1.05f ), 1.0f );
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        m_itemController.SetForcus(-1);
        m_uiGirlTaskSelect.SetForcus(-1);
        m_cursor.Destruction(this.transform.parent.gameObject);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (m_itemController.CheckWhetherTheCostIsEnough(id))
        {
            m_itemController.SetForcus(id);
            m_uiGirlTaskSelect.SetForcus(id);
            m_uiGirlTaskSelect.SelectOK();

            //  効果音再生  
            SoundController.PlayNow( "UI_Click2", 0.0f, 0.1f, 1.24f, 1.0f );
            SoundController.PlayNow( "UI_Click", 0.0f, 0.1f, 0.84f, 1.0f );
        }
        else{
            //  効果音再生
            SoundController.PlayNow( "UI_NG", 0.0f, 0.1f, 0.64f, 1.0f );
        }
    }

}
