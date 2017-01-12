
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

	// Use this for initialization
	void    Start()
    {
        m_rCloudControl =   transform.FindChild( "CloudSystem" ).GetComponent< Cloud_Control >();
	}
	
	// Update is called once per frame
	void    Update()
    {

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

        RenderSettings.skybox   =   rData.skyMaterial;

        m_rCloudControl.SetSpeed( rData.cloudSpeed );
        m_rCloudControl.SetMaterial( rData.cloudMaterial );

        m_rCurData  =   rData;
    }
}
