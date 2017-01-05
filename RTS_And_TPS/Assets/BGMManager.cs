
using   UnityEngine;
using   System.Collections;
using   System.Collections.Generic;

public class BGMManager : MonoBehaviour {

    //  再生管理データ
    class   PlayData{
        //  再生状態
        public  enum    State{
            FadeIn,
            Play,
            FadeOut,
            End,
        }

        //  パラメータ
        public  SoundController rControl    =   null;
        public  float           maxVolume   =   0.5f;
        public  float           fadeTime    =   1.0f;
        public  float           delayTimer  =   0.0f;

        public  State           state       =   State.FadeIn;
        public  float           stateTimer  =   0.0f;

        //  初期化
        public  PlayData( SoundController _rController, float _MaxVolume, float _FadeTime, float _Delay ){
            rControl    =   _rController;
            maxVolume   =   _MaxVolume;
            fadeTime    =   _FadeTime;
            delayTimer  =   _Delay;
        }

        //  更新処理
        public  void    Update(){
            switch( state ){
                case    State.FadeIn:   Update_FadeIn();    break;
                case    State.FadeOut:  Update_FadeOut();   break;
            }
        }
        //  フェードイン
        void    Update_FadeIn(){
            //  遅延タイマー更新
            delayTimer  -=  Time.deltaTime;
            delayTimer  =   Mathf.Max( delayTimer, 0.0f );
                //  遅延中はスキップ
                if( delayTimer > 0.0f )     return;

            //  タイマー更新
            stateTimer  +=  Time.deltaTime;
            stateTimer  =   Mathf.Min( stateTimer, fadeTime );

            //  音量設定（途中でフェードアウトに移ってもブツ切れにならないよう加算式にしておく）
            rControl.m_audioSource.volume   +=  maxVolume / fadeTime * Time.deltaTime;
            rControl.m_audioSource.volume   =   Mathf.Min( rControl.m_audioSource.volume, maxVolume );

            //  フェードイン終了
            if( stateTimer >= fadeTime ){
                state   =   State.Play;
            }
        }
        //  フェードアウト
        void    Update_FadeOut(){
            //  タイマー更新
            stateTimer  +=  Time.deltaTime;
            stateTimer  =   Mathf.Min( stateTimer, fadeTime );

            //  音量設定
            rControl.m_audioSource.volume   -=  maxVolume / fadeTime * Time.deltaTime;
            rControl.m_audioSource.volume   =   Mathf.Max( rControl.m_audioSource.volume, 0.0f );

            //  フェードアウト終了
            if( rControl.m_audioSource.volume <= 0.0f
            ||  stateTimer >= fadeTime ){
                //  オブジェクト破棄
                Destroy( rControl.gameObject );

                //  終了
                rControl    =   null;
                state       =   State.End;
            }
        }

        //  フェードアウト開始
        public  void    SetFadeOut( float _FadeTime ){
            //  既にフェードアウトを開始している場合は何もしない
            if( state > State.Play )    return;

            //  フェードアウト開始
            state       =   State.FadeOut;
            stateTimer  =   0.0f;
            fadeTime    =   _FadeTime;
        }
    }

    //  内部パラメータ
    static  SoundController     m_rCurBGM   =   null;
    static  Transform           m_rInstance =   null;

    static  List< PlayData >    m_rDataList =   new List< PlayData >();

//********************************************************************************************
//      インスタンス側の処理
//********************************************************************************************
	//  開始処理
	void    Awake()
    {
        //  データを初期化
        m_rDataList.Clear();

        //  アクセスを設定
	    m_rInstance =   transform;
	}
    //  終了処理
    void    OnDestroy()
    {
        if( m_rCurBGM ){
            Destroy( m_rCurBGM.gameObject );
        }
    }
	//  更新処理
	void    Update()
    {
        //  データがない場合は処理を行わない
        if( m_rDataList.Count == 0 )    return;

        //  無効なデータを削除
	    for( int i = 0; i < m_rDataList.Count; i++ ){
            PlayData    rData   =   m_rDataList[ i ];
            if( rData.rControl )    continue;

            //  削除
            m_rDataList.Remove( rData );

            //  最初に戻る
            i   =   -1;
        }

        //  データを更新
        for( int i = 0; i < m_rDataList.Count; i++ ){
            m_rDataList[ i ].Update();
        }
	}
//********************************************************************************************
//      スタティックな処理
//********************************************************************************************
    //  BGMを変更
    static  public  void    ChangeBGM( string _BGMName, float _Volume, float _FadeInTime, float _FadeOutTime, float _Delay )
    {
        //  再生中のBGMをフェードアウトする
        for( int i = 0; i < m_rDataList.Count; i++ ){
            m_rDataList[ i ].SetFadeOut( _FadeOutTime );
        }

        //  名前が無い場合は再生しない
        if( _BGMName == "" )    return;

        //  サウンドデータを作成
        SoundController rSound  =   SoundController.PlayNow( _BGMName, m_rInstance, Vector3.zero, _Delay, 0.0f, 1.0f, -1.0f );
            //  作成失敗
            if( !rSound )       return;

        //  データリストに追加
        m_rDataList.Add( new PlayData( rSound, _Volume, _FadeInTime, _Delay ) );
    }
}
