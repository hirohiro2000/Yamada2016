#define DEBUGLOG_ON
using UnityEngine;
using System.Collections;


public class UserLog
{



	static public void Kawaguchi(object message)
	{

#if DEBUGLOG_ON
		//Debug.Log(message);
#endif

	}

	static public void Sengoku(object message)
	{
#if DEBUGLOG_ON
		//Debug.Log(message);
#endif
	}

	static public void Nakano(object message)
	{
#if DEBUGLOG_ON
		//Debug.Log(message);
#endif
	}

	static public void Oki(object message)
	{
#if DEBUGLOG_ON
		//Debug.Log(message);
#endif
	}

	static public void Terauchi(object message)
	{
#if DEBUGLOG_ON
       //Debug.Log("tera : " + message);
#endif
	}

    static public void ErrorTerauchi(object msg)
    {
#if DEBUGLOG_ON
    //    Debug.LogError("tera : " +  msg);
#endif
    }

    /// <summary>
    /// ここからWarningLog
    /// </summary>

    static public void NakanoWarning(object message)
	{
#if DEBUGLOG_ON
		//Debug.LogWarning(message);
#endif
	}

}
