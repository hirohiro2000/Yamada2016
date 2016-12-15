using UnityEngine;
using System.Collections;

/**
*@brief 敵一体一体に対するparameter情報
*/
public class EnemyPersonalParametor : MonoBehaviour {

    [SerializeField, HeaderAttribute("ウェーブごとにHPをどれだけ乗算するか")]
    private float HPUpMultipleRate = 1.3f;
    public float GetHPUpMultipleRate() { return HPUpMultipleRate; }

    [SerializeField, HeaderAttribute("ウェーブごとに移動速度をどれだけ加算するか")]
    private float MoveSpeedUpMultipleRate = 1.05f;
    public float GetMoveSpeedUpMultipleRate() { return MoveSpeedUpMultipleRate; }

    [SerializeField, HeaderAttribute("最大移動速度")]
    private float MaxmoveSpeed = 1.05f;
    public float GetMaxmoveSpeed() { return MaxmoveSpeed; }

    [SerializeField, HeaderAttribute("最大加速度")]
    private float MaxAcceleration = 10;
    public float GetMaxAcceleration() { return MaxAcceleration; }

    [SerializeField, HeaderAttribute("攻撃力の加算レート(まだ未使用)")]
    private float AttackPowerIncrementRate = .0f;

    [SerializeField, HeaderAttribute("射撃のバースト弾数をウェーブごとにどれだけ加算するかのrate")]
    private float BurstIncrementRate = 0.2f;
    public float GetBurstIncrementRate() { return BurstIncrementRate; }

    [SerializeField, HeaderAttribute("バーストショットなどの最大数")]
    private int MaxBurstShot = 6;
    public int GetMaxBurstShot() { return MaxBurstShot; }

    public int m_emearge_level { get; private set; }
    
    void Awake()
    {
        m_emearge_level = 1;
    }
}
