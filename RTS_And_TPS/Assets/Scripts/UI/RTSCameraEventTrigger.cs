using UnityEngine;
using System.Collections;
using System.IO;

public class RTSCameraEventTrigger : MonoBehaviour
{
    [SerializeField]
    private RTSCamera m_rtsCamera   = null;

    [SerializeField]
    private RTSCursor m_cursor      = null;

    private Point m_cursorPoint;
    public void EventCameraMoveHorizontalStart()
    {
        m_cursor.Require(gameObject, RTSCursor.MODE.eCamera);
        m_rtsCamera.m_actionState = RTSCamera.ActionState.eMoveHorizontal;
        
        RTSCursor.GetCursorPos( out m_cursorPoint );
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }
    public void EventCameraMoveHorizontalEnd()
    {
        m_cursor.Destruction(gameObject);
        m_rtsCamera.m_actionState = RTSCamera.ActionState.eNone;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
        RTSCursor.SetCursorPos( m_cursorPoint.X, m_cursorPoint.Y );
    }
    public void EventCameraMoveVerticalStart()
    {
        m_cursor.Require(gameObject, RTSCursor.MODE.eCamera);
        m_rtsCamera.m_actionState = RTSCamera.ActionState.eMoveVertical;

        RTSCursor.GetCursorPos( out m_cursorPoint );
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }
    public void EventCameraMoveVerticalEnd()
    {
        m_cursor.Destruction(gameObject);
        m_rtsCamera.m_actionState = RTSCamera.ActionState.eNone;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
        RTSCursor.SetCursorPos( m_cursorPoint.X, m_cursorPoint.Y );
    }
    public void EventCameraSwitchCurrentForcus()
    {
        m_rtsCamera.m_isForcus = !m_rtsCamera.m_isForcus;
    }


    void OnGUI()
    {
        GUI.Label(new Rect(10,120,120,100), RTSCursor.m_curMode.ToString());
        GUI.Label(new Rect(10,140,120,100), m_rtsCamera.m_actionState.ToString());

        Point p;
        RTSCursor.GetCursorPos( out p );

        GUI.Label(new Rect(10,160,120,100), p.X.ToString() + ":" + p.Y.ToString() );

        GUI.Label(new Rect(10,180,120,100), Input.mousePosition.x.ToString() + ":" + Input.mousePosition.y.ToString() );
    }

}
