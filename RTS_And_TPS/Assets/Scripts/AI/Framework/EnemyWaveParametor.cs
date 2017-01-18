
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/**
*@brief 敵の全体的なparameter情報
*/
public class EnemyWaveParametor : NetworkBehaviour {

    //[SerializeField, HeaderAttribute("1ウェーブでの最大出現数")]
    //private int NumMaxSpawnEnemy = 20;
    //public int GetNumMaxSpawnEnemy() { return NumMaxSpawnEnemy; }

    //[SerializeField,HeaderAttribute("第一ウェーブでの敵の数")]
    //private int NumStartSpawnEnemy = 4;
    //public int GetNumStartSpawnEnemy() { return NumStartSpawnEnemy; }

    //[SerializeField, HeaderAttribute("ウェーブごとに増やす出現数の最大(増大数 +=NumIncrementSpawnEnemy  * rand(0.5 ~ 1.0)")]
    //private int NumIncrementSpawnEnemy = 4;
    //public int GetNumIncrementSpawnEnemy() { return NumIncrementSpawnEnemy; }

    [ SyncVar ]
    public int m_current_level  =   0;// { get; private set; }

    void Awake()
    {
        m_current_level = 0;
    }

    public void LevelUp()
    {
        m_current_level++;
    }
    public  void    SetLevel( int _Level ){
        m_current_level =   _Level;
    }
}
