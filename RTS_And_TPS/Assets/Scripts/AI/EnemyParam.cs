using UnityEngine;
using System.Collections;

public class EnemyParam : MonoBehaviour {

    [SerializeField, HeaderAttribute("ロボットの狙いやすさ(大きいほど狙いやすくなる)"), Range(.0f, 1.0f)]
    public float robo_attack_priority = 0.5f;

    [SerializeField, HeaderAttribute("小娘の狙いやすさ(大きいほど狙いやすくなる)"), Range(.0f, 1.0f)]
    public float girl_attack_priority = 0.5f;

    [SerializeField, HeaderAttribute("拠点の狙いやすさ(大きいほど狙いやすくなる)"), Range(.0f, 1.0f)]
    private float defense_base_attack_priority = 0.5f;

    [SerializeField, HeaderAttribute("バリケードなど障害物の狙いやすさ(大きいほど狙いやすくなる)"), Range(.0f, 1.0f)]
    private float obstacle_attack_priority = 0.5f;

    /**
    *@breif 攻撃対象のプライオリティを取得する
    */
    public float GetAttackPriorityParam(string tag_name)
    {
        switch(tag_name)
        {
            case "Girl":
                return girl_attack_priority;

            case "Robot":
                return robo_attack_priority;

            case "DefenseBase":
                return defense_base_attack_priority;

            case "Obstacle":
                return obstacle_attack_priority;

        }
        Debug.Log("EnemyParam.cs : タグ名とパラメータ名が一致していません");
        return .0f;
        
    }

}
