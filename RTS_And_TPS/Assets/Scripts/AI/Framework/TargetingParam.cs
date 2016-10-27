using UnityEngine;
using System.Collections;

public class TargetingParam : MonoBehaviour {

    [SerializeField, HeaderAttribute("ロボットの狙いやすさ(大きいほど狙いやすくなる)"), Range(.1f, 10.0f)]
    public float robo_priority = 5.0f;

    [SerializeField, HeaderAttribute("小娘の狙いやすさ(大きいほど狙いやすくなる)"), Range(.1f, 10.0f)]
    public float girl_priority = 5.0f;

    [SerializeField, HeaderAttribute("拠点の狙いやすさ(大きいほど狙いやすくなる)"), Range(.1f, 10.0f)]
    private float defense_base_priority = 5.0f;

    [SerializeField, HeaderAttribute("バリケードなど障害物の狙いやすさ(大きいほど狙いやすくなる)"), Range(.1f, 10.0f)]
    private float obstacle_priority = 5.0f;

    /**
    *@breif 対象のプライオリティを取得する
    */
    public float GetPriorityParam(string tag_name)
    {
        switch(tag_name)
        {
            case "Girl":
                return girl_priority;

            case "Robot":
                return robo_priority;

            case "DefenseBase":
                return defense_base_priority;

            case "Obstacle":
                return obstacle_priority;

        }
        Debug.Log("EnemyParam.cs : タグ名とパラメータ名が一致していません");
        return .0f;
        
    }

}
