
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;
using   System.Collections.Generic;

public class WaveManager : MonoBehaviour {

    //  生成情報
    class   SpawnData{
        public  int enemyID;
        public  int level;
        public  SpawnData(){
            enemyID =   0;
            level   =   0;
        }
        public  SpawnData( int _EnemyID, int _Level ){
            enemyID =   _EnemyID;
            level   =   _Level;
        }
    }
    //  生成クラス
    class   EnemySpawner{
        public  int                 c_SpawnerID     =   0;

        private float               c_PopInterval   =   1.0f;

        private Queue< SpawnData >  m_rWaveQueue    =   new Queue< SpawnData >();
        private float               m_IntervalTimer =   0;

        //  更新
        public  void    Update( WaveManager _rWManager ){

            return;
            //  未配置のエネミーがいなければ処理は行わない
            if( m_rWaveQueue.Count == 0 )   return;
            
            //  タイマー更新
            m_IntervalTimer =   Mathf.Max( m_IntervalTimer - Time.deltaTime, 0.0f );
            //  次の配置時間までスキップ
            if( m_IntervalTimer > 0.0f )    return;

            //  配置処理
            {
                //  配置
                SpawnData   rData       =   m_rWaveQueue.Peek();
                _rWManager.PopEnemy( c_SpawnerID, rData.enemyID, rData.level );

                //  配置情報削除
                m_rWaveQueue.Dequeue();

                //  インターバルリセット
                m_IntervalTimer =   c_PopInterval;
            }
        }

        //  配置情報追加
        public  void    AddSpawnData( SpawnData _rData ){
            m_rWaveQueue.Enqueue( _rData );
        }
        //  全て吐き出したかどうかチェック
        public  bool    IsEmpty(){
            return  m_rWaveQueue.Count == 0;
        }
    }

    //  公開パラメータ
	public  GameObject[]        c_EnemyPrefab   =   null;

    //  内部パラメータ
    private int                 m_WaveLevel     =   0;
    private EnemySpawner[]      m_rSpawner      =   new EnemySpawner[ 3 ];

    //  関連アクセス
    private EnemyShell_Control  m_rEnemyShell   =   null;

    //  外部へのアクセスr
    private GameManager         m_rGameManager  =   null;
    private EnemyGenerator       m_ganerator = null;

    // Use this for initialization
    void    Start()
    {
        //  アクセスを取得
	    m_rGameManager  =   FunctionManager.GetAccessComponent< GameManager >( "GameManager" );
        m_rEnemyShell   =   FunctionManager.GetAccessComponent< EnemyShell_Control >( "Enemy_Shell" );
        m_ganerator = GetComponent<EnemyGenerator>();
        
        //  スポーナー初期化
        for( int i = 0; i < m_rSpawner.Length; i++ ){
            m_rSpawner[ i ]             =   new EnemySpawner();
            m_rSpawner[ i ].c_SpawnerID =   i;
        }

        //  ウェーブ情報用意
        StandbyWave();
	}
	
	// Update is called once per frame
	void    Update()
    {
	    //  ゲーム中のみ処理を行う
        if( m_rGameManager.GetState() != GameManager.State.InGame ) return;

        //  スポーナー更新
        //for( int i = 0; i < m_rSpawner.Length; i++ ){
        //    m_rSpawner[ i ].Update( this );
        //}

        //  すべての配置が終わったら全滅するまで待機
        if( CheckWhetherEmptyAllSpawner()
        && m_ganerator.GetCurrentAliveEnemyCount() <= 0 ){
            //  次のウェーブを用意
            StandbyWave();
            //  ウェーブクリア
            m_rGameManager.RpcMainMessage( "第 " + ( m_WaveLevel - 1 ) +  " ウェーブクリア！", 3.7f, 1.7f );
            //  敵の襲来を通知
            if( m_WaveLevel % 3 == 0 )  m_rGameManager.RpcMainMessage( "敵の大軍が押し寄せています！", 3.7f, 1.2f );
            else                        m_rGameManager.RpcMainMessage( "新たな敵が接近しています", 3.7f, 1.2f );
        }
	}

    //  生成器が全て空になったかどうかチェック
    bool CheckWhetherEmptyAllSpawner()
    {
        return !m_ganerator.IsGeneratingEnemy();
       // return  true;
    }

    //  配置情報を準備
    void        StandbyWave()
    {
        //  レベルアップ
        ++m_WaveLevel;

        //  配置パラメータ
        bool    isPeak      =   m_WaveLevel % 3 == 0;
        int     largeLevel  =   ( m_WaveLevel - 1 ) / 3;
        int     miniLevel   =   ( m_WaveLevel - 1 ) % 3;
        int     numPop      =   10 + ( largeLevel + miniLevel ) * 3;

        //  ピーク時は出現量２倍
        if( isPeak ){
            numPop  *=  2;
        }

        m_ganerator.BeginGenerate(m_WaveLevel, numPop,10.0f);
        //  配置
        //for( int i = 0; i < numPop; i++ ){
        //    //  配置情報設定
        //    SpawnData   rData   =   new SpawnData();
        //    rData.enemyID       =   0;
        //    rData.level         =   Random.Range( 0, m_WaveLevel );

        //    //  配置場所決定
        //    int useSpanerID     =   Random.Range( 0, ( isPeak )? 3 : 2 );

        //    //  配置情報セット
        //    m_rSpawner[ useSpanerID ].AddSpawnData( rData );
        //}
    }
    //  エネミー配置
    void        PopEnemy( int _SpawnPointID, int _EnemyID, int _Level )
    {
        //Transform   rSpawnPoint =   GetTransformInActiveChild( _SpawnPointID );
        //GameObject  rObj        =   Instantiate( c_EnemyPrefab[ _EnemyID ] );
        //Transform   rTrans      =   rObj.transform;

        //rObj.transform.parent = transform;

        ////  配置設定
        //{
        //    //  座標
        //    NavMeshAgent    rAgent  =   rObj.GetComponent< NavMeshAgent >();
        //    if( rAgent )    rAgent.Warp( rSpawnPoint.position );
        //    else            rTrans.position =   rSpawnPoint.position;

        //    //  向き
        //    rTrans.rotation     =   rSpawnPoint.rotation;
        //}

        ////  パラメータ設定
        //{
        //    TPS_Enemy   rTPSEnemy   =   rObj.GetComponent< TPS_Enemy >();
        //    if( rTPSEnemy ){
        //        rTPSEnemy.m_MaxHP   =   rTPSEnemy.m_MaxHP * Mathf.Max( 1, _Level * 1.5f );
        //    }
        //}
        
        ////  ネットワーク上で生成
        //NetworkServer.Spawn( rObj );
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
}
