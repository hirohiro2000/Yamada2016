
using   UnityEngine;
using   System.Collections;
using   System.Collections.Generic;

public class Shaker_Control : MonoBehaviour {

    //  振動データ
    class   ShakeData{
        public  float   timer       =   0.0f;
        public  float   shakeTime   =   0.0f;
        public  float   frequency   =   0.0f;
        public  float   power       =   0.0f;
        public  Vector3 direction   =   Vector3.zero;
    }

    //  公開パラメータ
    public  float               c_ShakeRatio    =   1.0f;

    //  内部パラメータ
    private float[]             c_Curve         =   new float[]{   1.0f,   0.31f,   0.15f,   0.1f,  0.0f    };

    private Vector3             m_DefaultPos    =   Vector3.zero;
    private List< ShakeData >   m_rDataList     =   new List< ShakeData >();

	// Use this for initialization
	void    Start()
    {
        m_DefaultPos    =   transform.localPosition;
	}
	
	// Update is called once per frame
	void    Update()
    {
        //  データがなければ処理を行わない
        if( m_rDataList.Count == 0 )    return;

        //  振動処理
        {
            Vector3 shakeSum    =   Vector3.zero;
            for( int i = 0; i < m_rDataList.Count; i++ ){
                ShakeData   rData   =   m_rDataList[ i ];

                //  タイマーを更新
                rData.timer -=  Time.deltaTime;
                rData.timer =   Mathf.Max( rData.timer, 0.0f );

                //  振動を加算
                float   timeRate    =   1.0f - rData.timer / rData.shakeTime;
                float   powerInime  =   FunctionManager.CalcBezie( c_Curve, timeRate );
                shakeSum    +=  rData.direction * rData.power * Mathf.Sin( Mathf.PI * 2.0f * rData.frequency * timeRate  ) * powerInime;
            }

            //  振動を反映
            transform.localPosition =   m_DefaultPos + shakeSum * c_ShakeRatio;
        }

        //  項目をチェック
        for( int i = 0; i < m_rDataList.Count; i++ ){
            ShakeData   rData   =   m_rDataList[ i ];
            //  終了チェック
            if( rData.timer > 0.0f )    continue;

            //  項目を削除
            m_rDataList.Remove( rData );

            //  最初から
            i   =   -1;
        }
	}

    public  void    SetShake( Vector3 _Direction, float _Frequency, float _ShakeTime, float _ShakePower )
    {
        //  パラメータを設定 
        ShakeData   data    =   new ShakeData();
        data.timer      =   _ShakeTime;
        data.shakeTime  =   _ShakeTime;
        data.frequency  =   _Frequency;
        data.power      =   _ShakePower;
        data.direction  =   _Direction;
        
        //  リストに登録
        m_rDataList.Add( data );
    }
}
