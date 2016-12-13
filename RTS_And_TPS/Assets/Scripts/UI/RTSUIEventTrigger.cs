using UnityEngine;
using System.Collections;

public class RTSUIEventTrigger : MonoBehaviour
{
    private RTSCursor m_cursor      = null;

    void Start()
    {
        Transform   rHUD    = GameObject.Find( "Canvas" ).transform.FindChild("RTS_HUD");
        m_cursor            = rHUD.GetComponent<RTSCursor>();
    }

    void OnDisable()
    {
        m_cursor.Destruction(gameObject);
    }
    public void EventCursorLock()
    {
        m_cursor.Require(gameObject, RTSCursor.MODE.eUI);
    }
    public void EventCursorUnLock()
    {
        m_cursor.Destruction(gameObject);
    }

}
