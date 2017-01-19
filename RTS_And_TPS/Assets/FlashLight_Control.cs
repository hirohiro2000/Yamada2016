using UnityEngine;
using System.Collections;

public class FlashLight_Control : MonoBehaviour {

    private float   c_LightTime     =   0.1f;

    private float   m_Timer         =   0.0f;
    private Light   m_rLight        =   null;

	// Use this for initialization
	void    Start()
    {
	    m_rLight    =   transform.FindChild( "_Light" ).GetComponent< Light >();
	}
	
	// Update is called once per frame
	void    Update()
    {
	    //  共通処理
        {
            //  カウントダウン
            m_Timer    +=  Time.deltaTime;

            //  ライトの更新
            float   timeInSecond    =   m_Timer - ( int )m_Timer;
            m_rLight.enabled        =   timeInSecond <= c_LightTime;
        }   
	}
}
