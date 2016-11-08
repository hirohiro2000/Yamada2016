using UnityEngine;
using System.Collections;

public class EnemyDirectorAuto : EnemyDirectorBase {

    public override EnemyGenerator.EnemyData DirectionGenerateEnemy()
    {
        return new EnemyGenerator.EnemyData(
            UnityEngine.Random.Range(0, 3),
            UnityEngine.Random.Range(0, 3),
            UnityEngine.Random.Range(0, 2));
    }

}
