
using   UnityEngine;
using   UnityEngine.UI;
using   System.Collections;
using   System.Collections.Generic;

public class DamageFilter_Control : MonoBehaviour {

    class   Data{
        public  float   timer   =   0.0f;
        public  float   time    =   0.0f;
        public  float   power   =   0.0f;
    }

    //public  float           c_TestTime  =   0.0f;
    //public  float           c_TestPower =   0.0f;

    private float[]         c_Curve     =   new float[]{    0.0f,  1.0f,  0.0f,  0.0f,  0.0f,  0.0f     };
    private float           c_MaxAlpha  =   190 / 255.0f;

    private List< Data >    m_rDataList =   new List< Data >();
    private Image           m_rImage    =   null;

	// Use this for initialization
	void    Start()
    {
	    m_rImage    =   GetComponent< Image >();
	}
	
	// Update is called once per frame
	void    Update()
    {
        //if( Input.GetKeyDown( KeyCode.K ) ){
        //    SetEffect( c_TestTime, c_TestPower );
        //}

	    //  データがなければ処理を行わない
        if( m_rDataList.Count == 0
        &&  m_rImage.enabled  == false )    return;

        //  不透明度を更新
        {
            //  不透明度を計算
            float   alpha   =   0.0f;
            for( int i = 0; i < m_rDataList.Count; i++ ){
                Data    rData   =   m_rDataList[ i ];

                //  タイマーを更新
                rData.timer -=  Time.deltaTime;
                rData.timer =   Mathf.Max( rData.timer, 0.0f );

                //  不透明度を計算
                float   timeRate    =   1.0f - rData.timer / rData.time;
                float   addAlpha    =   FunctionManager.CalcBezie( c_Curve, timeRate ) * rData.power;//Mathf.Sin( Mathf.PI * timeRate ) * rData.power;

                //  不透明度を合算
                alpha   +=  addAlpha;
            }

            //  不透明度を反映
            m_rImage.color      =   new Color( m_rImage.color.r, m_rImage.color.g, m_rImage.color.b, Mathf.Min( alpha, c_MaxAlpha ) );
            m_rImage.enabled    =   alpha > 0.0f;
        }

        //  無効になった項目を削除
        for( int i = 0; i < m_rDataList.Count; i++ ){
            Data    rData   =   m_rDataList[ i ];
        
            //  終了チェック
            if( rData.timer > 0.0f )    continue;
            
            //  項目を削除
            m_rDataList.Remove( rData );
        
            //  最初に戻る
            i   =   -1;
        }
	}

    public  void    SetEffect( float _Time, float _Power )
    {
        //  データを設定
        Data    rData   =   new Data();
        rData.time      =   _Time;
        rData.timer     =   _Time;
        rData.power     =   _Power;

        //  リストに登録
        m_rDataList.Add( rData );
    }
}
