
using   UnityEngine;
using   System.Collections;

public class SkyManager : MonoBehaviour {

    [ System.Serializable ]
    public  class   SkyData{
        public  Material    skyMaterial     =   null;
        public  Material    cloudMaterial   =   null;
        public  float       cloudSpeed      =   0.0f;
        public  Color       fogColor        =   Color.black;
    };

    public  SkyData[]       c_SkyData       =   null;

    private SkyData         m_rCurData      =   null;
    private Cloud_Control   m_rCloudControl =   null;

	[SerializeField, Range(.1f,10.0f)]
	public float m_ChangeTime = 1.0f;

	private float m_BlendValue;

    private Color           m_PrevFogColor  =   Color.black;
    private Color           m_NextFogColor  =   Color.black;

	// Use this for initialization
	void    Start()
    {
        m_rCloudControl =   transform.FindChild( "CloudSystem" ).GetComponent< Cloud_Control >();


		ChangeMaterialTexture(c_SkyData[0].skyMaterial,1.0f);
    }
	
	// Update is called once per frame
	void    Update()
    {
        if( m_rCurData == null )    return;

		m_BlendValue += Time.deltaTime / m_ChangeTime;
		if (m_BlendValue > 1.0f)
			m_BlendValue = 1.0f;
        RenderSettings.skybox.SetFloat("_SkyBlend", m_BlendValue);

        RenderSettings.fogColor =   Color.Lerp( m_PrevFogColor, m_NextFogColor, m_BlendValue );
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

		//RenderSettings.skybox   =   rData.skyMaterial;
		ChangeMaterialTexture(rData.skyMaterial,.0f);
		m_BlendValue = .0f;

        m_PrevFogColor  =   ( m_rCurData != null )? m_rCurData.fogColor : rData.fogColor;
        m_NextFogColor  =   rData.fogColor;

		m_rCloudControl.SetSpeed( rData.cloudSpeed );
        m_rCloudControl.SetMaterial( rData.cloudMaterial );

        m_rCurData  =   rData;
    }

	void ChangeMaterialTexture(Material New,float value)
	{
		string[] PrevPropertys =
		{
			"_FrontTex_01",
			"_BackTex_01",
			"_LeftTex_01",
			"_RightTex_01",
			"_UpTex_01",
			"_DownTex_01",
		};
		string[] NextPropertys =
		{
			"_FrontTex_02",
			"_BackTex_02",
			"_LeftTex_02",
			"_RightTex_02",
			"_UpTex_02",
			"_DownTex_02",
		};
		string[] NewPropertys =
		{
			"_FrontTex",
			"_BackTex",
			"_LeftTex",
			"_RightTex",
			"_UpTex",
			"_DownTex",
		};
		//for (int i = 0; i < 6; i++)
		//{
		//	//前のNextをPrevに
		//	RenderSettings.skybox.SetTexture("_Tex", RenderSettings.skybox.GetTexture(NextPropertys[i]));
		//	//引数のマテリアルのテクスチャをNextに
		//	RenderSettings.skybox.SetTexture(NextPropertys[i], New.GetTexture(NewPropertys[i]));
		//	m_BlendValue = value;
		//	RenderSettings.skybox.SetFloat("_SkyBlend", m_BlendValue);

		//}

		//前のNextをPrevに
		RenderSettings.skybox.SetTexture("_Tex", RenderSettings.skybox.GetTexture("_Tex2"));
		//引数のマテリアルのテクスチャをNextに
		RenderSettings.skybox.SetTexture("_Tex2", New.GetTexture("_Tex"));
		m_BlendValue = value;
		RenderSettings.skybox.SetFloat("_SkyBlend", m_BlendValue);
	}
}
