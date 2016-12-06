using UnityEngine;
using System.Collections;

public class EnemyWaveParametor : MonoBehaviour {

    [SerializeField, HeaderAttribute("1ウェーブでの最大出現数")]
    private int NumMaxSpawnEnemy = 20;
    public int GetNumMaxSpawnEnemy() { return NumMaxSpawnEnemy; }

    [SerializeField,HeaderAttribute("第一ウェーブでの敵の数")]
    private int NumStartSpawnEnemy = 4;
    public int GetNumStartSpawnEnemy() { return NumStartSpawnEnemy; }

    [SerializeField, HeaderAttribute("ウェーブごとに増やす出現数の最大(増大数 +=NumIncrementSpawnEnemy  * rand(0.5 ~ 1.0)")]
    private int NumIncrementSpawnEnemy = 4;
    public int GetNumIncrementSpawnEnemy() { return NumIncrementSpawnEnemy; }

    [SerializeField, HeaderAttribute("ウェーブごとにHPをどれだけ加算するか")]
    private float HPUpPoint = 40.0f;
    public float GetHPUpPoint() { return HPUpPoint; }

    [SerializeField, HeaderAttribute("バーストショットなどの最大数")]
    private int MaxBurstShot = 6;
    public int GetMaxBurstShot() { return MaxBurstShot; }

    [SerializeField, HeaderAttribute("射撃のバースト弾数をウェーブごとにどれだけ増加するかのrate")]
    private float BurstIncrementRate = 0.2f;
    public float GetBurstIncrementRate() { return BurstIncrementRate; }

    [SerializeField,HeaderAttribute("攻撃力の加算レート(まだ未使用)")]
    private float AttackPowerIncrementRate = .0f;

    //private StringList RouteList = null;

    public int m_current_level { get; private set; }

    void Awake()
    {
        m_current_level = 0;
    }

    public void LevelUp()
    {
        m_current_level++;
    }
}
