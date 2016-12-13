using UnityEngine;
using System.Collections;

public class TargetingParam : MonoBehaviour {

    [SerializeField, HeaderAttribute("ロボットの狙いやすさ(大きいほど狙いやすくなる)"), Range(.01f, 10.0f)]
    public float robo_priority = 5.0f;

    [SerializeField, HeaderAttribute("小娘の狙いやすさ(大きいほど狙いやすくなる)"), Range(.01f, 10.0f)]
    public float girl_priority = 5.0f;

    [SerializeField, HeaderAttribute("拠点の狙いやすさ(大きいほど狙いやすくなる)"), Range(.01f, 10.0f)]
    private float defense_base_priority = 5.0f;

    [SerializeField, HeaderAttribute("バリケードなど障害物の狙いやすさ(大きいほど狙いやすくなる)"), Range(.01f, 10.0f)]
    private float obstacle_priority = 5.0f;

    [SerializeField, HeaderAttribute("砲台などの攻撃オブジェクトに対する狙いやすさ"), Range(.01f, 10.0f)]
    private float attack_object_priority = 5.0f;

    /**
    *@breif 対象のプライオリティを取得する
    */
    public float GetPriorityParam(PerceiveTag tag)
    {
        switch(tag)
        {
            case PerceiveTag.Girl:
                return girl_priority;

            case PerceiveTag.Robot:
                return robo_priority;

            case PerceiveTag.HomeBase:
                return defense_base_priority;

            case PerceiveTag.Obstacle:
                return obstacle_priority;

            case PerceiveTag.AttackObject:
                return attack_object_priority;


        }
        UserLog.Terauchi("EnemyParam.cs : タグ名とパラメータ名が一致していません tag名 " + tag );
        return .0f;
        
    }

}
