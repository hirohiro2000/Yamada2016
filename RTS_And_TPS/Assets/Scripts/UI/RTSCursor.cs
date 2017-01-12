
using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct Point
{
    public int X;
    public int Y;
    public static implicit operator Vector2(Point p)
    {
        return new Vector2(p.X, p.Y);
    }
}

public class RTSCursor : MonoBehaviour
{
    public enum MODE
    {
        eNone,
        eCamera,
        eUI,
    }

    [SerializeField]
    private Texture2D          m_cursorTexture = null;

    static public MODE        m_curMode    { get; private set; }
    static public GameObject  m_requester  { get; private set; }

    private void Start()
    {
        m_curMode = MODE.eNone;
        m_requester = null;
    }
    private void OnDisable ()
    {
        Cursor.SetCursor( null, Vector2.zero, CursorMode.Auto );
    }
    private void Update()
    {
        Cursor.SetCursor(m_cursorTexture, Vector2.zero, CursorMode.Auto);
    }
    public  bool Require( GameObject requester, MODE mode )
    {
        if ( m_requester != null )        return false;
        if ( mode        == MODE.eNone )  return false;

        m_curMode   = mode;
        m_requester = requester;
        
        return true;
    }
    public  void Destruction( GameObject reqester )
    {
        if ( m_requester != reqester )  return;

        m_curMode   = MODE.eNone;
        m_requester = null;

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
    

    [DllImport("user32.dll")]
    public static extern bool SetCursorPos(int X, int Y);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetCursorPos(out Point pos);
  
}
