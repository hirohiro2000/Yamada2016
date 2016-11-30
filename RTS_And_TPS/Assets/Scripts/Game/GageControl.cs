using UnityEngine;
using System.Collections;

public class GageControl : MonoBehaviour {

    public  Transform   m_rGageTrans    =   null;

	// Use this for initialization
	void    Start()
    {
        //  ゲージへのアクセス
	    m_rGageTrans    =   transform.FindChild( "_Gage" );
	}

    //  アクセス
    public  void    SetGage( float _GageRate )
    {
        //  正規化
        _GageRate   =   Mathf.Clamp( _GageRate, 0.0f, 1.0f );

        //  反映
        m_rGageTrans.localScale =   new Vector3( _GageRate, m_rGageTrans.localScale.y, m_rGageTrans.localScale.z );
    }
}
