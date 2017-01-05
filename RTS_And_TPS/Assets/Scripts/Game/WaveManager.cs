
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;
using   System.Collections.Generic;

public class WaveManager : NetworkBehaviour {
    
    //  内部パラメータ
    private int                 m_WaveLevel     =   0;

    //  関連アクセス
    //private EnemyShell_Control  m_rEnemyShell   =   null;

    //  外部へのアクセスr
    private GameManager         m_rGameManager  =   null;
    private EnemyGenerator      m_ganerator     =   null;

    // Use this for initialization
    void    Start()
    {
        //  アクセスを取得 
	    m_rGameManager  =   FunctionManager.GetAccessComponent< GameManager >( "GameManager" );
        //m_rEnemyShell   =   FunctionManager.GetAccessComponent< EnemyShell_Control >( "Enemy_Shell" );
        m_ganerator     =   GetComponent<EnemyGenerator>();
	}
	
	// Update is called once per frame
	void    Update()
    {
        //  サーバーでのみ処理を行う
        if( !NetworkServer.active )                                 return;
	    //  ゲーム中のみ処理を行う
        if( m_rGameManager.GetState() != GameManager.State.InGame ) return;
        //  スポーンポイントがなければ処理しない（エディット用）
        if( m_ganerator.GetNumSpawnPointList() == 0 )               return;

        //  すべての配置が終わったら全滅するまで待機
        if( CheckWhetherEmptyAllSpawner()
        &&  m_ganerator.GetCurrentAliveEnemyCount() <= 0 ){
            //  次のウェーブを用意
            //StandbyWave( 4.0f );

            //  ウェーブクリア
            if( NetworkServer.active ){
                if( m_WaveLevel % 3 == 0 )  m_rGameManager.SetMainMassage( "敵の大軍を撃退しました！！\n（ 第 " + m_WaveLevel + " ウェーブ ）", 3.7f, 1.7f );
                else                        m_rGameManager.SetMainMassage( "第 " + ( m_WaveLevel ) +  " ウェーブクリア！", 3.7f, 1.7f );
            }
            if( m_WaveLevel % 3 == 0 )  m_rGameManager.RpcMainMessage( "敵の大軍を撃退しました！！（ 第 " + m_WaveLevel + " ウェーブ ）", 3.7f, 1.7f );
            else                        m_rGameManager.RpcMainMessage( "第 " + ( m_WaveLevel ) +  " ウェーブクリア！", 3.7f, 1.7f );

            //  インターバル開始
            m_rGameManager.StartWaveInterval();
        }
	}

    //  生成器が全て空になったかどうかチェック
    bool CheckWhetherEmptyAllSpawner()
    {
        return !m_ganerator.IsGeneratingEnemy();
    }

    //  配置情報を準備 
    void        StandbyWave( float _Delay )
    {
        //  レベルアップ
        ++m_WaveLevel;

        //  配置パラメータ
        bool    isPeak      =   m_WaveLevel % 3 == 0;
        int     largeLevel  =   ( m_WaveLevel - 1 ) / 3;
        int     miniLevel   =   ( m_WaveLevel - 1 ) % 3;

		//GameWorldParameterで強制的に書き換える
		int numPop      =  (int)(( 6 + ( largeLevel * 2 + miniLevel ) * 3 )* GameWorldParameter.instance.Enemy.EmitMultiple);

        //  ピーク時は出現量２倍
        if( isPeak ){
            numPop  *=  2;
        }

        m_ganerator.BeginGenerate( m_WaveLevel, numPop, _Delay );

        // スキル初期化
        SkillInvoker.StartWave();

        //  ウェーブ数更新
        m_rGameManager.m_WaveLevel  =   m_WaveLevel;

        //  メッセージを通知 
        if( m_WaveLevel > 1 ){
            if( m_WaveLevel % 3 == 0 ){
                if( NetworkServer.active )  m_rGameManager.SetMainMassage( "敵の大軍が押し寄せています！", 3.7f, 1.2f );
                                            m_rGameManager.RpcMainMessage( "敵の大軍が押し寄せています！", 3.7f, 1.2f );
            }
            else{
                if( NetworkServer.active )  m_rGameManager.SetMainMassage( "新たな敵が接近しています", 3.7f, 1.2f );
                                            m_rGameManager.RpcMainMessage( "新たな敵が接近しています", 3.7f, 1.2f );
            }
        }
    }

    //  アクティブな子の数を取得
    int         CheckActiveChildCount()
    {
        int activeCount =   0;
        for( int i = 0; i < transform.childCount; i++ ){
            Transform   rTrans  =   transform.GetChild( i );
            if( rTrans.gameObject.activeInHierarchy == false )  continue;

             ++activeCount;
        }

        return  activeCount;
    }
    //  アクティブな子へのアクセスを取得
    Transform   GetTransformInActiveChild( int _ID )
    {
        int activeCount =   0;
        for( int i = 0; i < transform.childCount; i++ ){
            Transform   rTrans  =   transform.GetChild( i );
            if( rTrans.gameObject.activeInHierarchy == false )  continue;
            if( activeCount == _ID )                            return  rTrans;

            ++activeCount;
        }

        return  null;
    }

    //  ゲーム開始
    public  void    StartWave()
    {
        StandbyWave( 0.0f );
    }
}
