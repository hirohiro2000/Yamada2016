using UnityEngine;
using System.Collections;

public class EnemyDirectorBase : MonoBehaviour {

    public virtual EnemyGenerator.EnemyData DirectionGenerateEnemy()
    {
        UserLog.Terauchi("EnemyDirectorBase::DirectionGenerateEnemy call !!");
        return null;
    }
}
