
using   UnityEngine;
using   System.Collections;

public class SkyManager : MonoBehaviour {

    [ System.Serializable ]
    public  class   SkyData{
        public  Material    skyMaterial     =   null;
        public  Material    cloudMaterial   =   null;
        public  float       cloudSpeed      =   0.0f;
    };

    public  SkyData[]       c_SkyData       =   null;

    private SkyData         m_rCurData      =   null;
    private Cloud_Control   m_rCloudControl =   null;

	private Material		m_rPrevMaterial = null;
	public	float			m_LerpTime = .0f;

	// Use this for initialization
	void    Start()
    {
        m_rCloudControl =   transform.FindChild( "CloudSystem" ).GetComponent< Cloud_Control >();
	}
	
	// Update is called once per frame
	void    Update()
    {
		if (m_rPrevMaterial == null)
			return;
		m_LerpTime += Time.deltaTime;
		if (m_LerpTime > 1.0f)
			m_LerpTime = 1.0f;
		RenderSettings.skybox.Lerp(m_rPrevMaterial, m_rCurData.skyMaterial, m_LerpTime);
    }

    public  void    ChangeSky( int _SkyIndex )
    {
        if( c_SkyData == null )     return;
        if( c_SkyData.Length == 0 ) return;

        //  スカイボックス変更
        int     index   =   _SkyIndex % c_SkyData.Length;
        SkyData rData   =   c_SkyData[ index ];

        //  データが同じ場合は変更処理を行わない
        if( rData == m_rCurData )   return;

		m_rPrevMaterial = RenderSettings.skybox;
		m_LerpTime = .0f;

		RenderSettings.skybox   =   rData.skyMaterial;

        m_rCloudControl.SetSpeed( rData.cloudSpeed );
        m_rCloudControl.SetMaterial( rData.cloudMaterial );

        m_rCurData  =   rData;
    }
}
