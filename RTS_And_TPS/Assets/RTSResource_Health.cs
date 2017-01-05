
using   UnityEngine;
using   UnityEngine.UI;
using   System.Collections;

public class RTSResource_Health : MonoBehaviour {

    public  ResourceParameter   m_rResoureceParam   =   null;
    public  HealthBar3D         m_rHealthBar        =   null;
    public  Text                m_rLevelText        =   null;

	// Update is called once per frame
	void    Update()
    {
        if( !m_rResoureceParam )    return;
        if( !m_rHealthBar )         return;
        if( !m_rLevelText )         return;

        //  ゲージを更新
	    m_rHealthBar.setValue( ( float )m_rResoureceParam.m_curHp / m_rResoureceParam.GetCurLevelParam().hp );

        //  レベル更新 
        m_rLevelText.text   =   "Lv " + ( m_rResoureceParam.m_level + 1 );
	}
}
