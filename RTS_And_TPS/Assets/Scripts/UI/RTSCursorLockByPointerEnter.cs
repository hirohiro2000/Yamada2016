using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class RTSCursorLockByPointerEnter : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private RTSCursor.MODE  m_targetMode    = RTSCursor.MODE.eUI;
    private RTSCursor       m_cursor        = null;

    void Start()
    {
        Transform   rHUD    = GameObject.Find( "Canvas" ).transform.FindChild("RTS_HUD");
        m_cursor            = rHUD.GetComponent<RTSCursor>();
    }
    void OnDisable()
    {
        m_cursor.Destruction(gameObject);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        m_cursor.Require(gameObject, RTSCursor.MODE.eUI);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        m_cursor.Destruction(gameObject);
    }

}
