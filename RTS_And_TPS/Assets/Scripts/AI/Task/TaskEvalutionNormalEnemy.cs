using UnityEngine;
using System.Collections;


/**
*@brief 基本的な敵クラス（近距離攻撃と） 
*/
public class TaskEvalutionNormalEnemy : TaskEvaluationBase
{

    public override bool Execute(
        TargetingSystem target_info,
        EnemyTaskDirector director)
    {
        return false;
    }
}
