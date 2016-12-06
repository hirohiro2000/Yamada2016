
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

    static public MODE        m_curMode    { get; private set; }
    static public GameObject  m_requester  { get; private set; }
    
    public void Start()
    {
        m_curMode = MODE.eNone;
        m_requester = null;
    }
    public bool Require( GameObject requester, MODE mode )
    {
        if ( m_requester != null )        return false;
        if ( mode        == MODE.eNone )  return false;

        m_curMode   = mode;
        m_requester = requester;
        
        return true;
    }
    public void Destruction( GameObject reqester )
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
